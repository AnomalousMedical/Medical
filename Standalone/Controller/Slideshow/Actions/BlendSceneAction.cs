using Anomalous.GuiFramework.Cameras;
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
    public class BlendSceneAction : SlideAction
    {
        private RunCommandsAction action;

        [DoNotSave]
        private EditInterface editInterface;

        private bool layers;
        private bool musclePosition;
        private bool camera;
        private bool medicalState;

        private MoveBlendedCameraCommand cameraCommand;
        private BlendMedicalStateCommand medicalStateCommand;
        private BlendLayersCommand layersCommand;
        private BlendMusclePositionCommand muscleCommand;

        public BlendSceneAction(String name)
        {
            action = new RunCommandsAction(name);
            Layers = true;
            Camera = true;
            MusclePosition = true;
            MedicalState = true;
            AllowPreview = true;
        }

        public void captureSceneToStartAndEnd(EditUICallback callback)
        {
            captureStartState(callback);
            captureEndState(callback);
        }

        public void captureStartState(EditUICallback callback)
        {
            captureSceneStateTo(callback, 
                () => layersCommand.StartLayers.captureState(), 
                () => muscleCommand.StartPosition.captureState(), 
                camPos => cameraCommand.CameraStartPosition = camPos, 
                state => medicalStateCommand.captureStartFromMedicalState(state));
        }

        public void captureEndState(EditUICallback callback)
        {
            captureSceneStateTo(callback, 
                () => layersCommand.EndLayers.captureState(), 
                () => muscleCommand.EndPosition.captureState(), 
                camPos => cameraCommand.CameraEndPosition = camPos, 
                state => medicalStateCommand.captureEndFromMedicalState(state));
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, "Setup Scene");
                editInterface.addCommand(new EditInterfaceCommand("Capture Start", callback =>
                {
                    captureStartState(callback);
                    fireChangesMade();
                }));
                editInterface.addCommand(new EditInterfaceCommand("Capture End", callback =>
                {
                    captureEndState(callback);
                    fireChangesMade();
                }));
            }
            return editInterface;
        }

        public void captureSceneStateTo(EditUICallback callback, Action setLayerState, Action setMusclePosition, Action<CameraPosition> setCameraPosition, Action<MedicalState> setMedicalState)
        {
            if (Layers)
            {
                if (layersCommand == null)
                {
                    layersCommand = new BlendLayersCommand();
                    action.addCommand(layersCommand);
                }

                setLayerState();
            }
            else if (layersCommand != null)
            {
                action.removeCommand(layersCommand);
                layersCommand = null;
            }

            if (MusclePosition)
            {
                if (muscleCommand == null)
                {
                    muscleCommand = new BlendMusclePositionCommand();
                    action.addCommand(muscleCommand);
                }

                setMusclePosition();
            }
            else if (muscleCommand != null)
            {
                action.removeCommand(muscleCommand);
                muscleCommand = null;
            }

            if (Camera)
            {
                if (cameraCommand == null)
                {
                    cameraCommand = new MoveBlendedCameraCommand();
                    action.addCommand(cameraCommand);
                }

                CameraPosition camPos = new CameraPosition();
                callback.runOneWayCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, camPos);
                setCameraPosition(camPos);
            }
            else if (cameraCommand != null)
            {
                action.removeCommand(cameraCommand);
                cameraCommand = null;
            }

            if (MedicalState)
            {
                if (medicalStateCommand == null)
                {
                    medicalStateCommand = new BlendMedicalStateCommand();
                    action.addCommand(medicalStateCommand);
                }

                MedicalState medState = new MedicalState("");
                medState.update();
                setMedicalState(medState);
            }
            else if (medicalStateCommand != null)
            {
                action.removeCommand(medicalStateCommand);
                medicalStateCommand = null;
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

        [Editable(PrettyName = "Capture Layers")]
        public bool Layers
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

        [Editable(PrettyName = "Capture Muscle Position")]
        public bool MusclePosition
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

        [Editable(PrettyName = "Capture Camera Position")]
        public bool Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        [Editable(PrettyName = "Capture Medical State")]
        public bool MedicalState
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

        protected BlendSceneAction(LoadInfo info)
            : base(info)
        {
            AllowPreview = true;
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }
    }
}
