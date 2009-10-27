using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class MedicalState : Saveable
    {
        private BoneManipulatorState boneState;
        private DiscState discState;
        private TeethState teethState;
        private FossaState fossaState;
        private MedicalStateNotes notes;

        public MedicalState(String name)
        {
            Name = name;
            boneState = new BoneManipulatorState();
            discState = new DiscState();
            teethState = new TeethState();
            fossaState = new FossaState();
            notes = new MedicalStateNotes();
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
        }

        public DiscState Disc
        {
            get
            {
                return discState;
            }
        }

        public TeethState Teeth
        {
            get
            {
                return teethState;
            }
        }

        public FossaState Fossa
        {
            get
            {
                return fossaState;
            }
        }

        public MedicalStateNotes Notes
        {
            get
            {
                return notes;
            }
        }

        #region Saveable Members

        private const string BONE_MANIPULATOR_STATE = "BoneManipulatorState";
        private const string DISC_STATE = "DiscState";
        private const string TEETH_STATE = "TeethState";
        private const string FOSSA_STATE = "FossaState";
        private const string NOTES = "Notes";

        protected MedicalState(LoadInfo info)
        {
            boneState = info.GetValue<BoneManipulatorState>(BONE_MANIPULATOR_STATE);
            discState = info.GetValue<DiscState>(DISC_STATE);
            teethState = info.GetValue<TeethState>(TEETH_STATE);
            fossaState = info.GetValue<FossaState>(FOSSA_STATE);
            if (info.hasValue(NOTES))
            {
                notes = info.GetValue<MedicalStateNotes>(NOTES);
            }
            else
            {
                notes = new MedicalStateNotes();
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(BONE_MANIPULATOR_STATE, boneState);
            info.AddValue(DISC_STATE, discState);
            info.AddValue(TEETH_STATE, teethState);
            info.AddValue(FOSSA_STATE, fossaState);
            info.AddValue(NOTES, notes);
        }

        #endregion
    }
}
