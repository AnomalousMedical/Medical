using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MedicalState
    {
        private BoneManipulatorState boneState;
        private DiscState discState;
        private TeethState teethState;
        private FossaState fossaState;

        public MedicalState(String name)
        {
            Name = name;
        }

        public void blend(float percent, MedicalState target)
        {
            boneState.blend(target.boneState, percent);
            discState.blend(target.discState, percent);
            teethState.blend(target.teethState, percent);
            fossaState.blend(target.fossaState, percent);
        }

        public void update()
        {
            boneState = BoneManipulatorController.createBoneManipulatorState();
            discState = DiscController.createDiscState();
            teethState = TeethController.createTeethState();
            fossaState = FossaController.createState();
        }

        public String Name { get; set; }

        public BoneManipulatorState BoneManipulator
        {
            get
            {
                return boneState;
            }
            set
            {
                boneState = value;
            }
        }

        public DiscState Disc
        {
            get
            {
                return discState;
            }
            set
            {
                discState = value;
            }
        }

        public TeethState Teeth
        {
            get
            {
                return teethState;
            }
            set
            {
                teethState = value;
            }
        }

        public FossaState Fossa
        {
            get
            {
                return fossaState;
            }
            set
            {
                fossaState = value;
            }
        }
    }
}
