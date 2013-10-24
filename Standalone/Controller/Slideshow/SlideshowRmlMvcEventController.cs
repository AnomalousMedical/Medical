using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using libRocketPlugin;
using Medical.GUI;

namespace Medical
{
    class SlideshowRmlMvcEventController : RocketEventController
    {
        private AnomalousMvcContext mvcContext;
        private ViewHost viewHost;
        private Slide slide;

        public SlideshowRmlMvcEventController(AnomalousMvcContext mvcContext, ViewHost viewHost, Slide slide)
        {
            this.mvcContext = mvcContext;
            this.viewHost = viewHost;
            this.slide = slide;
        }

        public EventListener createEventListener(string name)
        {
            return new SlideshowRmlMvcEventListener(slide, name, mvcContext, viewHost);
        }
    }
}
