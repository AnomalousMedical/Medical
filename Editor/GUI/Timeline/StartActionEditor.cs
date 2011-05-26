using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class StartActionEditor : Dialog
    {
        private ComboBox sceneCombo;
        private CheckButton changeSceneCheck;
        private CheckButton showGuiCheck;
        private Button openGUIEditorButton;
        private OpenNewSceneAction newSceneAction;
        private ShowTimelineGUIAction showGUIAction;
        private Timeline currentTimeline;

        private ShowGUIEditor showGUIEditor;

        public StartActionEditor(TimelineFileBrowserDialog fileBrowser, TimelineController timelineController)
            :base("Medical.GUI.Timeline.StartActionEditor.layout")
        {
            changeSceneCheck = new CheckButton(window.findWidget("ChangeSceneCheck") as Button);
            changeSceneCheck.CheckedChanged += new MyGUIEvent(changeSceneCheck_CheckedChanged);
            sceneCombo = window.findWidget("SceneCombo") as ComboBox;
            findSceneFiles();

            showGuiCheck = new CheckButton(window.findWidget("ShowGUICheck") as Button);
            showGuiCheck.CheckedChanged += new MyGUIEvent(showGuiCheck_CheckedChanged);
            openGUIEditorButton = window.findWidget("OpenGUIEditorButton") as Button;
            openGUIEditorButton.MouseButtonClick += new MyGUIEvent(openGUIEditorButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            showGUIEditor = new ShowGUIEditor(fileBrowser, timelineController);
        }

        protected override void onShown(EventArgs args)
        {
            if (newSceneAction == null)
            {
                changeSceneCheck.Checked = false;
                if (sceneCombo.ItemCount > 0)
                {
                    sceneCombo.SelectedIndex = 0;
                }
            }
            else
            {
                changeSceneCheck.Checked = true;
                String fileName = VirtualFileSystem.GetFileName(newSceneAction.Scene);
                String baseName = fileName.Substring(0, fileName.IndexOf('.'));
                uint index = sceneCombo.findItemIndexWith(baseName);
                if (index < sceneCombo.ItemCount)
                {
                    sceneCombo.SelectedIndex = index;
                }
                else
                {
                    MessageBox.show(String.Format("Could not find the scene {0} specified by this timeline.", newSceneAction.Scene), "Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
            }
            sceneCombo.Enabled = changeSceneCheck.Checked;

            openGUIEditorButton.Enabled = showGuiCheck.Checked = showGUIAction != null;
            if (showGUIAction != null)
            {
                showGUIEditor.setProperties(showGUIAction);
            }
            
            base.onShown(args);
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (changeSceneCheck.Checked)
            {
                if (newSceneAction == null)
                {
                    newSceneAction = new OpenNewSceneAction();
                    currentTimeline.addPreAction(newSceneAction);
                }
                newSceneAction.Scene = sceneCombo.getItemDataAt(sceneCombo.SelectedIndex).ToString();
            }
            else if (newSceneAction != null)
            {
                currentTimeline.removePreAction(newSceneAction);
                newSceneAction = null;
            }

            //Show GUI
            if (showGUIAction != null)
            {
                //Always remove the old action
                currentTimeline.removePreAction(showGUIAction);
                showGUIAction = null;
            }

            if (showGuiCheck.Checked)
            {
                //Add the action back in if appropriate
                showGUIAction = showGUIEditor.createAction();
                currentTimeline.addPreAction(showGUIAction);
            }

            this.close();

            showGUIEditor.clear();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();

            showGUIEditor.clear();
        }

        void findSceneFiles()
        {
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            String sceneDirectory = MedicalConfig.SceneDirectory;
            String[] files = archive.listFiles(sceneDirectory, "*.sim.xml", false);
            foreach (String file in files)
            {
                String fileName = VirtualFileSystem.GetFileName(file);
                String baseName = fileName.Substring(0, fileName.IndexOf('.'));
                sceneCombo.addItem(baseName, file);
            }
            if (sceneCombo.ItemCount > 0)
            {
                sceneCombo.SelectedIndex = 0;
            }
        }

        public Timeline CurrentTimeline
        {
            get
            {
                return currentTimeline;
            }
            set
            {
                newSceneAction = null;
                showGUIAction = null;
                currentTimeline = value;
                foreach (TimelineInstantAction instantAction in currentTimeline.PreActions)
                {
                    if (newSceneAction == null)
                    {
                        newSceneAction = instantAction as OpenNewSceneAction;
                    }
                    if (showGUIAction == null)
                    {
                        showGUIAction = instantAction as ShowTimelineGUIAction;
                    }
                }
            }
        }

        void changeSceneCheck_CheckedChanged(Widget source, EventArgs e)
        {
            sceneCombo.Enabled = changeSceneCheck.Checked;
        }

        void showGuiCheck_CheckedChanged(Widget source, EventArgs e)
        {
            openGUIEditorButton.Enabled = showGuiCheck.Checked;
        }

        void openGUIEditorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            showGUIEditor.open(true);
        }
    }
}
