using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PoseableHandProperties : TimelineDataPanel
    {
        private ChangeHandPosition handPosition;

        private PoseableFingerSectionControl pinkyMeta;
        private PoseableFingerSectionControl pinkyProx;
        private PoseableFingerSectionControl pinkyInter;
        private PoseableFingerSectionControl pinkyDistal;

        private PoseableFingerSectionControl ringMeta;
        private PoseableFingerSectionControl ringProx;
        private PoseableFingerSectionControl ringInter;
        private PoseableFingerSectionControl ringDistal;

        private PoseableFingerSectionControl middleMeta;
        private PoseableFingerSectionControl middleProx;
        private PoseableFingerSectionControl middleInter;
        private PoseableFingerSectionControl middleDistal;

        private PoseableFingerSectionControl indexMeta;
        private PoseableFingerSectionControl indexProx;
        private PoseableFingerSectionControl indexInter;
        private PoseableFingerSectionControl indexDistal;

        private PoseableFingerSectionControl thumbMeta;
        private PoseableFingerSectionControl thumbProx;
        private PoseableFingerSectionControl thumbDistal;

        public PoseableHandProperties(Widget parentWidget, String layoutFile)
            :base(parentWidget, layoutFile)
        {
            pinkyMeta = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("PinkyMetaBottom"), (Edit)mainWidget.findWidget("PinkyMetaTop"));
            pinkyProx = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("PinkyProxTop"));
            pinkyInter = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("PinkyInterTop"));
            pinkyDistal = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("PinkyDistalTop"));

            ringMeta = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("RingMetaBottom"), (Edit)mainWidget.findWidget("RingMetaTop"));
            ringProx = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("RingProxTop"));
            ringInter = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("RingInterTop"));
            ringDistal = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("RingDistalTop"));

            middleMeta = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("MiddleMetaBottom"), (Edit)mainWidget.findWidget("MiddleMetaTop"));
            middleProx = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("MiddleProxTop"));
            middleInter = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("MiddleInterTop"));
            middleDistal = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("MiddleDistalTop"));

            indexMeta = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("IndexMetaBottom"), (Edit)mainWidget.findWidget("IndexMetaTop"));
            indexProx = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("IndexProxTop"));
            indexInter = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("IndexInterTop"));
            indexDistal = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("IndexDistalTop"));

            thumbMeta = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("ThumbMetaBottom"), (Edit)mainWidget.findWidget("ThumbMetaTop"));
            thumbProx = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("ThumbProxTop"));
            thumbDistal = new PoseableFingerSectionControl((Edit)mainWidget.findWidget("ThumbDistalTop"));
        }

        public override void setCurrentData(TimelineData data)
        {
            PropTimelineData propData = (PropTimelineData)data;
            handPosition = (ChangeHandPosition)propData.Action;

            pinkyMeta.FingerSection = handPosition.Pinky.MetacarpalAnimator;
            pinkyProx.FingerSection = handPosition.Pinky.ProximalAnimator;
            pinkyInter.FingerSection = handPosition.Pinky.IntermediateAnimator;
            pinkyDistal.FingerSection = handPosition.Pinky.DistalAnimator;

            ringMeta.FingerSection = handPosition.Ring.MetacarpalAnimator;
            ringProx.FingerSection = handPosition.Ring.ProximalAnimator;
            ringInter.FingerSection = handPosition.Ring.IntermediateAnimator;
            ringDistal.FingerSection = handPosition.Ring.DistalAnimator;

            middleMeta.FingerSection = handPosition.Middle.MetacarpalAnimator;
            middleProx.FingerSection = handPosition.Middle.ProximalAnimator;
            middleInter.FingerSection = handPosition.Middle.IntermediateAnimator;
            middleDistal.FingerSection = handPosition.Middle.DistalAnimator;

            indexMeta.FingerSection = handPosition.Index.MetacarpalAnimator;
            indexProx.FingerSection = handPosition.Index.ProximalAnimator;
            indexInter.FingerSection = handPosition.Index.IntermediateAnimator;
            indexDistal.FingerSection = handPosition.Index.DistalAnimator;

            thumbMeta.FingerSection = handPosition.Thumb.MetacarpalAnimator;
            thumbProx.FingerSection = handPosition.Thumb.ProximalAnimator;
            thumbDistal.FingerSection = handPosition.Thumb.DistalAnimator;
        }
    }
}
