using Anomalous.Medical.StoreManager.Controller;
using Medical;
using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager
{
    class StoreManagerPlugin : AtlasPlugin
    {
        public StoreManagerPlugin()
        {

        }

        public void Dispose()
        {
            
        }

        public void loadGUIResources()
        {
            //ResourceManager.Instance.load("Lecture.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            standaloneController.SharePluginController = new SharePluginController((source, tool) =>
                {
                    UploadPluginController uploadPlugin = new UploadPluginController(standaloneController, new DDAtlasPlugin());
                    uploadPlugin.showContext(source, tool);
                })
                {
                    Name = "Share",
                    IconName = CommonResources.NoIcon,
                    Category = "Store Manager"
                };
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
                return 31;
            }
        }

        public string PluginName
        {
            get
            {
                return "Store Manager";
            }
        }

        public string BrandingImageKey
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
    }
}
