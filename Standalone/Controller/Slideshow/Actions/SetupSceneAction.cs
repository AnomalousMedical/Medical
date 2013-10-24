using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SetupSceneAction : SlideAction
    {
        private RunCommandsAction action;
        private EditInterface editInterface;

        public SetupSceneAction(String name)
        {
            action = new RunCommandsAction(name);
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Setup Scene");
                editInterface.addCommand(new EditInterfaceCommand("Capture", (callback, caller) =>
                    {
                        action.clear();
                        ChangeLayersCommand changeLayers = new ChangeLayersCommand();
                        changeLayers.Layers.captureState();
                        action.addCommand(changeLayers);

                        SetMusclePositionCommand musclePosition = new SetMusclePositionCommand();
                        musclePosition.MusclePosition.captureState();
                        action.addCommand(musclePosition);

                        MoveCameraCommand moveCamera = new MoveCameraCommand();
                        callback.runOneWayCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, moveCamera.CameraPosition);
                        action.addCommand(moveCamera);

                        ChangeMedicalStateCommand medicalState = new ChangeMedicalStateCommand();
                        MedicalState medState = new MedicalState("");
                        medState.update();
                        medicalState.captureFromMedicalState(medState);
                        action.addCommand(medicalState);
                    }));
            }
            return editInterface;
        }

        public override void addToController(MvcController controller)
        {
            controller.Actions.add(action);
        }

        public override string Name
        {
            get
            {
                return action.Name;
            }
            set
            {
                action.Name = value;
            }
        }

        protected SetupSceneAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
