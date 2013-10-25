using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.SlideshowActions
{
    public class SetupSceneAction : SlideAction
    {
        private RunCommandsAction action;

        [DoNotSave]
        private EditInterface editInterface;

        public SetupSceneAction(String name)
        {
            action = new RunCommandsAction(name);
            Layers = true;
            Camera = true;
            Muscles = true;
            MedicalState = true;
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, "Setup Scene");
                editInterface.addCommand(new EditInterfaceCommand("Capture", (callback, caller) =>
                    {
                        captureSceneState(callback);
                        fireChangesMade();
                    }));
            }
            return editInterface;
        }

        public void captureSceneState(EditUICallback callback)
        {
            action.clear();
            if (Layers)
            {
                ChangeLayersCommand changeLayers = new ChangeLayersCommand();
                changeLayers.Layers.captureState();
                action.addCommand(changeLayers);
            }

            if (Muscles)
            {
                SetMusclePositionCommand musclePosition = new SetMusclePositionCommand();
                musclePosition.MusclePosition.captureState();
                action.addCommand(musclePosition);
            }

            if (Camera)
            {
                MoveCameraCommand moveCamera = new MoveCameraCommand();
                callback.runOneWayCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, moveCamera.CameraPosition);
                action.addCommand(moveCamera);
            }

            if (MedicalState)
            {
                ChangeMedicalStateCommand medicalState = new ChangeMedicalStateCommand();
                MedicalState medState = new MedicalState("");
                medState.update();
                medicalState.captureFromMedicalState(medState);
                action.addCommand(medicalState);
            }
        }

        public override void addToController(Slide slide, MvcController controller)
        {
            controller.Actions.add(action);
        }

        [Editable]
        public bool Layers { get; set; }

        [Editable]
        public bool Muscles { get; set; }

        [Editable]
        public bool Camera { get; set; }

        [Editable]
        public bool MedicalState { get; set; }

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
