using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;
using Engine;
using System.IO;
using System.Drawing;

namespace Medical.GUI
{
    class ChooseSceneDialog : Dialog
    {
        private StandaloneController controller;
        private ButtonGrid sceneFileGrid;
        private ImageAtlas imageAtlas;
        private PiperJBOGUI piperGUI;

        public ChooseSceneDialog(StandaloneController controller, PiperJBOGUI piperGUI)
            : base("Medical.GUI.AppMenu.Dialogs.ChooseSceneDialog.layout")
        {
            this.controller = controller;
            this.piperGUI = piperGUI;

            Button openButton = window.findWidget("ChooseScene/Open") as Button;
            Button cancelButton = window.findWidget("ChooseScene/Cancel") as Button;
            sceneFileGrid = new ButtonGrid(window.findWidget("ChooseScene/FileSelect") as ScrollView);
            sceneFileGrid.ItemActivated += new EventHandler(sceneFileGrid_ItemActivated);

            imageAtlas = new ImageAtlas("ChooseSceneDialog", new Size2(sceneFileGrid.ItemWidth, sceneFileGrid.ItemHeight), new Size2(512, 512));

            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            findSceneFiles();
        }

        public override void Dispose()
        {
            base.Dispose();
            imageAtlas.Dispose();
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
                String pictureFileName = sceneDirectory + "/" + baseName + ".png";
                String imageKey = null;
                if (archive.exists(pictureFileName))
                {
                    using (Stream imageStream = archive.openStream(pictureFileName, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                    {
                        Image image = Image.FromStream(imageStream);
                        if (image != null)
                        {
                            imageKey = imageAtlas.addImage(pictureFileName, image);
                        }
                    }
                }
                ButtonGridItem item = sceneFileGrid.addItem("Main", baseName, imageKey);
                item.UserObject = file;
            }
            if (sceneFileGrid.Count > 0)
            {
                sceneFileGrid.SelectedItem = sceneFileGrid.getItem(0);
            }
        }

        void sceneFileGrid_ItemActivated(object sender, EventArgs e)
        {
            changeScene();
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            changeScene();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void changeScene()
        {
            if (sceneFileGrid.SelectedItem != null)
            {
                piperGUI.changeActiveFile(null);
                controller.openNewScene(sceneFileGrid.SelectedItem.UserObject.ToString());
            }
            this.close();
        }
    }
}
