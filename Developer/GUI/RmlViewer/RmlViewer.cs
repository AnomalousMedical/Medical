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

namespace Developer.GUI
{
    class RmlViewer : MDIDialog
    {
        private RocketWidget rocketWidget;
        private String documentName = "Developer.GUI.TestRocketWindow.demo.rml";
        private FileSystemWatcher fileWatcher;

        public RmlViewer()
            : base("Developer.GUI.RmlViewer.RmlViewer.layout")
        {
            ImageBox imageBox = (ImageBox)window.findWidget("RocketImage");
            rocketWidget = new RocketWidget("Developer.GUI.RmlViewer", documentName, imageBox);
            rocketWidget.Enabled = false;

            Button reload = (Button)window.findWidget("Reload");
            reload.MouseButtonClick += new MyGUIEvent(reload_MouseButtonClick);

            this.Resized += new EventHandler(TestRocketWindow_Resized);

            try
            {
                if (VirtualFileSystem.Instance.exists(documentName))
                {
                    VirtualFileInfo fileInfo = VirtualFileSystem.Instance.getFileInfo(documentName);
                    if (File.Exists(fileInfo.RealLocation))
                    {
                        fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(fileInfo.RealLocation));
                        fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                        fileWatcher.EnableRaisingEvents = true;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public override void Dispose()
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }
            rocketWidget.Dispose();
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

        void reload_MouseButtonClick(Widget source, EventArgs e)
        {
            Factory.ClearStyleSheetCache();
            rocketWidget.changeDocument(documentName);
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ThreadManager.invoke(new Action(delegate()
            {
                Factory.ClearStyleSheetCache();
                rocketWidget.changeDocument(documentName);
            }));
        }
    }
}
