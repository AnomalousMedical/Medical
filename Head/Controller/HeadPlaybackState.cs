using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    class HeadPlaybackState : MedicalState
    {
        private BoneManipulatorState boneState;

        public HeadPlaybackState(String name)
            :base(name)
        {

        }

        public override void blend(float percent, MedicalState target)
        {
            HeadPlaybackState headState = target as HeadPlaybackState;
            boneState.blend(headState.boneState, percent);
        }

        public override void update()
        {
            boneState = BoneManipulatorController.createBoneManipulatorState();
        }
    }
}
