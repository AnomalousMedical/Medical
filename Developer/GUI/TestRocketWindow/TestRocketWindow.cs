using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using libRocketPlugin;
using System.IO;
using Engine;
using Medical.Controller;

namespace Developer.GUI
{
    class TestRocketWindow : MDIDialog
    {
        private RocketWidget rocketWidget;
        private String documentName = "demo.rml";
        private FileSystemWatcher fileWatcher;

        public TestRocketWindow(String name)
            :base("Developer.GUI.TestRocketWindow.TestRocketWindow.layout")
        {
            ImageBox imageBox = (ImageBox)window.findWidget("RocketImage");
            rocketWidget = new RocketWidget(name, documentName, imageBox);
            rocketWidget.Enabled = false;

            Button reload = (Button)window.findWidget("Reload");
            reload.MouseButtonClick += new MyGUIEvent(reload_MouseButtonClick);

            this.Resized += new EventHandler(TestRocketWindow_Resized);

            VirtualFileInfo fileInfo = VirtualFileSystem.Instance.getFileInfo(documentName);
            if(File.Exists(fileInfo.RealLocation))
            {
                fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(fileInfo.RealLocation));
                fileWatcher.Changed += new FileSystemEventHandler(fileWatcher_Changed);
                fileWatcher.EnableRaisingEvents = true;
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
