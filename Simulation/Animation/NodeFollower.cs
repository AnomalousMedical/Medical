using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Attributes;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine.Platform;
using OgreWrapper;

namespace Medical
{
    class NodeFollower : Behavior
    {
        [Editable]
        String targetSimObject;

        [Editable]
        String targetNode;

        SimObject targetObject;
        SceneNodeElement nodeElement;

        [DoNotSave]
        [DoNotCopy]
        Vector3 offset;

        [DoNotSave]
        [DoNotCopy]
        Vector3 lastPosition;

        protected override void constructed()
        {
            targetObject = Owner.getOtherSimObject(targetSimObject);
            if (targetObject != null)
            {
                nodeElement = targetObject.getElement(targetNode) as SceneNodeElement;
                if (nodeElement != null)
                {
                    offset = this.Owner.Translation - nodeElement.getDerivedPosition();
                }
                else
                {
                    blacklist("Could not find target SceneNodeElement {0}.", targetNode);
                }
            }
            else
            {
                blacklist("Could not find Target SimObject {0}.", targetSimObject);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 bonePos = nodeElement.getDerivedPosition();
            Quaternion rot = nodeElement.getDerivedOrientation();
            //if (bonePos != lastPosition)
            {
                this.updateScale(nodeElement.getDerivedScale());
                Vector3 newPos = nodeElement.getDerivedPosition() + Quaternion.quatRotate(rot, offset * Owner.Scale);
                this.updatePosition(ref newPos, ref rot);
                lastPosition = bonePos;
            }
        }
    }
}
