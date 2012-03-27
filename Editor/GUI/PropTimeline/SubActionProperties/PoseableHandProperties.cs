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
            pinkyMeta = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("PinkyMetaBottom"), (EditBox)mainWidget.findWidget("PinkyMetaTop"));
            pinkyProx = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("PinkyProxTop"));
            pinkyInter = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("PinkyInterTop"));
            pinkyDistal = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("PinkyDistalTop"));

            ringMeta = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("RingMetaBottom"), (EditBox)mainWidget.findWidget("RingMetaTop"));
            ringProx = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("RingProxTop"));
            ringInter = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("RingInterTop"));
            ringDistal = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("RingDistalTop"));

            middleMeta = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("MiddleMetaBottom"), (EditBox)mainWidget.findWidget("MiddleMetaTop"));
            middleProx = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("MiddleProxTop"));
            middleInter = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("MiddleInterTop"));
            middleDistal = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("MiddleDistalTop"));

            indexMeta = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("IndexMetaBottom"), (EditBox)mainWidget.findWidget("IndexMetaTop"));
            indexProx = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("IndexProxTop"));
            indexInter = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("IndexInterTop"));
            indexDistal = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("IndexDistalTop"));

            thumbMeta = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("ThumbMetaBottom"), (EditBox)mainWidget.findWidget("ThumbMetaTop"));
            thumbProx = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("ThumbProxTop"));
            thumbDistal = new PoseableFingerSectionControl((EditBox)mainWidget.findWidget("ThumbDistalTop"));
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
