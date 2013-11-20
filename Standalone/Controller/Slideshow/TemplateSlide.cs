using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This is a slide that is meant to work as a template for other slides. It it not to be used for a slideshow directly.
    /// </summary>
    public class TemplateSlide : Slide
    {
        public TemplateSlide()
        {

        }

        public TemplateSlide(SlideLayoutStrategy layoutStrategy)
            :base(layoutStrategy)
        {

        }

        protected override void customizeController(MvcController controller, RunCommandsAction showCommand)
        {
            
        }

        public String Name { get; set; }

        public String IconName { get; set; }
    }
}
