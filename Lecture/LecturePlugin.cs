using Medical;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture
{
    class LecturePlugin : AtlasPlugin
    {
        public LecturePlugin()
        {

        }

        public void Dispose()
        {

        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Lecture.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            
        }

        public void sceneLoaded(Engine.ObjectManagement.SimScene scene)
        {
            
        }

        public void sceneUnloading(Engine.ObjectManagement.SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        public long PluginId
        {
            get
            {
                return -1;
            }
        }

        public string PluginName
        {
            get
            {
                return "Lecture Tools";
            }
        }

        public string BrandingImageKey
        {
            get
            {
                return "Lecture.BrandingImage";
            }
        }

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }
    }
}
