using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MedicalRmlSlide : RmlSlide
    {
        private CameraPosition cameraPosition;
        private LayerState layers;
        private MusclePosition musclePosition;
        private PresetState medicalState;

        public MedicalRmlSlide()
        {

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

        protected MedicalRmlSlide(LoadInfo info)
            :base(info)
        {

        }
    }
}
