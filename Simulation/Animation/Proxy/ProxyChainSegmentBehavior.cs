using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    public abstract class ProxyChainSegmentBehavior : BehaviorInterface, ProxyChainSegment
    {
        [Editable]
        private String parentSegmentSimObjectName;

        [Editable]
        private String parentSegmentName = "Follower";

        [DoNotCopy]
        [DoNotSave]
        protected SimObject parentSegmentSimObject;

        [DoNotCopy]
        [DoNotSave]
        private ProxyChainSegment childSegment;

        protected override void link()
        {
            base.link();

            //Parent segment
            parentSegmentSimObject = Owner.getOtherSimObject(parentSegmentSimObjectName);
            if (parentSegmentSimObject == null)
            {
                blacklist("Cannot find parent segment SimObject '{0}'.", parentSegmentSimObjectName);
            }
            var parentSegment = parentSegmentSimObject.getElement(parentSegmentName) as ProxyChainSegment;
            if (parentSegment == null)
            {
                blacklist("Cannot find segment '{0}' on parent segment SimObject '{1}'.", parentSegmentName, parentSegmentSimObjectName);
            }
            parentSegment.setChildSegment(this);
        }

        public void setChildSegment(ProxyChainSegment segment)
        {
            if (childSegment == null)
            {
                childSegment = segment;
            }
            else if (childSegment is MultiChildSegment)
            {
                childSegment.setChildSegment(segment);
            }
            else
            {
                var oldChild = childSegment;
                childSegment = new MultiChildSegment();
                childSegment.setChildSegment(oldChild);
                childSegment.setChildSegment(segment);
            }
        }

        public void updatePosition()
        {
            computePosition();
            if (childSegment != null)
            {
                childSegment.updatePosition();
            }
        }

        protected abstract void computePosition();

        protected override void customLoad(Engine.Saving.LoadInfo info)
        {
            base.customLoad(info);
            parentSegmentSimObjectName = info.GetString("parentSpineSegmentSimObjectName", parentSegmentSimObjectName);
            parentSegmentName = info.GetString("parentSpineSegmentName", parentSegmentName);
        }
    }
}
