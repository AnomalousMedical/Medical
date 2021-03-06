﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using OgrePlugin;
using FreeImageAPI;
using Anomalous.GuiFramework;

namespace Medical.GUI
{
    public class ChooseSceneDialog : AbstractFullscreenGUIPopup
    {
        public event EventHandler ChooseScene;

        private SingleSelectButtonGrid sceneFileGrid;
        private ImageAtlas imageAtlas;
        private bool allowSceneChanges = false;
        private Widget loadingWidget;

        public ChooseSceneDialog(GUIManager guiManager)
            : base("Medical.GUI.ChooseSceneDialog.ChooseSceneDialog.layout", guiManager)
        {
            Button cancelButton = widget.findWidget("ChooseScene/Cancel") as Button;
            sceneFileGrid = new SingleSelectButtonGrid(widget.findWidget("ChooseScene/FileSelect") as ScrollView);
            sceneFileGrid.HighlightSelectedButton = false;

            imageAtlas = new ImageAtlas("ChooseSceneDialog", new IntSize2(sceneFileGrid.ItemWidth, sceneFileGrid.ItemHeight));

            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            findSceneFiles();

            loadingWidget = widget.findWidget("Loading");
            loadingWidget.Visible = false;

            this.Hiding += ChooseSceneDialog_Hiding;
            this.Showing += new EventHandler(ChooseSceneDialog_Showing);
        }

        public override void Dispose()
        {
            base.Dispose();
            imageAtlas.Dispose();
        }

        protected override void layoutUpdated()
        {
            sceneFileGrid.resizeAndLayout(sceneFileGrid.Width);
        }

        public String SelectedFile
        {
            get
            {
                if (sceneFileGrid.SelectedItem != null)
                {
                    return sceneFileGrid.SelectedItem.UserObject.ToString();
                }
                return null;
            }
        }

        void findSceneFiles()
        {
            try
            {
                VirtualFileSystem archive = VirtualFileSystem.Instance;
                String sceneDirectory = MedicalConfig.SceneDirectory;
                IEnumerable<String> files = archive.listFiles(sceneDirectory, "*.sim.xml", false);
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
                            using (var image = new FreeImageBitmap(imageStream))
                            {
                                if (image != null)
                                {
                                    imageKey = imageAtlas.addImage(pictureFileName, image);
                                }
                            }
                        }
                    }
                    ButtonGridItem item = sceneFileGrid.addItem("Main", baseName, imageKey);
                    item.ItemClicked += new EventHandler(item_ItemClicked);
                    item.UserObject = file;
                }
                if (sceneFileGrid.Count > 0)
                {
                    sceneFileGrid.SelectedItem = sceneFileGrid.getItem(0);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} loading scene files for ChooseScneDialog.\nMessage: {1}", ex.GetType().Name, ex.Message);
            }
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            changeScene();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void changeScene()
        {
            if (allowSceneChanges)
            {
                loadingWidget.Visible = true;
                OgreInterface.Instance.OgrePrimaryWindow.OgreRenderTarget.update();
                if (sceneFileGrid.SelectedItem != null)
                {
                    if (ChooseScene != null)
                    {
                        ChooseScene.Invoke(this, EventArgs.Empty);
                    }
                }
                this.hide();
            }
        }

        void ChooseSceneDialog_Showing(object sender, EventArgs e)
        {
            allowSceneChanges = true;
            loadingWidget.Visible = false;
        }

        void ChooseSceneDialog_Hiding(object sender, CancelEventArgs e)
        {
            allowSceneChanges = false;
        }
    }
}
