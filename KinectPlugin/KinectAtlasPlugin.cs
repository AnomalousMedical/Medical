using Engine;
using Engine.ObjectManagement;
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

        private MedicalController medicalController;
        private Dictionary<JointType, SimObjectBase> testSimObjs = new Dictionary<JointType, SimObjectBase>();

        private static Dictionary<JointType, Tuple<String, String>> ikJointMap = new Dictionary<JointType, Tuple<String, String>>();

        private static Dictionary<JointType, String> simObjectMap = new Dictionary<JointType, String>();

        static KinectAtlasPlugin()
        {
            simObjectMap.Add(JointType.HipCenter, "Pelvis");
            simObjectMap.Add(JointType.Spine, "SpineC7");
            simObjectMap.Add(JointType.ShoulderCenter, "Manubrium");
            simObjectMap.Add(JointType.Head, "Skull");
            simObjectMap.Add(JointType.ShoulderLeft, "LeftScapula");
            simObjectMap.Add(JointType.ElbowLeft, "LeftHumerus");
            simObjectMap.Add(JointType.WristLeft, "LeftUlna");
            simObjectMap.Add(JointType.HandLeft, "LeftHandBase");
            simObjectMap.Add(JointType.ShoulderRight, "RightScapula");
            simObjectMap.Add(JointType.ElbowRight, "RightHumerus");
            simObjectMap.Add(JointType.WristRight, "RightUlna");
            simObjectMap.Add(JointType.HandRight, "RightHandBase");
            simObjectMap.Add(JointType.HipLeft, null);
            simObjectMap.Add(JointType.KneeLeft, "LeftFemur");
            simObjectMap.Add(JointType.AnkleLeft, "LeftTibia");
            simObjectMap.Add(JointType.FootLeft, "LeftFootBase");
            simObjectMap.Add(JointType.HipRight, null);
            simObjectMap.Add(JointType.KneeRight, "RightFemur");
            simObjectMap.Add(JointType.AnkleRight, "RightTibia");
            simObjectMap.Add(JointType.FootRight, "RightFootBase");

            ikJointMap.Add(JointType.HipCenter, Tuple.Create("Pelvis", "Pelvis"));
            ikJointMap.Add(JointType.Spine, Tuple.Create("SpineC7", "SpineC7"));
            ikJointMap.Add(JointType.ShoulderCenter, Tuple.Create("Manubrium", "Manubrium"));
            ikJointMap.Add(JointType.Head, Tuple.Create("Skull", "Skull"));
            ikJointMap.Add(JointType.ShoulderLeft, Tuple.Create("LeftScapula", "LeftScapula"));
            ikJointMap.Add(JointType.ElbowLeft, Tuple.Create("LeftHumerus", "LeftHumerusUlnaJoint"));
            ikJointMap.Add(JointType.WristLeft, Tuple.Create("LeftUlna", "LeftRadiusHandBaseJoint"));
            ikJointMap.Add(JointType.HandLeft, Tuple.Create("LeftHandBase", "LeftHandBase"));
            ikJointMap.Add(JointType.ShoulderRight, Tuple.Create("RightScapula", "RightScapula"));
            ikJointMap.Add(JointType.ElbowRight, Tuple.Create("RightHumerus", "RightHumerusUlnaJoint"));
            ikJointMap.Add(JointType.WristRight, Tuple.Create("RightUlna", "RightRadiusHandBaseJoint"));
            ikJointMap.Add(JointType.HandRight, Tuple.Create("RightHandBase", "RightHandBase"));
            ikJointMap.Add(JointType.HipLeft, Tuple.Create("Pelvis", "LeftFemurPelvisJoint"));
            ikJointMap.Add(JointType.KneeLeft, Tuple.Create("LeftFemur", "LeftFemurTibiaJoint"));
            ikJointMap.Add(JointType.AnkleLeft, Tuple.Create("LeftTibia", "LeftTibiaFootBaseJoint"));
            ikJointMap.Add(JointType.FootLeft, Tuple.Create("LeftFootBase", "LeftFootBase"));
            ikJointMap.Add(JointType.HipRight, Tuple.Create("Pelvis", "RightFemurPelvisJoint"));
            ikJointMap.Add(JointType.KneeRight, Tuple.Create("RightFemur", "RightFemurTibiaJoint"));
            ikJointMap.Add(JointType.AnkleRight, Tuple.Create("RightTibia", "RightTibiaFootBaseJoint"));
            ikJointMap.Add(JointType.FootRight, Tuple.Create("RightFootBase", "RightFootBase"));
        }

        public KinectAtlasPlugin()
        {
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
            
            if(sensor == null)
            {
                Logging.Log.ImportantInfo("No Kinect Sensor found");
            }
        }

        public void Dispose()
        {
            if(sensor != null)
            {
                sensor.Stop();
            }
        }

        public void loadGUIResources()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            this.medicalController = standaloneController.MedicalController;
        }

        public void sceneLoaded(SimScene scene)
        {
            TrackArrows(scene);
            //TrackSimObjects(scene);
        }

        private void TrackSimObjects(SimScene scene)
        {
            var subScene = scene.getDefaultSubScene();

            foreach (var enumVal in EnumUtil.Elements(typeof(JointType)))
            {
                JointType jointType = (JointType)Enum.Parse(typeof(JointType), enumVal);
                testSimObjs.Add(jointType, (GenericSimObject)medicalController.getSimObject(simObjectMap[jointType]));
            }
        }

        private void TrackArrows(SimScene scene)
        {
            GenericSimObjectDefinition testArrow = new GenericSimObjectDefinition("TestArrow");
            SceneNodeDefinition node = new SceneNodeDefinition("Node");
            EntityDefinition entityDef = new EntityDefinition("Entity");
            entityDef.MeshName = "Arrow.mesh";
            node.addMovableObjectDefinition(entityDef);
            testArrow.addElement(node);

            var subScene = scene.getDefaultSubScene();

            foreach (var enumVal in EnumUtil.Elements(typeof(JointType)))
            {
                JointType jointType = (JointType)Enum.Parse(typeof(JointType), enumVal);

                testArrow.Name = enumVal;
                testArrow.Translation = medicalController.getSimObject(ikJointMap[jointType].Item2).Translation;
                SimObjectBase instance = testArrow.register(subScene);
                medicalController.addSimObject(instance);
                scene.buildScene();

                SceneNodeElement sceneNode = instance.getElement("Node") as SceneNodeElement;
                var entity = sceneNode.getNodeObject("Entity") as OgreWrapper.Entity;
                var subEntity = entity.getSubEntity(0);
                subEntity.setCustomParameter(1, new Quaternion(1, 0, 0, 1));

                testSimObjs.Add(jointType, instance);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            testSimObjs.Clear();
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

        Vector3 convertPoint(Joint joint)
        {
            return new Vector3(joint.Position.X * 1000f * SimulationConfig.MMToUnits, joint.Position.Y * 1000f * SimulationConfig.MMToUnits - 85f, joint.Position.Z * 1000f * SimulationConfig.MMToUnits);
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if(testSimObjs.Count == 0)
            {
                return;
            }

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
                    if (skel.TrackingState != SkeletonTrackingState.NotTracked)
                    {
                        //Logging.Log.Debug("Tracking skeleton");
                        foreach(Joint joint in skel.Joints)
                        {
                            if(joint.TrackingState != JointTrackingState.NotTracked)
                            {
                                SimObjectBase simObject = testSimObjs[joint.JointType];
                                if (simObject != null)
                                {
                                    Vector3 pos = convertPoint(joint);
                                    ThreadManager.invoke(() =>
                                        {
                                            simObject.updateTranslation(ref pos, null);
                                        });
                                    //Logging.Log.Debug("{0} {1}", joint.JointType, pos);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
