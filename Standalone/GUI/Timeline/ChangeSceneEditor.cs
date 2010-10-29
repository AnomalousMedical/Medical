using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ChangeSceneEditor : Dialog
    {
        private ComboBox sceneCombo;
        private CheckButton openSceneCheck;
        private OpenNewSceneAction newSceneAction;
        private Timeline currentTimeline;

        public ChangeSceneEditor()
            : base("Medical.GUI.Timeline.ChangeSceneEditor.layout")
        {
            openSceneCheck = new CheckButton(window.findWidget("OpenSceneCheck") as Button);
            sceneCombo = window.findWidget("SceneCombo") as ComboBox;
            findSceneFiles();

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);
        }

        protected override void onShown(EventArgs args)
        {
            if (newSceneAction == null)
            {
                openSceneCheck.Checked = false;
                if (sceneCombo.ItemCount > 0)
                {
                    sceneCombo.SelectedIndex = 0;
                }
            }
            else
            {
                openSceneCheck.Checked = true;
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
            base.onShown(args);
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (openSceneCheck.Checked)
            {
                if (newSceneAction == null)
                {
                    newSceneAction = new OpenNewSceneAction();
                    currentTimeline.addPreAction(newSceneAction);
                }
                newSceneAction.Scene = sceneCombo.getItemDataAt(sceneCombo.SelectedIndex).ToString();
            }
            else
            {
                if (newSceneAction != null)
                {
                    currentTimeline.removePreAction(newSceneAction);
                    newSceneAction = null;
                }
            }
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
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
                currentTimeline = value;
                foreach (TimelineInstantAction instantAction in currentTimeline.PreActions)
                {
                    newSceneAction = instantAction as OpenNewSceneAction;
                    if (newSceneAction != null)
                    {
                        break;
                    }
                }
            }
        }
    }
}
