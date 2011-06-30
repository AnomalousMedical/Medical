using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class PoseableFingerAnimator : Saveable
    {
        private PoseableFingerSectionAnimator metacarpalAnimator;
        private PoseableFingerSectionAnimator proximalAnimator;
        private PoseableFingerSectionAnimator intermediateAnimator;
        private PoseableFingerSectionAnimator distalAnimator;

        public PoseableFingerAnimator()
        {
            metacarpalAnimator = new PoseableFingerSectionAnimator();
            proximalAnimator = new PoseableFingerSectionAnimator();
            intermediateAnimator = new PoseableFingerSectionAnimator();
            distalAnimator = new PoseableFingerSectionAnimator();
        }

        public void setFinger(PoseableFinger finger)
        {
            metacarpalAnimator.FingerSection = finger.Metacarpal;
            proximalAnimator.FingerSection = finger.ProximalPhalanges;
            intermediateAnimator.FingerSection = finger.IntermediatePhalanges;
            distalAnimator.FingerSection = finger.DistalPhalanges;
        }

        public PoseableFingerSectionAnimator MetacarpalAnimator
        {
            get
            {
                return metacarpalAnimator;
            }
        }

        public PoseableFingerSectionAnimator ProximalAnimator
        {
            get
            {
                return proximalAnimator;
            }
        }

        public PoseableFingerSectionAnimator IntermediateAnimator
        {
            get
            {
                return intermediateAnimator;
            }
        }

        public PoseableFingerSectionAnimator DistalAnimator
        {
            get
            {
                return distalAnimator;
            }
        }

        #region Saveable Members

        protected PoseableFingerAnimator(LoadInfo info)
        {
            metacarpalAnimator = info.GetValue<PoseableFingerSectionAnimator>("MetacarpalAnimator");
            proximalAnimator = info.GetValue<PoseableFingerSectionAnimator>("ProximalAnimator");
            intermediateAnimator = info.GetValue<PoseableFingerSectionAnimator>("IntermediateAnimator");
            distalAnimator = info.GetValue<PoseableFingerSectionAnimator>("DistalAnimator");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("MetacarpalAnimator", metacarpalAnimator);
            info.AddValue("ProximalAnimator", proximalAnimator);
            info.AddValue("IntermediateAnimator", intermediateAnimator);
            info.AddValue("DistalAnimator", distalAnimator);
        }

        #endregion
    }
}
