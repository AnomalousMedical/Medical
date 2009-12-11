using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Medical
{
    public class MedicalState : Saveable, IDisposable
    {
        private AnimationManipulatorState boneState;
        private DiscState discState;
        private TeethState teethState;
        private FossaState fossaState;
        private MedicalStateNotes notes;
        private Bitmap thumbnail;

        public MedicalState(String name)
        {
            Name = name;
            boneState = new AnimationManipulatorState();
            discState = new DiscState();
            teethState = new TeethState();
            fossaState = new FossaState();
            notes = new MedicalStateNotes();
        }

        public void Dispose()
        {
            if (thumbnail != null)
            {
                thumbnail.Dispose();
            }
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
            boneState = AnimationManipulatorController.createAnimationManipulatorState();
            discState = DiscController.createDiscState();
            teethState = TeethController.createTeethState();
            fossaState = FossaController.createState();
        }

        public String Name { get; set; }

        public AnimationManipulatorState BoneManipulator
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

        /// <summary>
        /// Set the thumbnail for this state. This data is not copied and will
        /// become managed by this class. Namely it will be disposed when this
        /// state is disposed. If a copy of the image is needed otherwise you
        /// must copy it manually.
        /// </summary>
        public Bitmap Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                }
                thumbnail = value;
            }
        }

        #region Saveable Members

        private const string BONE_MANIPULATOR_STATE = "BoneManipulatorState";
        private const string DISC_STATE = "DiscState";
        private const string TEETH_STATE = "TeethState";
        private const string FOSSA_STATE = "FossaState";
        private const string NOTES = "Notes";
        private const string THUMBNAIL = "Thumbnail";
        private const string NAME = "Name";

        protected MedicalState(LoadInfo info)
        {
            boneState = info.GetValue<AnimationManipulatorState>(BONE_MANIPULATOR_STATE);
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
            if (info.hasValue(THUMBNAIL))
            {
                using (MemoryStream memStream = new MemoryStream(info.GetBlob(THUMBNAIL)))
                {
                    thumbnail = new Bitmap(memStream);
                    memStream.Close();
                }
            }
            else
            {
                thumbnail = null;
            }
            if (info.hasValue(NAME))
            {
                Name = info.GetString(NAME);
            }
            else
            {
                Name = "Unnamed";
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(BONE_MANIPULATOR_STATE, boneState);
            info.AddValue(DISC_STATE, discState);
            info.AddValue(TEETH_STATE, teethState);
            info.AddValue(FOSSA_STATE, fossaState);
            info.AddValue(NOTES, notes);
            info.AddValue(NAME, Name);
            if (thumbnail != null)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    thumbnail.Save(memStream, ImageFormat.Png);
                    info.AddValue(THUMBNAIL, memStream.GetBuffer());
                    memStream.Close();
                }
            }
        }

        #endregion
    }
}
