using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Engine.Saving;
using Medical;
using Medical.Controller;
using Microsoft.Kinect;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectAtlasPlugin : AtlasPlugin
    {
        private KinectDebugVisualizer kinectDebugger;
        private KinectIkController ikController;
        private KinectSensorManager sensorManager;

        private KinectGui kinectGui;

        public KinectAtlasPlugin()
        {
            
        }

        public void Dispose()
        {
            kinectGui.Dispose();
            if(sensorManager != null)
            {
                sensorManager.Dispose();
            }
        }

        public void loadGUIResources()
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            ikController = new KinectIkController(standaloneController);
            kinectDebugger = new KinectDebugVisualizer(standaloneController);
            sensorManager = new KinectSensorManager();
            sensorManager.SkeletonFrameReady += sensorManager_SkeletonFrameReady;

            kinectGui = new KinectGui(ikController, sensorManager, kinectDebugger);
            standaloneController.GUIManager.addManagedDialog(kinectGui);

            var taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(kinectGui, "KinectPlugin.KinectGui", "Kinect", CommonResources.NoIcon, "Kinect"));
        }

        public void sceneLoaded(SimScene scene)
        {
            ikController.createIkControls(scene);
            kinectDebugger.createDebugObjects(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            ikController.destroyIkControls(scene);
            kinectDebugger.destroyDebugObjects(scene);
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
                return "Kinect Plugin";
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

        void sensorManager_SkeletonFrameReady(Skeleton[] skeletons)
        {
            if (skeletons.Length != 0)
            {
                foreach (Skeleton skel in skeletons)
                {
                    ikController.updateControls(skel);
                    kinectDebugger.debugSkeleton(skel);
                }
            }
        }
    }
}
