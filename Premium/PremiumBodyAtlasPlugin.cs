using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using OgrePlugin;
using Medical.GUI;
using Engine;
using Anomalous.GuiFramework;

namespace Medical
{
    public class PremiumBodyAtlasPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;
        private LicenseManager licenseManager;
        
        //Dialogs
        private CloneWindowDialog cloneWindowDialog;

        //Tasks
        private ChangeWindowLayoutTask windowLayout;

        public PremiumBodyAtlasPlugin(StandaloneController standaloneController)
        {
            this.AllowUninstall = true;
            this.licenseManager = standaloneController.LicenseManager;
        }

        public void Dispose()
        {
            cloneWindowDialog.Dispose();
            windowLayout.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Medical.Resources.PremiumImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.guiManager = standaloneController.GUIManager;
            this.standaloneController = standaloneController;

            standaloneController.AnatomyController.ShowPremiumAnatomy = true;

            //Dialogs
            cloneWindowDialog = new CloneWindowDialog();

            //Tasks
            windowLayout = new ChangeWindowLayoutTask(standaloneController.SceneViewController);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new ChangeBackgroundColorTask(standaloneController.SceneViewController));
            if (PlatformConfig.AllowCloneWindows)
            {
                standaloneController.TaskController.addTask(new CloneWindowTask(standaloneController, cloneWindowDialog));
            }
            taskController.addTask(windowLayout);
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {

        }

        public void sceneLoaded(SimScene scene)
        {
            windowLayout.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public long PluginId
        {
            get
            {
                return 1;
            }
        }

        public String PluginName
        {
            get
            {
                return "Premium Features";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "PremiumFeatures/BrandingImage";
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

        public bool AllowUninstall { get; set; }

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
