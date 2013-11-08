﻿using Engine;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Medical
{
    public class RmlSlidePanel : SlidePanel
    {
        private String rml;

        public RmlSlidePanel()
        {

        }

        public override MyGUIView createView(Slide slide, String name)
        {
            return new RawRmlView(createViewName(name))
            {
                Rml = Rml,
                FakePath = slide.UniqueName + "/index.rml",
                ViewLocation = ViewLocation,
                CreateCustomEventController = (context, viewHost) =>
                    {
                        return new SlideshowRmlMvcEventController(context, viewHost, slide);
                    }
            };
        }

        [Editable]
        public String Rml
        {
            get
            {
                return rml;
            }
            set
            {
                rml = value;
            }
        }

        protected RmlSlidePanel(LoadInfo info)
            :base(info)
        {
            
        }

        protected internal override void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide)
        {
            XDocument rmlDoc = XDocument.Parse(rml);
            var images = from query in rmlDoc.Descendants("img")
                         where query.Attribute("src") != null
                         select query.Attribute("src").Value;

            foreach (String image in images)
            {
                info.claimFile(Path.Combine(slide.UniqueName, image));
            }

            var triggers = from e in rmlDoc.Root.Descendants()
                           where e.Attribute("onclick") != null
                           select e;

            foreach (var element in triggers)
            {
                info.claimObject(Slide.SlideActionClass, element.Attribute("onclick").Value);
            }

            base.claimFiles(info, resourceProvider, slide);
        }

        public override bool applyToExisting(SlidePanel panel, bool overwriteContent)
        {
            if (panel is RmlSlidePanel)
            {
                if (overwriteContent)
                {
                    ((RmlSlidePanel)panel).Rml = this.Rml;
                }
                return base.applyToExisting(panel, overwriteContent);
            }
            return false;
        }

        public override SlidePanel clone()
        {
            RmlSlidePanel clone = new RmlSlidePanel();
            applyToExisting(clone, true);
            return clone;
        }
    }
}
