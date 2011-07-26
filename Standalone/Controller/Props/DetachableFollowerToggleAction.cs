using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    [TimelineActionProperties("Attach To Object")]
    public class DetachableFollowerToggleAction : EditableShowPropSubAction
    {
        public DetachableFollowerToggleAction()
        {

        }

        public override void started(float timelineTime, Clock clock)
        {
            doAttachment();
        }

        public override void skipTo(float timelineTime)
        {
            doAttachment();
        }

        public override void stopped(float timelineTime, Clock clock)
        {

        }

        public override void update(float timelineTime, Clock clock)
        {

        }

        public override void editing()
        {
            doAttachment();
        }

        [Editable]
        public bool Attached { get; set; }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        public override float Duration
        {
            get
            {
                return 0.0f;
            }
            set
            {

            }
        }

        private void doAttachment()
        {
            DetachableSimObjectFollower follower = PropSimObject.getElement(PropFactory.DetachableFollowerName) as DetachableSimObjectFollower;
            if (follower != null)
            {
                if (Attached)
                {
                    follower.attach();
                }
                else
                {
                    follower.detach();
                }
            }
        }

        #region Saveable Members

        protected DetachableFollowerToggleAction(LoadInfo info)
            : base(info)
        {
            Attached = info.GetBoolean("Attached", Attached);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("Attached", Attached);
        }

        #endregion
    }
}
