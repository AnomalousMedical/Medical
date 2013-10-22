using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SlidePanel : Saveable
    {
        private int size = 50;
        private ViewSizeStrategy sizeStrategy = ViewSizeStrategy.Auto;
        private ViewLocations viewLocation = ViewLocations.Left;

        public SlidePanel()
        {

        }

        public String createViewName(String masterName)
        {
            return masterName + ViewLocation.ToString();
        }

        protected internal virtual void claimFiles(CleanupFileInfo info, ResourceProvider resourceProvider, Slide slide)
        {

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

        protected SlidePanel(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
        }
    }
}
