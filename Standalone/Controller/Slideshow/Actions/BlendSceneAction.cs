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
        private bool highlightTeeth;

        private MoveBlendedCameraCommand cameraCommand;

        public BlendSceneAction(String name)
        {
            action = new RunCommandsAction(name);
            //Layers = true;
            Camera = true;
            //MusclePosition = true;
            //MedicalState = true;
            AllowPreview = true;
        }

        public void captureSceneToStartAndEnd(EditUICallback callback)
        {
            captureSceneStateTo(callback, () => cameraCommand.CameraStartPosition);
            captureSceneStateTo(callback, () => cameraCommand.CameraEndPosition);
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, "Setup Scene");
                editInterface.addCommand(new EditInterfaceCommand("Capture Start", callback =>
                {
                    captureSceneStateTo(callback, () => cameraCommand.CameraStartPosition);
                    fireChangesMade();
                }));
                editInterface.addCommand(new EditInterfaceCommand("Capture End", callback =>
                {
                    captureSceneStateTo(callback, () => cameraCommand.CameraEndPosition);
                    fireChangesMade();
                }));
            }
            return editInterface;
        }

        public void captureSceneStateTo(EditUICallback callback, Func<CameraPosition> getCameraPositionFunc)
        {
            //if (Layers)
            //{
            //    ChangeLayersCommand changeLayers = new ChangeLayersCommand();
            //    changeLayers.Layers.captureState();
            //    action.addCommand(changeLayers);
            //}

            //if (MusclePosition)
            //{
            //    SetMusclePositionCommand musclePosition = new SetMusclePositionCommand();
            //    musclePosition.MusclePosition.captureState();
            //    action.addCommand(musclePosition);
            //}

            if (Camera)
            {
                if(cameraCommand == null)
                {
                    cameraCommand = new MoveBlendedCameraCommand();
                    action.addCommand(cameraCommand);
                }

                callback.runOneWayCustomQuery(CameraPosition.CustomEditQueries.CaptureCameraPosition, getCameraPositionFunc());
            }
            else
            {
                if(cameraCommand != null)
                {
                    action.removeCommand(cameraCommand);
                    cameraCommand = null;
                }
            }

            //if (MedicalState)
            //{
            //    ChangeMedicalStateCommand medicalState = new ChangeMedicalStateCommand();
            //    MedicalState medState = new MedicalState("");
            //    medState.update();
            //    medicalState.captureFromMedicalState(medState);
            //    action.addCommand(medicalState);
            //}
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

        //[Editable(PrettyName = "Capture Layers")]
        //public bool Layers
        //{
        //    get
        //    {
        //        return layers;
        //    }
        //    set
        //    {
        //        layers = value;
        //    }
        //}

        //[Editable(PrettyName = "Capture Muscle Position")]
        //public bool MusclePosition
        //{
        //    get
        //    {
        //        return musclePosition;
        //    }
        //    set
        //    {
        //        musclePosition = value;
        //    }
        //}

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

        //[Editable(PrettyName = "Capture Medical State")]
        //public bool MedicalState
        //{
        //    get
        //    {
        //        return medicalState;
        //    }
        //    set
        //    {
        //        medicalState = value;
        //    }
        //}

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
