﻿using Anomalous.Medical.StoreManager.Controller;
using Engine;
using Medical;
using Medical.GUI;
using MyGUIPlugin;
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
            ResourceManager.Instance.load("Anomalous.Medical.StoreManager.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            standaloneController.SharePluginController = new SharePluginController((source, tool) =>
                {
                    UploadPluginController uploadPlugin = new UploadPluginController(standaloneController);
                    uploadPlugin.showContext(source, tool);
                })
                {
                    Name = "Share",
                    IconName = "StoreManager/Icon",
                    Category = "Store Manager"
                };
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {

        }

        public void sceneLoaded(Engine.ObjectManagement.SimScene scene)
        {
            
        }

        public void sceneUnloading(Engine.ObjectManagement.SimScene scene)
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
                return "StoreManager/BrandingImage";
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

        public bool AllowRuntimeUninstall
        {
            get
            {
                return false;
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
