using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Medical
{
    public class RmlSlidePanel : Saveable
    {
        private String rml;
        private int size = 50;
        private ViewSizeStrategy sizeStrategy = ViewSizeStrategy.Auto;
        private ViewLocations viewLocation = ViewLocations.Left;

        public RmlSlidePanel()
        {

        }

        public String createViewName(String masterName)
        {
            return masterName + ViewLocation.ToString();
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

        [Editable]
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        [Editable]
        public ViewSizeStrategy SizeStrategy
        {
            get
            {
                return sizeStrategy;
            }
            set
            {
                sizeStrategy = value;
            }
        }

        [Editable]
        public ViewLocations ViewLocation
        {
            get
            {
                return viewLocation;
            }
            set
            {
                viewLocation = value;
            }
        }

        protected RmlSlidePanel(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }

        internal void claimFiles(CleanupFileInfo info, ResourceProvider resourceProvider, RmlSlide slide)
        {
            XDocument rmlDoc = XDocument.Parse(rml);
            var images = from query in rmlDoc.Descendants("img")
                         where query.Attribute("src") != null
                         select query.Attribute("src").Value;

            foreach (String image in images)
            {
                info.claimFile(Path.Combine(slide.UniqueName, image));
            }
        }
    }
}
