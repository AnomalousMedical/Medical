using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    /// <summary>
    /// Does what the base class does, but adds the slide name to the beginning of the event name address.
    /// </summary>
    class SlideshowRmlMvcEventListener : RmlMvcEventListener
    {
        public SlideshowRmlMvcEventListener(Slide slide, String name, AnomalousMvcContext mvcContext, ViewHost viewHost)
            :base(String.Format("{0}/{1}", slide.UniqueName, name), mvcContext, viewHost)
        {
            
        }
    }
}
