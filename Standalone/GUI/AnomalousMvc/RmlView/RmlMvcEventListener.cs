using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class RmlMvcEventListener : EventListener
    {
        private AnomalousMvcContext mvcContext;
        private String name;
        private ViewHost viewHost;

        public RmlMvcEventListener(String name, AnomalousMvcContext mvcContext, ViewHost viewHost)
        {
            this.name = name;
            this.mvcContext = mvcContext;
            this.viewHost = viewHost;
        }

        public override void ProcessEvent(Event evt)
        {
            try
            {
                mvcContext.runAction(name, viewHost);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("An error occured running event '{0}'\n{1}", name, ex.Message), "An error has occured", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }
    }
}
