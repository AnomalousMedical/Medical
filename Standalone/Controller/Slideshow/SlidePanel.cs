using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class SlidePanel : Saveable
    {
        private int size = 50;
        private ViewLocations viewLocation = ViewLocations.Left;

        public SlidePanel()
        {

        }

        public String createViewName(String masterName)
        {
            return masterName + ViewLocation.ToString();
        }

        public abstract MyGUIView createView(Slide slide, String name);

        protected internal virtual void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide)
        {

        }

        public virtual bool applyToExisting(SlidePanel panel, bool overwriteContent)
        {
            panel.Size = this.Size;
            panel.ViewLocation = this.ViewLocation;
            return true;
        }

        public abstract SlidePanel clone();

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
