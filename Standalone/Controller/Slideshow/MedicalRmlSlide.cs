using Engine.Saving;
using Medical.Controller.AnomalousMvc;
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

        protected override void customizeController(MvcController controller, RunCommandsAction showCommand)
        {
            populateCommand(showCommand);
        }

        public void populateCommand(RunCommandsAction action)
        {
            action.addCommand(new MoveCameraCommand()
            {
                CameraPosition = this.CameraPosition
            });
            ChangeLayersCommand layers = new ChangeLayersCommand();
            layers.Layers.copyFrom(Layers);
            action.addCommand(layers);
            action.addCommand(new ChangeMedicalStateCommand()
            {
                PresetState = this.MedicalState
            });
            action.addCommand(new SetMusclePositionCommand()
            {
                MusclePosition = this.MusclePosition
            });
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
