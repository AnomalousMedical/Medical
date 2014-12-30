﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using libRocketPlugin;
using libRocketWidget;

namespace Medical.GUI.AnomalousMvc
{
    class RmlMvcEventController : RocketEventController
    {
        private AnomalousMvcContext mvcContext;
        private ViewHost viewHost;

        public RmlMvcEventController(AnomalousMvcContext mvcContext, ViewHost viewHost)
        {
            this.mvcContext = mvcContext;
            this.viewHost = viewHost;
        }

        public EventListener createEventListener(string name)
        {
            return new RmlMvcEventListener(name, mvcContext, viewHost);
        }
    }
}
