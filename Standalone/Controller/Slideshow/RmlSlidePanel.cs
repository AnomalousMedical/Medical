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
        private String rmlFile;

        public RmlSlidePanel()
        {
            RmlFile = Guid.NewGuid().ToString("D") + ".rml";
        }

        public override MyGUIView createView(Slide slide, String name)
        {
            return new RmlView(createViewName(name))
            {
                RmlFile = Path.Combine(slide.UniqueName, RmlFile),
                ElementName = ElementName,
                CreateCustomEventController = (context, viewHost) =>
                    {
                        return new SlideshowRmlMvcEventController(context, viewHost, slide);
                    }
            };
        }

        [Editable]
        public String RmlFile
        {
            get
            {
                return rmlFile;
            }
            set
            {
                rmlFile = value;
            }
        }

        protected internal override void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide)
        {
            String rml = null;
            String rmlFullPath = Path.Combine(slide.UniqueName, RmlFile);
            if (resourceProvider.fileExists(rmlFullPath))
            {
                using (StreamReader stringReader = new StreamReader(resourceProvider.openFile(rmlFullPath)))
                {
                    rml = stringReader.ReadToEnd();
                }
            }

            if (String.IsNullOrEmpty(rml))
            {
                Logging.Log.Warning("Could not claim files for slide '{0}', cannot find rml file '{1}' for panel '{2}'", slide.UniqueName, RmlFile, ElementName);
                return; //Break if we cannot load the rml
            }

            info.claimFile(rmlFullPath);

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

        protected internal override void updateToVersion(int fromVersion, int toVersion, Slide slide, ResourceProvider slideshowResources)
        {
            if (toVersion >= 2 && fromVersion < toVersion)
            {
                String rml;
                if (InlineRmlUpgradeCache.tryGetValue(this, out rml))
                {
                    rml = rml.Replace(Version1TemplateLink, Version2TemplateLinkReplacement);
                    InlineRmlUpgradeCache.setRml(this, rml);
                }
                using (Stream stream = slideshowResources.openWriteStream(Path.Combine(slide.UniqueName, Slide.StyleSheetName))) { }
            }
            if (toVersion >= 3 && fromVersion < toVersion)
            {
                String rml;
                if (InlineRmlUpgradeCache.tryGetValue(this, out rml))
                {
                    using (StreamWriter writer = new StreamWriter(slideshowResources.openWriteStream(Path.Combine(slide.UniqueName, RmlFile))))
                    {
                        writer.Write(rml);
                    }
                }
            }
        }

        public override bool applyToExisting(SlidePanel panel, bool overwriteContent)
        {
            if (panel is RmlSlidePanel)
            {
                if (overwriteContent)
                {
                    ((RmlSlidePanel)panel).RmlFile = this.RmlFile;
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

        protected RmlSlidePanel(LoadInfo info)
            : base(info)
        {
            if (info.Version < 2)
            {
                InlineRmlUpgradeCache.setRml(this, info.GetString("rml", null));
                RmlFile = Guid.NewGuid().ToString("D") + ".rml";
            }
        }

        private const String Version1TemplateLink = @"<link type=""text/template"" href=""/MasterTemplate.trml"" />";
        private const String Version2TemplateLinkReplacement = Version1TemplateLink + @"
        <link type=""text/rcss"" href=""SlideStyle.rcss""/>";
    }
}
