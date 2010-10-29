using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using OgrePlugin;
using Logging;
using System.Text.RegularExpressions;

namespace Medical.GUI
{
    class OptionsDialog : Dialog
    {
        public event EventHandler VideoOptionsChanged;

        private ComboBox cameraSpeedCombo;

        private ComboBox aaCombo;
        private ComboBox resolutionCombo;
        private CheckButton fullscreenCheck;
        private CheckButton vsyncCheck;
        private CheckButton showStatsCheck;
        private static readonly char[] seps = { 'x' };
        private const String resolutionRegex = "[1-9][0-9]* x [1-9][0-9]*";

        public OptionsDialog()
            :base("Medical.Controller.GUIController.Options.OptionsDialog.layout")
        {
            this.Modal = true;
            this.SmoothShow = true;

            cameraSpeedCombo = window.findWidget("CameraSpeedCombo") as ComboBox; 

            aaCombo = window.findWidget("AACombo") as ComboBox;
            resolutionCombo = window.findWidget("ResolutionCombo") as ComboBox;

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

            fullscreenCheck = new CheckButton(window.findWidget("FullscreenCheck") as Button);
            vsyncCheck = new CheckButton(window.findWidget("VSyncCheck") as Button);
            showStatsCheck = new CheckButton(window.findWidget("ShowStatsCheck") as Button);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        protected override void onShown(EventArgs args)
        {
            base.onShown(args);

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

            //Graphics Options
            aaCombo.SelectedIndex = aaCombo.findItemIndexWith(OgreConfig.FSAA);
            fullscreenCheck.Checked = MedicalConfig.EngineConfig.Fullscreen;
            vsyncCheck.Checked = OgreConfig.VSync;
            showStatsCheck.Checked = MedicalConfig.EngineConfig.ShowStatistics;

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
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            switch(cameraSpeedCombo.SelectedIndex)
            {
                case 0:
                    MedicalConfig.CameraTransitionTime = 0.01f;
                    MedicalConfig.TransparencyChangeMultiplier = 1000.0f;
                    break;
                case 1:
                    MedicalConfig.CameraTransitionTime = 0.25f;
                    MedicalConfig.TransparencyChangeMultiplier = 5.0f;
                    break;
                case 2:
                    MedicalConfig.CameraTransitionTime = 0.5f;
                    MedicalConfig.TransparencyChangeMultiplier = 2.0f;
                    break;
                case 3:
                    MedicalConfig.CameraTransitionTime = 1.0f;
                    MedicalConfig.TransparencyChangeMultiplier = 1.0f;
                    break;
            }
            MedicalConfig.EngineConfig.ShowStatistics = showStatsCheck.Checked;

            bool videoOptionsChanged = false;

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
            int horizRes = int.Parse(res[0]);
            int vertRes = int.Parse(res[1]);
            if(MedicalConfig.EngineConfig.HorizontalRes != horizRes || MedicalConfig.EngineConfig.VerticalRes != vertRes)
            {
                MedicalConfig.EngineConfig.HorizontalRes = horizRes;
                MedicalConfig.EngineConfig.VerticalRes = vertRes;
                videoOptionsChanged = true;
            }
            if (videoOptionsChanged && VideoOptionsChanged != null)
            {
                VideoOptionsChanged.Invoke(this, EventArgs.Empty);
            }
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }
    }
}
