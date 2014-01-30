using Engine;
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
        private List<Pair<RmlSlidePanel, String>> panelDefaultRml = new List<Pair<RmlSlidePanel, string>>();

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

        public void addPanelWithRml(RmlSlidePanel panel, String rml)
        {
            addPanel(panel);
            setRmlForPanel(panel, rml);
        }

        public void setRmlForPanel(RmlSlidePanel panel, String rml)
        {
            panelDefaultRml.Add(new Pair<RmlSlidePanel, string>(panel, rml));
        }

        public void writePanelRml(EditorResourceProvider resourceProvider, Slide slide, bool overwrite)
        {
            String rmlDestination;
            foreach (var rmlPair in panelDefaultRml)
            {
                rmlDestination = rmlPair.First.getRmlFilePath(slide);
                if (overwrite || !resourceProvider.fileExists(rmlDestination))
                {
                    resourceProvider.ResourceCache.add(new ResourceProviderTextCachedResource(rmlDestination, Encoding.UTF8, rmlPair.Second, resourceProvider));
                }
            }
        }

        public String Name { get; set; }

        public String IconName { get; set; }
    }
}
