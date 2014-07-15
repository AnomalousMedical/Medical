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
        private static Dictionary<JointType, Tuple<String, String>> ikJointMap = new Dictionary<JointType, Tuple<String, String>>();
        private static HashSet<String> createDragControlsFor = new HashSet<string>();

        private DebugDrawingSurface ikDebug;

        static KinectIkController()
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
            createDragControlsFor.Add("LeftUlna");
            createDragControlsFor.Add("RightUlna");
            createDragControlsFor.Add("Skull");
            createDragControlsFor.Add("LeftFootBase");
            createDragControlsFor.Add("RightFootBase");
            createDragControlsFor.Add("LeftFemur");
            createDragControlsFor.Add("RightFemur");
            createDragControlsFor.Add("Pelvis");
        }

        private MedicalController medicalController;
        private GenericSimObjectDefinition dragSimObjectDefinition;
        private BEPUikDragControlDefinition dragControl;
        private KinectIKBone hips;
        private KinectIKBone leftShoulder;
        private KinectIKBone rightShoulder;

        public KinectIkController(StandaloneController controller)
        {
            this.medicalController = controller.MedicalController;

            dragSimObjectDefinition = new GenericSimObjectDefinition("TestArrow");
            dragControl = new BEPUikDragControlDefinition("DragControl");
            dragSimObjectDefinition.addElement(dragControl);
            SceneNodeDefinition node = new SceneNodeDefinition("Node");
            EntityDefinition entityDef = new EntityDefinition("Entity");
            entityDef.MeshName = "Syringe.mesh";
            node.addMovableObjectDefinition(entityDef);
            dragSimObjectDefinition.addElement(node);
        }

        public void createIkControls(SimScene scene)
        {
            var subScene = scene.getDefaultSubScene();
            ikDebug = medicalController.PluginManager.RendererPlugin.createDebugDrawingSurface("KinectIKDebug", subScene);

            hips = createKinectBone(JointType.HipCenter, "Pelvis", "Pelvis", null, scene, subScene);

            KinectIKBone leftKnee = createKinectBone( JointType.KneeRight,  "LeftFemur",    "LeftFemurTibiaJoint",    hips,                             scene, subScene);
            KinectIKBone leftAnkle = createKinectBone(JointType.AnkleRight, "LeftTibia",    "LeftTibiaFootBaseJoint", leftKnee,                         scene, subScene);
            KinectIKBone leftFoot = createKinectBone( JointType.FootRight,  "LeftFootBase", "LeftFootBase",           leftAnkle, new Vector3(0, -2, 5), scene, subScene);

            KinectIKBone rightKnee = createKinectBone( JointType.KneeLeft,   "RightFemur",    "RightFemurTibiaJoint",    hips,                              scene, subScene);
            KinectIKBone rightAnkle = createKinectBone(JointType.AnkleLeft,  "RightTibia",    "RightTibiaFootBaseJoint", rightKnee,                         scene, subScene);
            KinectIKBone rightFoot = createKinectBone( JointType.FootLeft,   "RightFootBase", "RightFootBase",           rightAnkle, new Vector3(0, -2, 5), scene, subScene);

            leftShoulder = createKinectBone(          JointType.ShoulderRight, "LeftScapula",  "LeftScapula",             null,                             scene, subScene);
            KinectIKBone leftElbow = createKinectBone(JointType.ElbowRight,    "LeftHumerus",  "LeftHumerusUlnaJoint",    leftShoulder,                     scene, subScene);
            KinectIKBone leftWrist = createKinectBone(JointType.WristRight,    "LeftUlna",     "LeftRadiusHandBaseJoint", leftElbow,                        scene, subScene);
            KinectIKBone leftHand = createKinectBone( JointType.HandRight,     "LeftHandBase", "LeftHandBase",            leftWrist, new Vector3(0, -5, 2), scene, subScene);

            rightShoulder = createKinectBone(          JointType.ShoulderLeft, "RightScapula",  "RightScapula",             null,                              scene, subScene);
            KinectIKBone rightElbow = createKinectBone(JointType.ElbowLeft,    "RightHumerus",  "RightHumerusUlnaJoint",    rightShoulder,                     scene, subScene);
            KinectIKBone rightWrist = createKinectBone(JointType.WristLeft,    "RightUlna",     "RightRadiusHandBaseJoint", rightElbow,                        scene, subScene);
            KinectIKBone rightHand = createKinectBone( JointType.HandLeft,     "RightHandBase", "RightHandBase",            rightWrist, new Vector3(0, -5, 2), scene, subScene);

            KinectIKBone skull = createKinectBone(JointType.Head, "Skull", "Skull", hips, scene, subScene);
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
            dragSimObjectDefinition.Translation = medicalController.getSimObject(translationSimObjectName).Translation + additionalOffset;
            SimObjectBase instance = dragSimObjectDefinition.register(subScene);
            medicalController.addSimObject(instance);
            scene.buildScene();

            float distanceToParent = 0;
            if (parent != null)
            {
                distanceToParent = (instance.Translation - parent.Translation).length();
            }

            var bone = new KinectIKBone(jointType, distanceToParent, instance);
            if(parent != null)
            {
                parent.addChild(bone);
            }
            return bone;
        }

        public void destroyIkControls(SimScene scene)
        {
            medicalController.PluginManager.RendererPlugin.destroyDebugDrawingSurface(ikDebug);

            //Need to implement this
        }

        public void updateControls(Skeleton skel)
        {
            if (skel.TrackingState != SkeletonTrackingState.NotTracked)
            {
                hips.update(skel);
                leftShoulder.update(skel);
                rightShoulder.update(skel);
                ikDebug.begin("Main", DrawingType.LineList);
                ikDebug.Color = Color.Red;
                hips.render(ikDebug);
                leftShoulder.render(ikDebug);
                rightShoulder.render(ikDebug);
                ikDebug.end();
            }
        }
    }
}
