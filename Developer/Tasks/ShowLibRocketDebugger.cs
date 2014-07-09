using Medical;
using Medical.GUI;
using Medical.GUI.RocketGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Developer
{
    class ShowLibRocketDebugger : Task, IDisposable
    {
        private RocketDebuggerWindow rocketDebugger;
        private GUIManager guiManager;

        public ShowLibRocketDebugger(GUIManager guiManager)
            :base("ShowLibRocketDebugger", "Show LibRocket Debugger", "Developer.libRocketDebugger", "Developer")
        {
            this.guiManager = guiManager;
        }

        public void Dispose()
        {
            if (rocketDebugger != null)
            {
                guiManager.removeManagedDialog(rocketDebugger);
                rocketDebugger.Dispose();
            }
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            if (rocketDebugger == null)
            {
                rocketDebugger = new RocketDebuggerWindow();
                guiManager.addManagedDialog(rocketDebugger);
            }
            rocketDebugger.Position = taskPositioner.findGoodWindowPosition(rocketDebugger.Width, rocketDebugger.Height);
            rocketDebugger.Visible = true;
            rocketDebugger.Closed += rocketDebugger_Closed;
        }

        public override bool Active
        {
            get
            {
                return rocketDebugger != null && rocketDebugger.Visible;
            }
        }

        void rocketDebugger_Closed(object sender, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
