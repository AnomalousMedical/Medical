using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class PoseableHandProperties : PropertiesFormLayoutComponent
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

        public PoseableHandProperties(EditableProperty property, Widget parentWidget, String layoutFile = "Medical.GUI.Editor.PropTimeline.SubActionProperties.PoseableLeftHandProperties.layout")
            :base(property, parentWidget, layoutFile)
        {
            widget.ForwardMouseWheelToParent = true;

            pinkyMeta = new PoseableFingerSectionControl((EditBox)widget.findWidget("PinkyMetaBottom"), (EditBox)widget.findWidget("PinkyMetaTop"));
            pinkyProx = new PoseableFingerSectionControl((EditBox)widget.findWidget("PinkyProxTop"));
            pinkyInter = new PoseableFingerSectionControl((EditBox)widget.findWidget("PinkyInterTop"));
            pinkyDistal = new PoseableFingerSectionControl((EditBox)widget.findWidget("PinkyDistalTop"));

            ringMeta = new PoseableFingerSectionControl((EditBox)widget.findWidget("RingMetaBottom"), (EditBox)widget.findWidget("RingMetaTop"));
            ringProx = new PoseableFingerSectionControl((EditBox)widget.findWidget("RingProxTop"));
            ringInter = new PoseableFingerSectionControl((EditBox)widget.findWidget("RingInterTop"));
            ringDistal = new PoseableFingerSectionControl((EditBox)widget.findWidget("RingDistalTop"));

            middleMeta = new PoseableFingerSectionControl((EditBox)widget.findWidget("MiddleMetaBottom"), (EditBox)widget.findWidget("MiddleMetaTop"));
            middleProx = new PoseableFingerSectionControl((EditBox)widget.findWidget("MiddleProxTop"));
            middleInter = new PoseableFingerSectionControl((EditBox)widget.findWidget("MiddleInterTop"));
            middleDistal = new PoseableFingerSectionControl((EditBox)widget.findWidget("MiddleDistalTop"));

            indexMeta = new PoseableFingerSectionControl((EditBox)widget.findWidget("IndexMetaBottom"), (EditBox)widget.findWidget("IndexMetaTop"));
            indexProx = new PoseableFingerSectionControl((EditBox)widget.findWidget("IndexProxTop"));
            indexInter = new PoseableFingerSectionControl((EditBox)widget.findWidget("IndexInterTop"));
            indexDistal = new PoseableFingerSectionControl((EditBox)widget.findWidget("IndexDistalTop"));

            thumbMeta = new PoseableFingerSectionControl((EditBox)widget.findWidget("ThumbMetaBottom"), (EditBox)widget.findWidget("ThumbMetaTop"));
            thumbProx = new PoseableFingerSectionControl((EditBox)widget.findWidget("ThumbProxTop"));
            thumbDistal = new PoseableFingerSectionControl((EditBox)widget.findWidget("ThumbDistalTop"));

            refreshData();
        }

        public override void refreshData()
        {
            setCurrentData((ChangeHandPosition)Property.getRealValue(1));
        }

        public void setCurrentData(ChangeHandPosition handPosition)
        {
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
