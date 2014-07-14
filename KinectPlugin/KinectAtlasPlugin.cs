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

        private MedicalController medicalController;
        private Dictionary<JointType, SimObjectBase> testSimObjs = new Dictionary<JointType, SimObjectBase>();

        private static Dictionary<JointType, JointType> parentJointTypeMap = new Dictionary<JointType, JointType>();
        private static Dictionary<JointType, String> ikJointMap = new Dictionary<JointType, String>();

        private MusclePosition bindPosition;

        private DebugDrawingSurface debugDrawer;

        static KinectAtlasPlugin()
        {
            ikJointMap.Add(JointType.HipCenter, null);
            ikJointMap.Add(JointType.Spine, null);
            ikJointMap.Add(JointType.ShoulderCenter, null);
            ikJointMap.Add(JointType.Head, "Skull");
            ikJointMap.Add(JointType.ShoulderLeft, null);
            ikJointMap.Add(JointType.ElbowLeft, "RightHumerus");
            ikJointMap.Add(JointType.WristLeft, "RightUlna");
            ikJointMap.Add(JointType.HandLeft, "RightHandBase");
            ikJointMap.Add(JointType.ShoulderRight, null);
            ikJointMap.Add(JointType.ElbowRight, "LeftHumerus");
            ikJointMap.Add(JointType.WristRight, "LeftUlna");
            ikJointMap.Add(JointType.HandRight, "LeftHandBase");
            ikJointMap.Add(JointType.HipLeft, null);
            ikJointMap.Add(JointType.KneeLeft, "RightFemur");
            ikJointMap.Add(JointType.AnkleLeft, "RightTibia");
            ikJointMap.Add(JointType.FootLeft, "RightFootBase");
            ikJointMap.Add(JointType.HipRight, null);
            ikJointMap.Add(JointType.KneeRight, "LeftFemur");
            ikJointMap.Add(JointType.AnkleRight, "LeftTibia");
            ikJointMap.Add(JointType.FootRight, "LeftFootBase");

            //ikJointMap.Add(JointType.HipCenter, "Pelvis");
            //ikJointMap.Add(JointType.Spine, "SpineC7");
            //ikJointMap.Add(JointType.ShoulderCenter, "Manubrium");
            //ikJointMap.Add(JointType.Head, "Skull");
            //ikJointMap.Add(JointType.ShoulderLeft, "RightScapula");
            //ikJointMap.Add(JointType.ElbowLeft, "RightHumerus");
            //ikJointMap.Add(JointType.WristLeft, "RightUlna");
            //ikJointMap.Add(JointType.HandLeft, "RightHandBase");
            //ikJointMap.Add(JointType.ShoulderRight, "LeftScapula");
            //ikJointMap.Add(JointType.ElbowRight, "LeftHumerus");
            //ikJointMap.Add(JointType.WristRight, "LeftUlna");
            //ikJointMap.Add(JointType.HandRight, "LeftHandBase");
            //ikJointMap.Add(JointType.HipLeft, "Pelvis");
            //ikJointMap.Add(JointType.KneeLeft, "RightFemur");
            //ikJointMap.Add(JointType.AnkleLeft, "RightTibia");
            //ikJointMap.Add(JointType.FootLeft, "RightFootBase");
            //ikJointMap.Add(JointType.HipRight, "Pelvis");
            //ikJointMap.Add(JointType.KneeRight, "LeftFemur");
            //ikJointMap.Add(JointType.AnkleRight, "LeftTibia");
            //ikJointMap.Add(JointType.FootRight, "LeftFootBase");

            parentJointTypeMap.Add(JointType.HipCenter, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.Spine, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.ShoulderCenter, JointType.Spine);
            parentJointTypeMap.Add(JointType.Head, JointType.ShoulderCenter);
            parentJointTypeMap.Add(JointType.ShoulderLeft, JointType.ShoulderCenter);
            parentJointTypeMap.Add(JointType.ElbowLeft, JointType.ShoulderLeft);
            parentJointTypeMap.Add(JointType.WristLeft, JointType.ElbowLeft);
            parentJointTypeMap.Add(JointType.HandLeft, JointType.WristLeft);
            parentJointTypeMap.Add(JointType.ShoulderRight, JointType.ShoulderCenter);
            parentJointTypeMap.Add(JointType.ElbowRight, JointType.ShoulderRight);
            parentJointTypeMap.Add(JointType.WristRight, JointType.ElbowRight);
            parentJointTypeMap.Add(JointType.HandRight, JointType.WristRight);
            parentJointTypeMap.Add(JointType.HipLeft, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.KneeLeft, JointType.HipLeft);
            parentJointTypeMap.Add(JointType.AnkleLeft, JointType.KneeLeft);
            parentJointTypeMap.Add(JointType.FootLeft, JointType.AnkleLeft);
            parentJointTypeMap.Add(JointType.HipRight, JointType.HipCenter);
            parentJointTypeMap.Add(JointType.KneeRight, JointType.HipRight);
            parentJointTypeMap.Add(JointType.AnkleRight, JointType.KneeRight);
            parentJointTypeMap.Add(JointType.FootRight, JointType.AnkleRight);
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
            debugDrawer = medicalController.PluginManager.RendererPlugin.createDebugDrawingSurface("KinectDebug", scene.getDefaultSubScene());

            bindPosition = new MusclePosition();
            bindPosition.captureState();

            GenericSimObjectDefinition kinectJointVisual = new GenericSimObjectDefinition("TestArrow");
            SceneNodeDefinition node = new SceneNodeDefinition("Node");
            EntityDefinition entityDef = new EntityDefinition("Entity");
            entityDef.MeshName = "Arrow.mesh";
            node.addMovableObjectDefinition(entityDef);
            kinectJointVisual.addElement(node);

            var subScene = scene.getDefaultSubScene();

            foreach (var enumVal in EnumUtil.Elements(typeof(JointType)))
            {
                JointType jointType = (JointType)Enum.Parse(typeof(JointType), enumVal);
                var jointInfo = ikJointMap[jointType];

                kinectJointVisual.Name = enumVal;
                SimObjectBase instance = kinectJointVisual.register(subScene);
                medicalController.addSimObject(instance);
                scene.buildScene();

                SceneNodeElement sceneNode = instance.getElement("Node") as SceneNodeElement;
                if (sceneNode != null)
                {
                    var entity = sceneNode.getNodeObject("Entity") as OgreWrapper.Entity;
                    var subEntity = entity.getSubEntity(0);
                    subEntity.setCustomParameter(1, new Quaternion(1, 0, 0, 1));
                }

                testSimObjs.Add(jointType, instance);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            medicalController.PluginManager.RendererPlugin.destroyDebugDrawingSurface(debugDrawer);
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
            return new Vector3(joint.Position.X * 1000f * SimulationConfig.MMToUnits, joint.Position.Y * 1000f * SimulationConfig.MMToUnits - 90f, (joint.Position.Z - 2) * 1000f * SimulationConfig.MMToUnits);
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
                        foreach(Joint joint in skel.Joints)
                        {
                            if(joint.TrackingState != JointTrackingState.NotTracked)
                            {
                                SimObjectBase simObject = testSimObjs[joint.JointType];
                                if (simObject != null)
                                {
                                    Vector3 pos = convertPoint(joint);

                                    JointType parentJoint = parentJointTypeMap[joint.JointType];
                                    Quaternion absOrientation;
                                    Quaternion relOrientation;
                                    Vector3 direction;
                                    Vector3 parentPos = convertPoint(skel.Joints[parentJoint]);
                                    float length = 0;
                                    if(parentJoint == joint.JointType)
                                    {
                                        relOrientation = absOrientation = Quaternion.Identity;
                                        direction = Vector3.Zero;
                                    }
                                    else
                                    {
                                        //Option 1
                                        direction = pos - parentPos;
                                        length = direction.length();
                                        direction.normalize();
                                        //orientation = Quaternion.shortestArcQuatFixedYaw(ref direction, ref Vector3.Up);

                                        var msQuat = skel.BoneOrientations[joint.JointType].AbsoluteRotation.Quaternion;
                                        absOrientation = new Quaternion(msQuat.X, msQuat.Y, msQuat.Z, msQuat.W);

                                        var msRelQuat = skel.BoneOrientations[joint.JointType].HierarchicalRotation.Quaternion;
                                        relOrientation = new Quaternion(msRelQuat.X, msRelQuat.Y, msRelQuat.Z, msRelQuat.W);
                                    }

                                    String lineName = joint.JointType.ToString();

                                    ThreadManager.invoke(() =>
                                        {
                                            simObject.updatePosition(ref pos, ref absOrientation, null);

                                            float halfLength = length / 2;

                                            debugDrawer.begin(lineName, DrawingType.LineList);
                                            debugDrawer.Color = Color.White;
                                            debugDrawer.drawLine(parentPos, parentPos + direction * halfLength);
                                            debugDrawer.Color = Color.Green;
                                            debugDrawer.drawLine(parentPos + direction * halfLength, parentPos + direction * length);
                                            debugDrawer.end();
                                        });

                                    //Modify bind position
                                    String linkName = ikJointMap[joint.JointType];
                                    if (linkName != null)
                                    {
                                        var fkLink = bindPosition.PelvisChainState[linkName];
                                        fkLink.LocalRotation = relOrientation;
                                    }
                                }
                            }
                        }
                    }
                }

                ThreadManager.invoke(() =>
                    {
                        bindPosition.preview();
                    });
            }
        }
    }
}
