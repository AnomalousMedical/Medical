using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using Engine;
using libRocketPlugin;
using Medical.Controller;
using Logging;
using Medical;
using OgreWrapper;

namespace Developer.GUI
{
    class RmlViewer : MDIDialog
    {
        private const String RmlViewerOgreGroup = "RmlViewer";

        private RocketWidget rocketWidget;
        private String documentName = null;
        private FileSystemWatcher fileWatcher;
        private String windowTitleBase;
        private bool loadedOnce = false;

        public RmlViewer()
            : base("Developer.GUI.RmlViewer.RmlViewer.layout")
        {
            ImageBox imageBox = (ImageBox)window.findWidget("RocketImage");
            rocketWidget = new RocketWidget("Developer.GUI.RmlViewer", imageBox);
            rocketWidget.Enabled = false;

            MenuBar menuBar = (MenuBar)window.findWidget("MenuBar");
            MenuItem file = menuBar.addItem("File", MenuItemType.Popup, "File");
            MenuControl fileControl = menuBar.createItemPopupMenuChild(file);
            fileControl.addItem("Open", MenuItemType.Normal, "Open");
            fileControl.ItemAccept += new MyGUIEvent(fileControl_ItemAccept);

            this.Resized += new EventHandler(TestRocketWindow_Resized);

            windowTitleBase = window.Caption;
        }

        void fileControl_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            switch (mcae.Item.ItemId)
            {
                case "Open":
                    using (FileOpenDialog fileOpen = new FileOpenDialog(MainWindow.Instance, "Choose RML File"))
                    {
                        if (fileOpen.showModal() == NativeDialogResult.OK)
                        {
                            documentName = fileOpen.Path;
                            loadDocument();
                        }
                    }
                    break;
            }
        }

        public override void Dispose()
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }
            rocketWidget.Dispose();
            if (loadedOnce)
            {
                TextureDatabase.ReleaseTextures();
                OgreResourceGroupManager.getInstance().removeResourceLocation("__RmlViewerFilesystem__", RmlViewerOgreGroup);
                OgreResourceGroupManager.getInstance().destroyResourceGroup(RmlViewerOgreGroup);
            }
            base.Dispose();
        }

        protected override void onClosed(EventArgs args)
        {
            rocketWidget.Enabled = false;
            base.onClosed(args);
        }

        protected override void onShown(EventArgs args)
        {
            rocketWidget.Enabled = true;
            base.onShown(args);
        }

        void TestRocketWindow_Resized(object sender, EventArgs e)
        {
            rocketWidget.resized();
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ThreadManager.invoke(new Action(delegate()
            {
                loadDocument();
            }));
        }

        private void loadDocument()
        {
            try
            {
                if (loadedOnce)
                {
                    //The textures are unloaded and released so they will refresh if they are updated.
                    TextureDatabase.ReleaseTextures();
                    OgreResourceGroupManager.getInstance().removeResourceLocation("__RmlViewerFilesystem__", RmlViewerOgreGroup);
                    OgreResourceGroupManager.getInstance().destroyResourceGroup(RmlViewerOgreGroup);
                }
                loadedOnce = true;
                OgreResourceGroupManager.getInstance().addResourceLocation("__RmlViewerFilesystem__", RocketRawOgreFilesystemArchive.ArchiveName, RmlViewerOgreGroup, false);

                window.Caption = String.Format("{0} - '{1}'  {2}", windowTitleBase, Path.GetFileName(documentName), Path.GetDirectoryName(documentName));

                Factory.ClearStyleSheetCache();
                rocketWidget.Context.UnloadAllDocuments();
                using (ElementDocument document = rocketWidget.Context.LoadDocument(documentName))
                {
                    if (document != null)
                    {
                        document.Show();
                    }
                }

                if (fileWatcher != null)
                {
                    fileWatcher.Dispose();
                    fileWatcher = null;
                }

                String realLocation = documentName;

                if (VirtualFileSystem.Instance.exists(realLocation))
                {
                    VirtualFileInfo fileInfo = VirtualFileSystem.Instance.getFileInfo(documentName);
                    realLocation = fileInfo.RealLocation;
                }
                else
                {

                }
                if (File.Exists(realLocation))
                {
                    fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(realLocation));
                    fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                    fileWatcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Could not load file watcher for {0} because {1}", documentName, ex.Message);
            }
        }
    }
}
