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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectIkController
    {
        private static Dictionary<JointType, Tuple<String, String>> ikJointMap = new Dictionary<JointType, Tuple<String, String>>();
        private static HashSet<String> createDragControlsFor = new HashSet<string>();

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
        private Dictionary<JointType, SimObjectBase> dragSimObjs = new Dictionary<JointType, SimObjectBase>();
        private GenericSimObjectDefinition dragSimObjectDefinition;
        private BEPUikDragControlDefinition dragControl;

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

            foreach (var enumVal in EnumUtil.Elements(typeof(JointType)))
            {
                JointType jointType = (JointType)Enum.Parse(typeof(JointType), enumVal);
                var jointInfo = ikJointMap[jointType];

                if (createDragControlsFor.Contains(jointInfo.Item1))
                {
                    dragControl.BoneSimObjectName = jointInfo.Item1;

                    var targetSimObject = medicalController.getSimObject(dragControl.BoneSimObjectName);
                    var ikBone = targetSimObject.getElement("IKBone") as BEPUikBone;
                    ikBone.Pinned = false;

                    dragSimObjectDefinition.Name = enumVal + "DragControl";
                    dragSimObjectDefinition.Translation = medicalController.getSimObject(jointInfo.Item2).Translation;
                    SimObjectBase instance = dragSimObjectDefinition.register(subScene);
                    medicalController.addSimObject(instance);
                    scene.buildScene();

                    dragSimObjs.Add(jointType, instance);
                }
            }
        }

        public void destroyIkControls(SimScene scene)
        {
            dragSimObjs.Clear();
        }

        public void updateControls(Skeleton skel)
        {
            if (skel.TrackingState != SkeletonTrackingState.NotTracked)
            {
                //Walk the bone tree
                //walkBones(JointType.HipCenter);

                foreach (Joint joint in skel.Joints)
                {
                    SimObjectBase simObject;
                    if (dragSimObjs.TryGetValue(joint.JointType, out simObject))
                    {
                        Vector3 pos = joint.Position.toEngineCoords();
                        simObject.updateTranslation(ref pos, null);
                    }
                }
            }
        }

        private void walkBones(JointType jointType, Skeleton skel)
        {
            Joint joint = skel.Joints[jointType];
            var bone = skel.BoneOrientations[jointType];
            SimObjectBase simObject = dragSimObjs[joint.JointType];
            if (simObject != null)
            {
                Vector3 pos = joint.Position.toEngineCoords();
                simObject.updateTranslation(ref pos, null);
            }
        }
    }
}
