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
        private LayoutElementName elementName;

        public SlidePanel()
        {
            elementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left);
        }

        public String createViewName(String masterName)
        {
            return masterName + elementName.Name + elementName.LocationHint;
        }

        public abstract MyGUIView createView(Slide slide, String name);

        protected internal virtual void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide)
        {

        }

        public virtual bool applyToExisting(Slide slide, SlidePanel panel, bool overwriteContent, EditorResourceProvider resourceProvider)
        {
            panel.Size = this.Size;
            panel.ElementName = this.ElementName;
            return true;
        }

        protected internal virtual void updateToVersion(int fromVersion, int toVersion, Slide slide, ResourceProvider slideshowResources)
        {
            
        }

        public abstract SlidePanel clone(Slide cloneOwnerSlide, Slide destinationSlide, bool asTemplate, EditorResourceProvider resourceProvider);

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

        public LayoutElementName ElementName
        {
            get
            {
                return elementName;
            }
            set
            {
                elementName = value;
            }
        }

        protected const int CurrentVersion = 2;

        protected SlidePanel(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
            if (info.Version < CurrentVersion)
            {
                if (info.Version < 1)
                {
                    BorderLayoutLocations location = BorderLayoutLocations.Left;
                    switch (info.GetValue("viewLocation", ViewLocations.Left))
                    {
                        case ViewLocations.Left:
                            location = BorderLayoutLocations.Left;
                            break;
                        case ViewLocations.Right:
                            location = BorderLayoutLocations.Right;
                            break;
                        case ViewLocations.Top:
                            location = BorderLayoutLocations.Top;
                            break;
                        case ViewLocations.Bottom:
                            location = BorderLayoutLocations.Bottom;
                            break;
                    }
                    ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, location);
                }
            }
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
            info.Version = CurrentVersion;
        }
    }
}
