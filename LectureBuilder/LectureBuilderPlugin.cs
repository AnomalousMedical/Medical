using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using MyGUIPlugin;

namespace LectureBuilder
{
    class LectureBuilderPlugin : AtlasPlugin
    {
        public LectureBuilderPlugin()
        {

        }

        public void Dispose()
        {
            
        }

        public void loadGUIResources()
        {
            Gui.Instance.load("LectureBuilder.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void sceneRevealed()
        {
            
        }

        public long PluginId
        {
            get
            {
                return 10;
            }
        }

        public string PluginName
        {
            get
            {
                return "Lecture Builder";
            }
        }

        public string BrandingImageKey
        {
            get
            {
                return "LectureBuilder/BrandingImage";
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
