using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Medical.GUI;
using Engine.ObjectManagement;

namespace LeapMotionPlugin
{
    class LeapAtlasPlugin : AtlasPlugin
    {

        public LeapAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {

        }

        public void loadGUIResources()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneRevealed()
        {

        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {

        }

        public long PluginId
        {
            get
            {
                return -1;
            }
        }

        public String PluginName
        {
            get
            {
                return "Leap Motion Plugin";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return CommonResources.NoIcon;
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
