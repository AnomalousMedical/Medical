using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using libRocketPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class RmlMvcEventController : RocketEventController
    {
        private AnomalousMvcContext mvcContext;

        public RmlMvcEventController(AnomalousMvcContext mvcContext)
        {
            this.mvcContext = mvcContext;
        }

        public EventListener createEventListener(string name)
        {
            return new RmlMvcEventListener(name, mvcContext);
        }
    }
}
