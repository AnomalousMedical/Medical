using Medical;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture
{
    class SlideSceneInfo : IDisposable
    {
        private CameraPosition cameraPosition;
        private LayerState layers;
        private MusclePosition musclePosition;
        private PresetState medicalState;
        private SceneThumbInfo sceneThumbInfo = null;

        public SlideSceneInfo()
        {

        }

        public SlideSceneInfo(MedicalRmlSlide slide, SceneThumbInfo sceneThumbInfo)
        {
            this.CameraPosition = slide.CameraPosition;
            this.Layers = slide.Layers;
            this.MedicalState = slide.MedicalState;
            this.MusclePosition = slide.MusclePosition;
            this.sceneThumbInfo = sceneThumbInfo;
        }

        public void Dispose()
        {
            if (sceneThumbInfo != null)
            {
                sceneThumbInfo.Dispose();
                sceneThumbInfo = null;
            }
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

        /// <summary>
        /// Set the scene thumb info for this class. The object will be taken over by this class
        /// and will be disposed when it is or when the SceneThumbInfo is changed with this property.
        /// You should make copies of stuff you put in here (if applicable) and take out (if applicable).
        /// </summary>
        public SceneThumbInfo SceneThumbInfo
        {
            get
            {
                return sceneThumbInfo;
            }
            set
            {
                if (sceneThumbInfo != null)
                {
                    sceneThumbInfo.Dispose();
                }
                sceneThumbInfo = value;
            }
        }
    }
}
