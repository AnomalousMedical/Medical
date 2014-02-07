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
            HighlightTeeth = true;
        }

        public SetupSceneAction(String name, CameraPosition cameraPosition, LayerState layers, MusclePosition musclePosition, PresetState medicalState, bool captureHighlight, bool isHighlighted)
        {
            action = new RunCommandsAction(name);
            Layers = layers != null;
            Camera = cameraPosition != null;
            Muscles = musclePosition != null;
            MedicalState = medicalState != null;
            HighlightTeeth = captureHighlight;

            if (Layers)
            {
                ChangeLayersCommand changeLayersCommand = new ChangeLayersCommand();
                changeLayersCommand.Layers.copyFrom(layers);
                action.addCommand(changeLayersCommand);
            }

            if (Muscles)
            {
                SetMusclePositionCommand musclePositionCommand = new SetMusclePositionCommand();
                musclePositionCommand.MusclePosition = musclePosition;
                action.addCommand(musclePositionCommand);
            }

            if (Camera)
            {
                MoveCameraCommand moveCameraCommand = new MoveCameraCommand();
                moveCameraCommand.CameraPosition = cameraPosition;
                action.addCommand(moveCameraCommand);
            }

            if (MedicalState)
            {
                ChangeMedicalStateCommand medicalStateCommand = new ChangeMedicalStateCommand();
                medicalStateCommand.PresetState = medicalState;
                action.addCommand(medicalStateCommand);
            }

            if (HighlightTeeth)
            {
                action.addCommand(new ChangeTeethHighlightsCommand(isHighlighted));
            }
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

            if (HighlightTeeth)
            {
                action.addCommand(new ChangeTeethHighlightsCommand(TeethController.HighlightContacts));
            }
        }

        public override void addToController(Slide slide, MvcController controller)
        {
            RunCommandsAction setupSceneAction = new RunCommandsAction(action.Name);
            setupAction(slide, setupSceneAction);
            controller.Actions.add(setupSceneAction);
        }

        public override void setupAction(Slide slide, RunCommandsAction action)
        {
            RunCommandsAction clone = CopySaver.Default.copy(this.action);
            foreach (var command in clone.Commands)
            {
                action.addCommand(command);
            }
            clone.clear();
        }

        public override void configureThumbnailProperties(ImageRendererProperties imageProperties)
        {
            base.configureThumbnailProperties(imageProperties);
            foreach (var command in action.Commands)
            {
                if (command is MoveCameraCommand)
                {
                    MoveCameraCommand moveCamera = (MoveCameraCommand)command;
                    imageProperties.CameraPosition = moveCamera.CameraPosition.Translation;
                    imageProperties.CameraLookAt = moveCamera.CameraPosition.LookAt;
                    imageProperties.UseIncludePoint = moveCamera.CameraPosition.UseIncludePoint;
                    imageProperties.IncludePoint = moveCamera.CameraPosition.IncludePoint;
                }
                if (command is ChangeLayersCommand)
                {
                    ChangeLayersCommand changeLayers = (ChangeLayersCommand)command;
                    imageProperties.OverrideLayers = true;
                    imageProperties.LayerState = changeLayers.Layers;
                }
            }
        }

        [Editable]
        public bool Layers { get; set; }

        [Editable]
        public bool Muscles { get; set; }

        [Editable]
        public bool Camera { get; set; }

        [Editable]
        public bool MedicalState { get; set; }

        [Editable]
        public bool HighlightTeeth { get; set; }

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
