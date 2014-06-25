using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class will search for poseables by raycasting onto an entity.
    /// </summary>
    class PoseableByEntity : Interface, PoseableIdentifier
    {
        [Editable]
        private String entitySimObjectName = "this";

        [Editable]
        private String nodeName = "Node";

        [Editable]
        private String entityName = "Entity";

        [Editable]
        private String boneSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        private Entity entity;

        [DoNotCopy]
        [DoNotSave]
        private BEPUikBone bone;

        protected override void link()
        {
            base.link();
            SimObject entitySimObject = Owner.getOtherSimObject(entitySimObjectName);
            if(entitySimObject == null)
            {
                blacklist("Cannot find Entity SimObject named '{0}'", entitySimObjectName);
            }

            SceneNodeElement node = entitySimObject.getElement(nodeName) as SceneNodeElement;
            if(node == null)
            {
                blacklist("Cannot find Node '{0}' on Entity SimObject '{1}'", nodeName, entitySimObjectName);
            }

            entity = node.getNodeObject(entityName) as Entity;
            if(entity == null)
            {
                blacklist("Cannot find Entity '{0}' in node '{1}' on Entity SimObject '{2}'", entityName, nodeName, entitySimObjectName);
            }

            SimObject boneSimObject = Owner.getOtherSimObject(boneSimObjectName);
            if(boneSimObject == null)
            {
                blacklist("Cannot find Bone SimObject named '{0}'", boneSimObjectName);
            }

            bone = boneSimObject.getElement(boneName) as BEPUikBone;
            if (bone == null)
            {
                blacklist("Cannot find BEPUikBone '{0}' in Bone SimObject '{1}'", boneName, boneSimObjectName);
            }

            PoseableObjectsManager.add(this);
        }

        protected override void destroy()
        {
            PoseableObjectsManager.remove(this);
            base.destroy();
        }

        public bool checkCollision(Ray3 ray, ref float distanceOnRay)
        {
            return entity.raycastPolygonLevel(ray, ref distanceOnRay);
        }

        public BEPUikBone Bone
        {
            get
            {
                return bone;
            }
        }
    }
}
