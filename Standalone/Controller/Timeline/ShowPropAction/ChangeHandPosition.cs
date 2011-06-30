using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Editing;
using Engine;
using Engine.Attributes;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Hand Position")]
    public class ChangeHandPosition : EditableShowPropSubAction
    {
        [DoNotSave]
        private PoseableHand hand;

        public ChangeHandPosition()
        {
            Thumb = new PoseableThumbAnimator();
            Index = new PoseableFingerAnimator();
            Middle = new PoseableFingerAnimator();
            Ring = new PoseableFingerAnimator();
            Pinky = new PoseableFingerAnimator();
        }

        public override void started(float timelineTime, Clock clock)
        {
            findHandBehavior();
        }

        public override void skipTo(float timelineTime)
        {
            findHandBehavior();
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override void editing()
        {
            findHandBehavior();
            Thumb.apply();
            Index.apply();
            Middle.apply();
            Ring.apply();
            Pinky.apply();
        }

        private void findHandBehavior()
        {
            hand = (PoseableHand)PropSimObject.getElement(PoseableHand.PoseableHandBehavior);
            Thumb.setFinger(hand.Thumb);
            Index.setFinger(hand.Index);
            Middle.setFinger(hand.Middle);
            Ring.setFinger(hand.Ring);
            Pinky.setFinger(hand.Pinky);
        }

        public override bool Finished
        {
            get 
            {
                return true;
            }
        }

        public PoseableThumbAnimator Thumb { get; set; }

        public PoseableFingerAnimator Index { get; set; }

        public PoseableFingerAnimator Middle { get; set; }

        public PoseableFingerAnimator Ring { get; set; }

        public PoseableFingerAnimator Pinky { get; set; }

        #region Saveable Members

        protected ChangeHandPosition(LoadInfo info)
            :base (info)
        {
            Thumb = info.GetValue<PoseableThumbAnimator>("Thumb");
            Index = info.GetValue<PoseableFingerAnimator>("Index");
            Middle = info.GetValue<PoseableFingerAnimator>("Middle");
            Ring = info.GetValue<PoseableFingerAnimator>("Ring");
            Pinky = info.GetValue<PoseableFingerAnimator>("Pinky");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);

            info.AddValue("Thumb", Thumb);
            info.AddValue("Index", Index);
            info.AddValue("Middle", Middle);
            info.AddValue("Ring", Ring);
            info.AddValue("Pinky", Pinky);
        }

        #endregion
    }
}
