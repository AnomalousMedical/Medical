using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Engine.Saving;
using Medical;
using Medical.Controller;
using Microsoft.Kinect;
using MyGUIPlugin;
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
        private KinectPoseController poseController;
        private KinectSensorManager sensorManager;

        private KinectGui kinectGui;
        private MovementSequenceRecorder sequenceRecorder;

        public KinectAtlasPlugin()
        {
            
        }

        public void Dispose()
        {
            kinectGui.Dispose();
            if(sensorManager != null)
            {
                sensorManager.Dispose();
                sensorManager = null;
            }
            if(sequenceRecorder != null)
            {
                sequenceRecorder.Dispose();
                sequenceRecorder = null;
            }
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("KinectPlugin.Resources.Imagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            poseController = new KinectIkController(standaloneController);
            kinectDebugger = new KinectDebugVisualizer(standaloneController);
            sensorManager = new KinectSensorManager();
            sensorManager.SkeletonFrameReady += sensorManager_SkeletonFrameReady;
            sensorManager.StatusChanged += sensorManager_StatusChanged;

            kinectGui = new KinectGui(poseController, sensorManager, kinectDebugger);
            standaloneController.GUIManager.addManagedDialog(kinectGui);

            sequenceRecorder = new MovementSequenceRecorder(standaloneController.MedicalController, standaloneController.MovementSequenceController);
            standaloneController.GUIManager.addManagedDialog(sequenceRecorder);
            
            var taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(kinectGui, "KinectPlugin.KinectGui", "Kinect", "KinectPlugin.Kinect", "Kinect"));
            taskController.addTask(new MDIDialogOpenTask(sequenceRecorder, "KinectPlugin.KinectMovementSequenceRecorder", "Record Sequence", "KinectPlugin.Record", "Kinect"));
        }

        void sensorManager_StatusChanged(KinectSensorManager obj)
        {
            //This function checks the status the first time the sensor is connected.
            if (sensorManager.Connected)
            {
                poseController.AllowMovement = true;
                sensorManager.StatusChanged -= sensorManager_StatusChanged; //Remove the event, we only care the first time
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            poseController.createIkControls(scene);
            kinectDebugger.createDebugObjects(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            poseController.destroyIkControls(scene);
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
                return "KinectPlugin.BrandingImage";
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

        void sensorManager_SkeletonFrameReady(Body[] skeletons)
        {
            if (skeletons.Length != 0)
            {
                foreach (Body skel in skeletons)
                {
                    poseController.updateControls(skel);
                    kinectDebugger.debugSkeleton(skel);
                }
            }
        }
    }
}
