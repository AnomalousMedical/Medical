using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RocketGui
{
    public class RocketDebuggerWindow : MDIDialog
    {
        private RocketWidget rocketWidget;

        public RocketDebuggerWindow()
            :base("Medical.GUI.RocketGui.RocketDebuggerWindow.layout")
        {
            rocketWidget = new RocketWidget((ImageBox)window.findWidget("RocketImage"));
            Debugger.Initialise(rocketWidget.Context);
            Debugger.SetVisible(true);
            this.Resized += RocketDebuggerWindow_Resized;
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        void RocketDebuggerWindow_Resized(object sender, EventArgs e)
        {
            rocketWidget.resized();
        }
    }
}
