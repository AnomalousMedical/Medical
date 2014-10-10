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
        private DebugDrawingSurface debugDrawer;
        private bool debugVisible = true;
        private MedicalController medicalController;

        public KinectDebugVisualizer(StandaloneController controller)
        {
            medicalController = controller.MedicalController;
        }

        public void createDebugObjects(SimScene scene)
        {
            debugDrawer = medicalController.PluginManager.RendererPlugin.createDebugDrawingSurface("KinectDebug", scene.getDefaultSubScene());
        }

        public void destroyDebugObjects(SimScene scene)
        {
            if (debugDrawer != null)
            {
                medicalController.PluginManager.RendererPlugin.destroyDebugDrawingSurface(debugDrawer);
                debugDrawer = null;
            }
        }

        public void debugSkeleton(Body skel)
        {
            if (debugVisible && skel.IsTracked)
            {
                debugDrawer.begin("SkeletonId" + skel.TrackingId, DrawingType.LineList);
                foreach (Joint joint in skel.Joints.Values)
                {
                    Vector3 pos = joint.Position.toEngineCoords();

                    JointType parentJoint = KinectUtilities.GetParentJoint(joint.JointType);
                    Vector3 direction;
                    Vector3 parentPos = skel.Joints[parentJoint].Position.toEngineCoords();
                    float length = 0;
                    if (parentJoint == joint.JointType)
                    {
                        direction = Vector3.Zero;
                    }
                    else
                    {
                        direction = pos - parentPos;
                        length = direction.length();
                        direction.normalize();
                    }

                    float halfLength = length / 2;

                    debugDrawer.Color = Color.White;
                    debugDrawer.drawLine(parentPos, parentPos + direction * halfLength);
                    debugDrawer.Color = Color.Green;
                    debugDrawer.drawLine(parentPos + direction * halfLength, parentPos + direction * length);
                }
                debugDrawer.end();
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
                if (debugDrawer != null)
                {
                    debugDrawer.setVisible(debugVisible);
                }
            }
        }
    }
}
