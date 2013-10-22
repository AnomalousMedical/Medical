using Engine;
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

        public MyGUIView createView(Slide slide, String name)
        {
            return new RawRmlView(createViewName(name))
            {
                Rml = Rml,
                FakePath = slide.UniqueName + "/index.rml",
                WidthSizeStrategy = SizeStrategy,
                HeightSizeStrategy = SizeStrategy,
                Size = new IntSize2(Size, Size),
                ViewLocation = ViewLocation
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

        protected internal override void claimFiles(CleanupFileInfo info, ResourceProvider resourceProvider, Slide slide)
        {
            XDocument rmlDoc = XDocument.Parse(rml);
            var images = from query in rmlDoc.Descendants("img")
                         where query.Attribute("src") != null
                         select query.Attribute("src").Value;

            foreach (String image in images)
            {
                info.claimFile(Path.Combine(slide.UniqueName, image));
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
