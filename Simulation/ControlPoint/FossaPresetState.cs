using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class FossaPresetState : PresetState
    {
        private String fossaName;
        private float position;

        public FossaPresetState(String name, String category, String imageName)
            : base(name, category, imageName)
        {

        }

        public void captureFromState(String fossaName, FossaState state)
        {
            this.fossaName = fossaName;
            position = state.getPosition(fossaName);
        }

        public override void applyToState(MedicalState state)
        {
            state.Fossa.addPosition(fossaName, position);
        }

        public void changeSide(string oldName, string newName)
        {
            fossaName = fossaName.Replace(oldName, newName);
        }

        public String FossaName
        {
            get
            {
                return fossaName;
            }
            set
            {
                fossaName = value;
            }
        }

        public float Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        #region Saveable

        private const String FOSSA_NAME = "FossaName";
        private const String POSITION = "Position";

        protected FossaPresetState(LoadInfo info)
            :base(info)
        {
            fossaName = info.GetString(FOSSA_NAME);
            position = info.GetFloat(POSITION);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(FOSSA_NAME, fossaName);
            info.AddValue(POSITION, position);
        }

        #endregion Saveable
    }
}
