using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using libRocketPlugin;

namespace Developer.GUI
{
    class TestRocketWindow : MDIDialog
    {
        private RocketWidget rocketWidget;
        private String documentName = "assets/demo.rml";

        public TestRocketWindow(String name)
            :base("Developer.GUI.TestRocketWindow.TestRocketWindow.layout")
        {
            ImageBox imageBox = (ImageBox)window.findWidget("RocketImage");
            rocketWidget = new RocketWidget(name, documentName, imageBox);
            rocketWidget.Enabled = false;

            Button reload = (Button)window.findWidget("Reload");
            reload.MouseButtonClick += new MyGUIEvent(reload_MouseButtonClick);

            this.Resized += new EventHandler(TestRocketWindow_Resized);
        }

        public override void Dispose()
        {
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
    }
}
