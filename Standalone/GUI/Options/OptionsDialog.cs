using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using OgrePlugin;
using Logging;
using System.Text.RegularExpressions;
using Engine.Platform;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    public class OptionsDialog : AbstractFullscreenGUIPopup
    {
        public event EventHandler VideoOptionsChanged;
        public event EventHandler RequestRestart;

        private GUIManager guiManager;

        private ComboBox cameraSpeedCombo;

        private ComboBox aaCombo;
        private ComboBox resolutionCombo;
        private ComboBox cameraButtonCombo;
        private ComboBox defaultSceneCombo;
        private ComboBox uiSize;
        private CheckButton fullscreenCheck;
        private CheckButton vsyncCheck;
        private CheckButton showStatsCheck;
        private CheckButton enableMultitouchCheck;
        private CheckButton autoOpenAnatomyFinder;
        private static readonly char[] seps = { 'x' };
        private const String resolutionRegex = "[1-9][0-9]* x [1-9][0-9]*";
        private NumericEdit maxFPS;

        public OptionsDialog(GUIManager guiManager)
            :base("Medical.GUI.Options.OptionsDialog.layout", guiManager)
        {
            this.SmoothShow = true;
            this.guiManager = guiManager;

            cameraSpeedCombo = widget.findWidget("CameraSpeedCombo") as ComboBox;
            enableMultitouchCheck = new CheckButton(widget.findWidget("EnableMultitouch") as Button);

            aaCombo = widget.findWidget("AACombo") as ComboBox;
            resolutionCombo = widget.findWidget("ResolutionCombo") as ComboBox;
            cameraButtonCombo = widget.findWidget("CameraButtonCombo") as ComboBox;

            uiSize = (ComboBox)widget.findWidget("UISize");
            uiSize.addItem("Smaller", UIExtraScale.Smaller);
            uiSize.addItem("Normal", UIExtraScale.Normal);
            uiSize.addItem("Larger", UIExtraScale.Larger);

            RenderSystem rs = Root.getSingleton().getRenderSystem();
            if (rs.hasConfigOption("FSAA"))
            {
                aaCombo.Enabled = true;
                ConfigOption configOption = rs.getConfigOption("FSAA");
                foreach (String value in configOption.PossibleValues)
                {
                    aaCombo.addItem(value);
                }
            }
            else
            {
                aaCombo.Enabled = false;
            }
            if (rs.hasConfigOption("Video Mode"))
            {
                resolutionCombo.Enabled = true;
                ConfigOption configOption = rs.getConfigOption("Video Mode");
                foreach (String value in configOption.PossibleValues)
                {
                    Match match = Regex.Match(value, resolutionRegex);
                    String resString = value.Substring(match.Index, match.Length);
                    if (resolutionCombo.findItemIndexWith(resString) == uint.MaxValue)
                    {
                        resolutionCombo.addItem(resString);
                    }
                }
            }

            defaultSceneCombo = (ComboBox)widget.findWidget("DefaultScene");
            findSceneFiles();

            fullscreenCheck = new CheckButton(widget.findWidget("FullscreenCheck") as Button);
            vsyncCheck = new CheckButton(widget.findWidget("VSyncCheck") as Button);
            showStatsCheck = new CheckButton(widget.findWidget("ShowStatsCheck") as Button);
            autoOpenAnatomyFinder = new CheckButton((Button)widget.findWidget("AutoOpenAnatomyFinder"));

            Button applyButton = widget.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = widget.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button resetWindows = widget.findWidget("ResetWindows") as Button;
            resetWindows.MouseButtonClick += new MyGUIEvent(resetWindows_MouseButtonClick);

            maxFPS = new NumericEdit((EditBox)widget.findWidget("MaxFPS"));
            maxFPS.MinValue = 0;
            maxFPS.MaxValue = 1000;

            this.Showing += new EventHandler(OptionsDialog_Showing);
        }

        void OptionsDialog_Showing(object sender, EventArgs e)
        {
            //Program options
            float cameraTransitionTime = MedicalConfig.CameraTransitionTime;
            if (cameraTransitionTime >= 1.0f)
            {
                cameraSpeedCombo.SelectedIndex = 3;
            }
            else if (cameraTransitionTime >= 0.5f)
            {
                cameraSpeedCombo.SelectedIndex = 2;
            }
            else if (cameraTransitionTime >= 0.25f)
            {
                cameraSpeedCombo.SelectedIndex = 1;
            }
            else
            {
                cameraSpeedCombo.SelectedIndex = 0;
            }

            enableMultitouchCheck.Checked = MedicalConfig.EnableMultitouch;

            MouseButtonCode cameraButtonCode = MedicalConfig.CameraMouseButton;
            cameraButtonCombo.SelectedIndex = (uint)cameraButtonCode;

            //Default Scene
            uint count = defaultSceneCombo.ItemCount;
            String defaultScene = MedicalConfig.DefaultScene;
            bool defaultSceneNotFound = true;
            for (uint i = 0; i < defaultSceneCombo.ItemCount; ++i)
            {
                if (defaultSceneCombo.getItemDataAt(i).ToString() == defaultScene)
                {
                    defaultSceneCombo.SelectedIndex = i;
                    defaultSceneNotFound = false;
                    break;
                }
            }
            if (defaultSceneNotFound)
            {
                defaultSceneCombo.SelectedIndex = 0;
            }

            uint scalingIndex = uiSize.findItemIndexWithData(MedicalConfig.ExtraScaling);
            if (scalingIndex != ComboBox.Invalid)
            {
                uiSize.SelectedIndex = scalingIndex;
            }
            else
            {
                uiSize.SelectedIndex = uiSize.findItemIndexWithData(UIExtraScale.Normal);
            }

            //Graphics Options
            aaCombo.SelectedIndex = aaCombo.findItemIndexWith(OgreConfig.FSAA);
            fullscreenCheck.Checked = MedicalConfig.EngineConfig.Fullscreen;
            vsyncCheck.Checked = OgreConfig.VSync;
            showStatsCheck.Checked = MedicalConfig.EngineConfig.ShowStatistics;
            autoOpenAnatomyFinder.Checked = MedicalConfig.AutoOpenAnatomyFinder;

            String resString = String.Format("{0} x {1}", MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes);
            uint resIndex = resolutionCombo.findItemIndexWith(resString);
            if (resIndex == uint.MaxValue)
            {
                resolutionCombo.addItem(resString);
                resolutionCombo.SelectedIndex = resolutionCombo.ItemCount - 1;
            }
            else
            {
                resolutionCombo.SelectedIndex = resIndex;
            }

            uint aaIndex = aaCombo.findItemIndexWith(OgreConfig.FSAA);
            if (aaIndex == uint.MaxValue)
            {
                if (aaCombo.ItemCount == 0)
                {
                    aaCombo.addItem(OgreConfig.FSAA);
                    aaCombo.SelectedIndex = aaCombo.ItemCount - 1;
                }
                else
                {
                    aaCombo.SelectedIndex = 0;
                }
            }
            else
            {
                aaCombo.SelectedIndex = aaIndex;
            }

            maxFPS.IntValue = MedicalConfig.EngineConfig.FPSCap;
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            switch(cameraSpeedCombo.SelectedIndex)
            {
                case 0:
                    MedicalConfig.CameraTransitionTime = 0.01f;
                    break;
                case 1:
                    MedicalConfig.CameraTransitionTime = 0.25f;
                    break;
                case 2:
                    MedicalConfig.CameraTransitionTime = 0.5f;
                    break;
                case 3:
                    MedicalConfig.CameraTransitionTime = 1.0f;
                    break;
            }
            MedicalConfig.EnableMultitouch = enableMultitouchCheck.Checked;
            MedicalConfig.EngineConfig.ShowStatistics = showStatsCheck.Checked;
            MouseButtonCode cameraButtonCode = (MouseButtonCode)cameraButtonCombo.SelectedIndex;
            MedicalConfig.CameraMouseButton = cameraButtonCode;
            CameraInputController.changeMouseButton(cameraButtonCode);
            MedicalConfig.DefaultScene = defaultSceneCombo.SelectedItemData.ToString();
            MedicalConfig.AutoOpenAnatomyFinder = autoOpenAnatomyFinder.Checked;

            bool videoOptionsChanged = false;

            if (MedicalConfig.ExtraScaling != (UIExtraScale)uiSize.SelectedItemData)
            {
                MedicalConfig.ExtraScaling = (UIExtraScale)uiSize.SelectedItemData;
                videoOptionsChanged = true;
            }

            if (OgreConfig.FSAA != aaCombo.getItemNameAt(aaCombo.SelectedIndex))
            {
                OgreConfig.FSAA = aaCombo.getItemNameAt(aaCombo.SelectedIndex);
                videoOptionsChanged = true;
            }
            if (OgreConfig.VSync != vsyncCheck.Checked)
            {
                OgreConfig.VSync = vsyncCheck.Checked;
                videoOptionsChanged = true;
            }
            if (MedicalConfig.EngineConfig.Fullscreen != fullscreenCheck.Checked)
            {
                MedicalConfig.EngineConfig.Fullscreen = fullscreenCheck.Checked;
                videoOptionsChanged = true;
            }
            String[] res = resolutionCombo.getItemNameAt(resolutionCombo.SelectedIndex).Split(seps, StringSplitOptions.RemoveEmptyEntries);
            int horizRes = NumberParser.ParseInt(res[0]);
            int vertRes = NumberParser.ParseInt(res[1]);
            if(MedicalConfig.EngineConfig.HorizontalRes != horizRes || MedicalConfig.EngineConfig.VerticalRes != vertRes)
            {
                MedicalConfig.EngineConfig.HorizontalRes = horizRes;
                MedicalConfig.EngineConfig.VerticalRes = vertRes;
                videoOptionsChanged = true;
            }
            int maxFpsValue = maxFPS.IntValue;
            if (maxFpsValue < EngineConfig.MinimumAllowedFramerate && maxFpsValue != 0)
            {
                maxFpsValue = EngineConfig.MinimumAllowedFramerate;
            }
            if (MedicalConfig.EngineConfig.FPSCap != maxFpsValue)
            {
                MedicalConfig.EngineConfig.FPSCap = maxFpsValue;
                videoOptionsChanged = true;
            }
            if (videoOptionsChanged && VideoOptionsChanged != null)
            {
                VideoOptionsChanged.Invoke(this, EventArgs.Empty);
            }
            this.hide();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void resetWindows_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("This will reset all window positions back to their defaults.\nIt will remove all icons from the Task Bar and you will have to put them back.\nAnomalous Medical will need to restart and you will lose any unsaved data.\nAre you sure you want to continue?", "Restart", MessageBoxStyle.IconInfo | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
            {
                if (result == MessageBoxStyle.Yes)
                {
                    if (guiManager.deleteWindowsFile())
                    {
                        if (RequestRestart != null)
                        {
                            RequestRestart.Invoke(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        MessageBox.show(String.Format("Could not delete windows.ini file located at '{0}'.\nPlease delete this file manually to reset your UI.", MedicalConfig.WindowsFile), "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            });
        }

        void findSceneFiles()
        {
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            String sceneDirectory = MedicalConfig.SceneDirectory;
            IEnumerable<String> files = archive.listFiles(sceneDirectory, "*.sim.xml", false);
            foreach (String file in files)
            {
                String fileName = VirtualFileSystem.GetFileName(file);
                String baseName = fileName.Substring(0, fileName.IndexOf('.'));
                defaultSceneCombo.addItem(baseName, fileName);
            }
        }
    }
}
