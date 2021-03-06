﻿using Engine;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
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

        public String Name { get; set; }

        public String IconName { get; set; }

        public void copyLayoutToSlide(Slide slide, EditorResourceProvider resourceProvider, bool overwriteContent)
        {
            slide.LayoutStrategy = LayoutStrategy.createDerivedStrategy(slide, this, resourceProvider, overwriteContent, false);
        }
    }
}
