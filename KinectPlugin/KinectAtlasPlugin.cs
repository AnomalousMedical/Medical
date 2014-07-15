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
        private KinectSensor sensor;

        private KinectDebugVisualizer kinectDebugger;
        private KinectIkController ikController;

        public KinectAtlasPlugin()
        {
            
        }

        public void Dispose()
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        public void loadGUIResources()
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            ikController = new KinectIkController(standaloneController);
            kinectDebugger = new KinectDebugVisualizer(standaloneController);

            //Setup kinect
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (this.sensor != null)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new skeleton frame data
                this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (sensor == null)
            {
                Logging.Log.ImportantInfo("No Kinect Sensor found");
            }
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

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            //Get the skeletons
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

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
