using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class PoseableThumbAnimator : Saveable
    {
        private PoseableFingerSectionAnimator metacarpalAnimator;
        private PoseableFingerSectionAnimator proximalAnimator;
        private PoseableFingerSectionAnimator distalAnimator;

        public PoseableThumbAnimator()
        {
            metacarpalAnimator = new PoseableFingerSectionAnimator();
            proximalAnimator = new PoseableFingerSectionAnimator();
            distalAnimator = new PoseableFingerSectionAnimator();
        }

        public void setFinger(PoseableThumb thumb)
        {
            metacarpalAnimator.FingerSection = thumb.Metacarpal;
            proximalAnimator.FingerSection = thumb.ProximalPhalanges;
            distalAnimator.FingerSection = thumb.DistalPhalanges;
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

        public PoseableFingerSectionAnimator DistalAnimator
        {
            get
            {
                return distalAnimator;
            }
        }

        #region Saveable Members

        protected PoseableThumbAnimator(LoadInfo info)
        {
            metacarpalAnimator = info.GetValue<PoseableFingerSectionAnimator>("MetacarpalAnimator");
            proximalAnimator = info.GetValue<PoseableFingerSectionAnimator>("ProximalAnimator");
            distalAnimator = info.GetValue<PoseableFingerSectionAnimator>("DistalAnimator");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("MetacarpalAnimator", metacarpalAnimator);
            info.AddValue("ProximalAnimator", proximalAnimator);
            info.AddValue("DistalAnimator", distalAnimator);
        }

        #endregion
    }
}
