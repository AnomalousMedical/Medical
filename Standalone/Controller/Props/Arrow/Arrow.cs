using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgreWrapper;
using OgrePlugin;
using Engine.Attributes;
using Engine.ObjectManagement;

namespace Medical
{
    public class Arrow : BehaviorInterface
    {
        public const string ArrowBehaviorName = "Behavior";

        private Entity entity;
        private SubEntity subEntity;
        private Color color = Color.Red;

        private Bone topBone;
        private Bone bottomBone;
        private float tailLength;
        private float scale = 1.0f;

        public Arrow()
        {

        }

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in Arrow SimObject", PropFactory.NodeName);
            }
            entity = sceneNode.getNodeObject(PropFactory.EntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in Arrow SimObject", PropFactory.EntityName);
            }
            subEntity = entity.getSubEntity(0);
            if (subEntity == null)
            {
                blacklist("Could not find sub entity with index 0 in Arrow SimObject");
            }
            Skeleton skeleton = entity.getSkeleton();
            if (skeleton == null)
            {
                blacklist("Could not find skeleton for Arrow SimObject");
            }
            topBone = skeleton.getBone("ArrowTopBone");
            if (topBone == null)
            {
                blacklist("Could not find 'ArrowTopBone' for Arrow SimObject");
            }
            topBone.setManuallyControlled(true);
            bottomBone = skeleton.getBone("ArrowBottomBone");
            if (bottomBone == null)
            {
                blacklist("Could not find 'ArrowBottomBone' for Arrow SimObject");
            }
            bottomBone.setManuallyControlled(true);
            Color = color;
            tailLength = bottomBone.getPosition().y;
        }

        protected override void destroy()
        {
            base.destroy();
        }

        [DoNotCopy]
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                topBone.setScale(new Vector3(scale, scale, scale));
                topBone.needUpdate(true);
            }
        }

        [DoNotCopy]
        public float TailLength
        {
            get
            {
                return tailLength;
            }
            set
            {
                tailLength = value;
                Vector3 pos = bottomBone.getPosition();
                pos.y = value;
                bottomBone.setPosition(pos);
                bottomBone.needUpdate(true);
            }
        }

        [DoNotCopy]
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (subEntity != null)
                {
                    subEntity.setCustomParameter(1, new Quaternion(value.r, value.g, value.b, value.a));
                }
            }
        }
    }
}
