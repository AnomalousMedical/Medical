using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
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
    class KinectDebugVisualizer
    {
        private Dictionary<JointType, SimObjectBase> debugSimObjs = new Dictionary<JointType, SimObjectBase>();
        private DebugDrawingSurface debugDrawer;

        private MedicalController medicalController;

        public KinectDebugVisualizer(StandaloneController controller)
        {
            medicalController = controller.MedicalController;
        }

        public void createDebugObjects(SimScene scene)
        {
            debugDrawer = medicalController.PluginManager.RendererPlugin.createDebugDrawingSurface("KinectDebug", scene.getDefaultSubScene());

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

                kinectJointVisual.Name = enumVal + "KinectDebugVisual";
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

                debugSimObjs.Add(jointType, instance);
            }
        }

        public void destroyDebugObjects(SimScene scene)
        {
            if (debugDrawer != null)
            {
                medicalController.PluginManager.RendererPlugin.destroyDebugDrawingSurface(debugDrawer);
            }
            debugSimObjs.Clear();
        }

        public void debugSkeleton(Skeleton skel)
        {
            //Debug drawing
            foreach (Joint joint in skel.Joints)
            {
                if (joint.TrackingState != JointTrackingState.NotTracked)
                {
                    SimObjectBase simObject = debugSimObjs[joint.JointType];
                    if (simObject != null)
                    {
                        Vector3 pos = joint.Position.toEngineCoords();

                        JointType parentJoint = KinectUtilities.GetParentJoint(joint.JointType);
                        Quaternion absOrientation;
                        Vector3 direction;
                        Vector3 parentPos = skel.Joints[parentJoint].Position.toEngineCoords();
                        float length = 0;
                        if (parentJoint == joint.JointType)
                        {
                            absOrientation = Quaternion.Identity;
                            direction = Vector3.Zero;
                        }
                        else
                        {
                            //Option 1
                            direction = pos - parentPos;
                            length = direction.length();
                            direction.normalize();

                            absOrientation = skel.BoneOrientations[joint.JointType].AbsoluteRotation.Quaternion.toEngineQuat();
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
                    }
                }
            }
        }
    }
}
