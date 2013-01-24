using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture
{
    class SlideSceneInfo
    {
        private CameraPosition cameraPosition;
        private LayerState layers;
        private MusclePosition musclePosition;
        private PresetState medicalState;

        public SlideSceneInfo()
        {

        }

        public SlideSceneInfo(MedicalRmlSlide slide)
        {
            this.CameraPosition = slide.CameraPosition;
            this.Layers = slide.Layers;
            this.MedicalState = slide.MedicalState;
            this.MusclePosition = slide.MusclePosition;
        }

        public void applyToSlide(MedicalRmlSlide slide)
        {
            slide.CameraPosition = this.CameraPosition;
            slide.Layers = this.Layers;
            slide.MedicalState = this.MedicalState;
            slide.MusclePosition = this.MusclePosition;
        }

        public CameraPosition CameraPosition
        {
            get
            {
                return cameraPosition;
            }
            set
            {
                cameraPosition = value;
            }
        }

        public LayerState Layers
        {
            get
            {
                return layers;
            }
            set
            {
                layers = value;
            }
        }

        public MusclePosition MusclePosition
        {
            get
            {
                return musclePosition;
            }
            set
            {
                musclePosition = value;
            }
        }

        public PresetState MedicalState
        {
            get
            {
                return medicalState;
            }
            set
            {
                medicalState = value;
            }
        }
    }
}
