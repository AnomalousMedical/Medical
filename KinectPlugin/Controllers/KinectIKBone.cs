using BEPUikPlugin;
using Engine;
using Engine.ObjectManagement;
using Engine.Renderer;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectPlugin
{
    class KinectIKBone
    {
        private static GenericSimObjectDefinition dragSimObjectDefinition;
        private static BEPUikDragControlDefinition dragControl;

        private List<KinectIKBone> children = new List<KinectIKBone>();
        private JointType jointType;
        private float distanceToParent;
        private SimObjectBase simObject;

        public KinectIKBone(JointType jointType, float distanceToParent, SimObjectBase simObject)
        {
            this.jointType = jointType;
            this.distanceToParent = distanceToParent;
            this.simObject = simObject;
        }

        public void update(Skeleton skeleton)
        {
            Vector3 pos = skeleton.Joints[jointType].Position.toEngineCoords();

            simObject.updateTranslation(ref pos, null);

            foreach (var child in children)
            {
                child.update(skeleton, pos);
            }
        }

        private void update(Skeleton skeleton, Vector3 parentPosition)
        {
            Vector3 pos = skeleton.Joints[jointType].Position.toEngineCoords();

            Vector3 direction = pos - parentPosition;
            direction.normalize();

            Vector3 newPos = parentPosition + direction * distanceToParent;
            simObject.updateTranslation(ref newPos, null);

            foreach(var child in children)
            {
                child.update(skeleton, pos);
            }
        }

        public void render(DebugDrawingSurface debugDraw)
        {
            foreach (var child in children)
            {
                child.render(debugDraw, Translation);
            }
        }

        private void render(DebugDrawingSurface debugDraw, Vector3 parentPosition)
        {
            debugDraw.drawLine(parentPosition, Translation);

            foreach (var child in children)
            {
                child.render(debugDraw, Translation);
            }
        }

        public void addChild(KinectIKBone child)
        {
            children.Add(child);
        }

        public void removeChild(KinectIKBone child)
        {
            children.Remove(child);
        }

        public Vector3 Translation
        {
            get
            {
                return simObject.Translation;
            }
        }
    }
}
