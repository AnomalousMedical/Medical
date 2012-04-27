using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Developer.GUI
{
    class TestRocketWindow : MDIDialog
    {
        private RocketWidget rocketWidget;

        public TestRocketWindow(String name)
            :base("Developer.GUI.TestRocketWindow.TestRocketWindow.layout")
        {
            ImageBox imageBox = (ImageBox)window.findWidget("RocketImage");
            rocketWidget = new RocketWidget(name, imageBox);

            this.Resized += new EventHandler(TestRocketWindow_Resized);
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        public override void deserialize(Engine.ConfigFile configFile)
        {
            base.deserialize(configFile);
            //rocketWidget.resized();
        }

        void TestRocketWindow_Resized(object sender, EventArgs e)
        {
            rocketWidget.resized();
        }
    }
}
