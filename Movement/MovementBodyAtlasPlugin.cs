using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using System.Diagnostics;
using Medical.GUI;
using Engine;

namespace Medical
{
    public class MovementBodyAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        public MovementBodyAtlasPlugin()
        {
            
        }

        public void Dispose()
        {
            
        }

        public void loadGUIResources()
        {
            //ResourceManager.Instance.load("Medical.Resources.PremiumImagesets.xml");
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

        public String PluginName
        {
            get
            {
                return "Movement Simulation";
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

        public bool AllowUninstall
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return IEnumerableUtil<long>.EmptyIterator;
            }
        }
    }
}
