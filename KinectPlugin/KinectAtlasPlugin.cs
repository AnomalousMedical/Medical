using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
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

        private MedicalController medicalController;
        private Dictionary<JointType, SimObjectBase> testSimObjs = new Dictionary<JointType, SimObjectBase>();

        private static Dictionary<JointType, Tuple<String, String>> ikJointMap = new Dictionary<JointType, Tuple<String, String>>();
        private static HashSet<String> createDragControlsFor = new HashSet<string>();

        static KinectAtlasPlugin()
        {
            ikJointMap.Add(JointType.HipCenter, Tuple.Create("Pelvis", "Pelvis"));
            ikJointMap.Add(JointType.Spine, Tuple.Create("SpineC7", "SpineC7"));
            ikJointMap.Add(JointType.ShoulderCenter, Tuple.Create("Manubrium", "Manubrium"));
            ikJointMap.Add(JointType.Head, Tuple.Create("Skull", "Skull"));
            ikJointMap.Add(JointType.ShoulderLeft, Tuple.Create("RightScapula", "RightScapula"));
            ikJointMap.Add(JointType.ElbowLeft, Tuple.Create("RightHumerus", "RightHumerusUlnaJoint"));
            ikJointMap.Add(JointType.WristLeft, Tuple.Create("RightUlna", "RightRadiusHandBaseJoint"));
            ikJointMap.Add(JointType.HandLeft, Tuple.Create("RightHandBase", "RightHandBase"));
            ikJointMap.Add(JointType.ShoulderRight, Tuple.Create("LeftScapula", "LeftScapula"));
            ikJointMap.Add(JointType.ElbowRight, Tuple.Create("LeftHumerus", "LeftHumerusUlnaJoint"));
            ikJointMap.Add(JointType.WristRight, Tuple.Create("LeftUlna", "LeftRadiusHandBaseJoint"));
            ikJointMap.Add(JointType.HandRight, Tuple.Create("LeftHandBase", "LeftHandBase"));
            ikJointMap.Add(JointType.HipLeft, Tuple.Create("Pelvis", "RightFemurPelvisJoint"));
            ikJointMap.Add(JointType.KneeLeft, Tuple.Create("RightFemur", "RightFemurTibiaJoint"));
            ikJointMap.Add(JointType.AnkleLeft, Tuple.Create("RightTibia", "RightTibiaFootBaseJoint"));
            ikJointMap.Add(JointType.FootLeft, Tuple.Create("RightFootBase", "RightFootBase"));
            ikJointMap.Add(JointType.HipRight, Tuple.Create("Pelvis", "LeftFemurPelvisJoint"));
            ikJointMap.Add(JointType.KneeRight, Tuple.Create("LeftFemur", "LeftFemurTibiaJoint"));
            ikJointMap.Add(JointType.AnkleRight, Tuple.Create("LeftTibia", "LeftTibiaFootBaseJoint"));
            ikJointMap.Add(JointType.FootRight, Tuple.Create("LeftFootBase", "LeftFootBase"));

            createDragControlsFor.Add("LeftHandBase");
            createDragControlsFor.Add("RightHandBase");
            createDragControlsFor.Add("Skull");
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
            GenericSimObjectDefinition arrowOnly = new GenericSimObjectDefinition("TestArrow");
            SceneNodeDefinition node = new SceneNodeDefinition("Node");
            EntityDefinition entityDef = new EntityDefinition("Entity");
            entityDef.MeshName = "Arrow.mesh";
            node.addMovableObjectDefinition(entityDef);
            arrowOnly.addElement(node);

            GenericSimObjectDefinition arrowAndDragControl = CopySaver.Default.copy(arrowOnly);
            var dragControl = new BEPUikDragControlDefinition("DragControl");
            arrowAndDragControl.addElement(dragControl);

            var subScene = scene.getDefaultSubScene();

            GenericSimObjectDefinition createMe;

            foreach (var enumVal in EnumUtil.Elements(typeof(JointType)))
            {
                JointType jointType = (JointType)Enum.Parse(typeof(JointType), enumVal);
                var jointInfo = ikJointMap[jointType];

                if(createDragControlsFor.Contains(jointInfo.Item1))
                {
                    createMe = arrowAndDragControl;
                    dragControl.BoneSimObjectName = jointInfo.Item1;
                }
                else
                {
                    createMe = arrowOnly;
                }

                createMe.Name = enumVal;
                createMe.Translation = medicalController.getSimObject(jointInfo.Item2).Translation;
                SimObjectBase instance = createMe.register(subScene);
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
