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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectIkController
    {
        private DebugDrawingSurface ikDebug;

        private MedicalController medicalController;
        private GenericSimObjectDefinition dragSimObjectDefinition;
        private BEPUikDragControlDefinition dragControl;
        private KinectIKBone hips;
        private bool debugVisible = true;
        private bool allowMovement = false;
        private List<SimObject> ikDragSimObjects = new List<SimObject>();

        public event Action<KinectIkController> AllowMovementChanged;

        public KinectIkController(StandaloneController controller)
        {
            this.medicalController = controller.MedicalController;

            dragSimObjectDefinition = new GenericSimObjectDefinition("TestArrow");
            dragControl = new BEPUikDragControlDefinition("DragControl");
            dragSimObjectDefinition.addElement(dragControl);
            //SceneNodeDefinition node = new SceneNodeDefinition("Node");
            //EntityDefinition entityDef = new EntityDefinition("Entity");
            //entityDef.MeshName = "Syringe.mesh";
            //node.addMovableObjectDefinition(entityDef);
            //dragSimObjectDefinition.addElement(node);
        }

        public void createIkControls(SimScene scene)
        {
            if(!canConnectToScene())
            {
                return;
            }

            var subScene = scene.getDefaultSubScene();

            ikDebug = medicalController.PluginManager.RendererPlugin.createDebugDrawingSurface("KinectIKDebug", subScene);
            ikDebug.setVisible(debugVisible);

            hips = createKinectBone(JointType.HipCenter, "Pelvis", "Pelvis", null, scene, subScene);

            KinectIKBone skull = createKinectBone(JointType.Head, "Skull", "Skull", hips, scene, subScene);

            KinectIKBone leftKnee = createKinectBone( JointType.KneeRight,  "LeftFemur",    "LeftFemurTibiaJoint",    hips,                             scene, subScene);
            KinectIKBone leftAnkle = createKinectBone(JointType.AnkleRight, "LeftTibia",    "LeftTibiaFootBaseJoint", leftKnee,                         scene, subScene);
            KinectIKBone leftFoot = createKinectBone( JointType.FootRight,  "LeftFootBase", "LeftFootBase",           leftAnkle, new Vector3(0, -2, 5), scene, subScene);

            KinectIKBone rightKnee = createKinectBone( JointType.KneeLeft,   "RightFemur",    "RightFemurTibiaJoint",    hips,                              scene, subScene);
            KinectIKBone rightAnkle = createKinectBone(JointType.AnkleLeft,  "RightTibia",    "RightTibiaFootBaseJoint", rightKnee,                         scene, subScene);
            KinectIKBone rightFoot = createKinectBone( JointType.FootLeft,   "RightFootBase", "RightFootBase",           rightAnkle, new Vector3(0, -2, 5), scene, subScene);

            KinectIKBone leftShoulder = createKinectBone(JointType.ShoulderRight, "LeftScapula", "LeftScapula", skull, scene, subScene);
            KinectIKBone leftElbow = createKinectBone(JointType.ElbowRight,    "LeftHumerus",  "LeftHumerusUlnaJoint",    leftShoulder,                     scene, subScene);
            KinectIKBone leftWrist = createKinectBone(JointType.WristRight,    "LeftUlna",     "LeftRadiusHandBaseJoint", leftElbow,                        scene, subScene);
            KinectIKBone leftHand = createKinectBone( JointType.HandRight,     "LeftHandBase", "LeftHandBase",            leftWrist, new Vector3(0, -5, 2), scene, subScene);

            KinectIKBone rightShoulder = createKinectBone(JointType.ShoulderLeft, "RightScapula", "RightScapula", skull, scene, subScene);
            KinectIKBone rightElbow = createKinectBone(JointType.ElbowLeft,    "RightHumerus",  "RightHumerusUlnaJoint",    rightShoulder,                     scene, subScene);
            KinectIKBone rightWrist = createKinectBone(JointType.WristLeft,    "RightUlna",     "RightRadiusHandBaseJoint", rightElbow,                        scene, subScene);
            KinectIKBone rightHand = createKinectBone( JointType.HandLeft,     "RightHandBase", "RightHandBase",            rightWrist, new Vector3(0, -5, 2), scene, subScene);
        }

        public void destroyIkControls(SimScene scene)
        {
            if(ikDebug != null)
            {
                medicalController.PluginManager.RendererPlugin.destroyDebugDrawingSurface(ikDebug);
                ikDebug = null;
            }
            hips = null;
            ikDragSimObjects.Clear();
        }

        public void updateControls(Skeleton skel)
        {
            if (hips != null && skel.TrackingState != SkeletonTrackingState.NotTracked)
            {
                hips.update(skel);
                if (debugVisible)
                {
                    ikDebug.begin("Main", DrawingType.LineList);
                    ikDebug.Color = Color.Red;
                    hips.render(ikDebug);
                    ikDebug.end();
                }
            }
        }

        public bool DebugVisible
        {
            get
            {
                return debugVisible;
            }
            set
            {
                debugVisible = value;
                if(ikDebug != null)
                {
                    ikDebug.setVisible(debugVisible);
                }
            }
        }

        public bool AllowMovement
        {
            get
            {
                return allowMovement;
            }
            set
            {
                if (allowMovement != value)
                {
                    allowMovement = value;
                    foreach(var simObj in ikDragSimObjects)
                    {
                        simObj.Enabled = allowMovement;
                    }
                    if(AllowMovementChanged != null)
                    {
                        AllowMovementChanged.Invoke(this);
                    }
                }
            }
        }

        private KinectIKBone createKinectBone(JointType jointType, String boneSimObjectName, String translationSimObjectName, KinectIKBone parent, SimScene scene, SimSubScene subScene)
        {
            return createKinectBone(jointType, boneSimObjectName, translationSimObjectName, parent, Vector3.Zero, scene, subScene);
        }

        private KinectIKBone createKinectBone(JointType jointType, String boneSimObjectName, String translationSimObjectName, KinectIKBone parent, Vector3 additionalOffset, SimScene scene, SimSubScene subScene)
        {
            dragControl.BoneSimObjectName = boneSimObjectName;

            var targetSimObject = medicalController.getSimObject(dragControl.BoneSimObjectName);
            var ikBone = targetSimObject.getElement("IKBone") as BEPUikBone;
            ikBone.Pinned = false;

            dragSimObjectDefinition.Name = jointType + "DragControl";
            dragSimObjectDefinition.Enabled = allowMovement;
            dragSimObjectDefinition.Translation = medicalController.getSimObject(translationSimObjectName).Translation + additionalOffset;
            SimObjectBase instance = dragSimObjectDefinition.register(subScene);
            medicalController.addSimObject(instance);
            scene.buildScene();

            ikDragSimObjects.Add(instance);

            float distanceToParent = 0;
            if (parent != null)
            {
                distanceToParent = (instance.Translation - parent.Translation).length();
            }

            var bone = new KinectIKBone(jointType, distanceToParent, instance);

            if (parent != null)
            {
                parent.addChild(bone);
            }
            return bone;
        }

        /// <summary>
        /// A quick check to see if the desired sim objects exist, does not check them all, but
        /// this should be enough with the current scenes.
        /// </summary>
        /// <returns></returns>
        private bool canConnectToScene()
        {
            return medicalController.getSimObject("LeftUlna") != null;
        }
    }
}
