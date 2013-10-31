using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.ObjectManagement;

namespace Medical
{
    public delegate void MedicalStateAdded(MedicalStateController controller, MedicalState state);
    public delegate void MedicalStateRemoved(MedicalStateController controller, MedicalState state);
    public delegate void MedicalStateEvent(MedicalStateController controller);
    public delegate void MedicalStateStatusUpdate(MedicalState state);

    public class MedicalStateController : IDisposable
    {
        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);

        public event MedicalStateAdded StateAdded;
        public event MedicalStateRemoved StateRemoved;
        public event MedicalStateEvent StatesCleared;
        public event MedicalStateStatusUpdate StateChanged;
        public event MedicalStateEvent BlendingStarted;
        public event MedicalStateEvent BlendingStopped;
        public event MedicalStateStatusUpdate StateUpdated;

        private List<MedicalState> states = new List<MedicalState>();
        private ImageRenderer imageRenderer;
        private ImageRendererProperties imageProperties;
        private MedicalController medicalController;

        private float blendLocation = 0.0f;
        private float blendSpeed = 1.0f;
        private float blendTarget = 0.0f;
        private bool playing = false;

        private MedicalState directStartState;
        private MedicalState directEndState;

        private MedicalState sceneLoadNormalState;

        public MedicalStateController(ImageRenderer imageRenderer, MedicalController medicalController)
        {
            this.imageRenderer = imageRenderer;

            imageProperties = new ImageRendererProperties();
            imageProperties.Width = 100;
            imageProperties.Height = 100;
            imageProperties.UseWindowBackgroundColor = false;
            imageProperties.CustomBackgroundColor = BACK_COLOR;
            imageProperties.AntiAliasingMode = 2;
            imageProperties.UseActiveViewportLocation = true;
            imageProperties.OverrideLayers = false;
            imageProperties.TransparentBackground = true;
            imageProperties.ShowBackground = false;
            imageProperties.ShowWatermark = false;
            imageProperties.ShowUIUpdates = false;

            this.medicalController = medicalController;
        }

        public void Dispose()
        {
            clearStates();
        }

        public MedicalState createAndAddState(String name)
        {
            MedicalState state = createState(name);
            addState(state);
            return state;
        }

        public MedicalState createState(String name)
        {
            MedicalState state = new MedicalState(name);
            state.update();
            return state;
        }

        public PresetState createPresetState(String name)
        {
            MedicalState medicalState = new MedicalState(name);
            medicalState.update();
            CompoundPresetState compoundPresetState = new CompoundPresetState("", "", "");

            DiscPresetState leftDiscPreset = new DiscPresetState("LeftTMJDisc", "", "", "");
            leftDiscPreset.captureFromState(medicalState.Disc.getPosition("LeftTMJDisc"));
            compoundPresetState.addSubState(leftDiscPreset);

            DiscPresetState rightDiscPreset = new DiscPresetState("RightTMJDisc", "", "", "");
            rightDiscPreset.captureFromState(medicalState.Disc.getPosition("RightTMJDisc"));
            compoundPresetState.addSubState(rightDiscPreset);
            
            FossaPresetState leftFossaPreset = new FossaPresetState("", "", "");
            leftFossaPreset.captureFromState("LeftFossa", medicalState.Fossa);
            compoundPresetState.addSubState(leftFossaPreset);
            
            FossaPresetState rightFossaPreset = new FossaPresetState("", "", "");
            rightFossaPreset.captureFromState("RightFossa", medicalState.Fossa);
            compoundPresetState.addSubState(rightFossaPreset);
            
            AnimationManipulatorPresetState animationManipPresetState = new AnimationManipulatorPresetState("", "", "");
            animationManipPresetState.captureFromState(medicalState.BoneManipulator);
            compoundPresetState.addSubState(animationManipPresetState);
            
            TeethPresetState teethPreset = new TeethPresetState("", "", "");
            teethPreset.captureFromState(medicalState.Teeth);
            compoundPresetState.addSubState(teethPreset);

            return compoundPresetState;
        }

        public void addState(MedicalState state)
        {
            //No states and normal state defined, add it as the first state.
            if (states.Count == 0)
            {
                states.Add(sceneLoadNormalState);
                if (StateAdded != null)
                {
                    StateAdded.Invoke(this, sceneLoadNormalState);
                }
            }
            states.Add(state);
            if (state.Thumbnail == null)
            {
                state.Thumbnail = imageRenderer.renderImage(imageProperties);
            }
            if (StateAdded != null)
            {
                StateAdded.Invoke(this, state);
            }
        }

        public void destroyState(MedicalState state)
        {
            stopBlending();
            states.Remove(state);
            if (StateRemoved != null)
            {
                StateRemoved.Invoke(this, state);
            }
            state.Dispose();
        }

        public void clearStates()
        {
            foreach (MedicalState state in states)
            {
                state.Dispose();
            }
            states.Clear();
            if (StatesCleared != null)
            {
                StatesCleared.Invoke(this);
            }
        }

        public void alertStateUpdated(MedicalState state)
        {
            if (StateUpdated != null && states.Contains(state))
            {
                StateUpdated.Invoke(state);
            }
        }

        public int getNumStates()
        {
            return states.Count;
        }

        /// <summary>
        /// Use the current setup of the scene to create a "normal" state.
        /// </summary>
        public void sceneLoaded(SimScene scene)
        {
            sceneLoadNormalState = this.createState("Normal");
            sceneLoadNormalState.Notes.Notes = "Normal";
            sceneLoadNormalState.Notes.DataSource = "Automatic";
            sceneLoadNormalState.Thumbnail = imageRenderer.renderImage(imageProperties);
            states.Add(sceneLoadNormalState);
            if (StateAdded != null)
            {
                StateAdded.Invoke(this, sceneLoadNormalState);
            }
        }

        public void sceneUnloading(SimScene scene)
        {
            if (sceneLoadNormalState != null)
            {
                states.Remove(sceneLoadNormalState);
                if (StateRemoved != null)
                {
                    StateRemoved.Invoke(this, sceneLoadNormalState);
                }
                sceneLoadNormalState.Dispose();
                sceneLoadNormalState = null;
            }
        }

        /// <summary>
        /// Blend between two states. The whole number part determines the start
        /// state, the whole number + 1 is the destination state. The partial
        /// part is the percentage of blend between the two states. So 2.3 will
        /// blend states 2 and 3 30% of the way from state 2 to 3.
        /// </summary>
        /// <param name="percent">The index and percentage to blend.</param>
        public void blend(float percent)
        {
            if (percent >= 1.0f)
            {
                directEndState.blend(0, directEndState);
                if (StateChanged != null)
                {
                    StateChanged.Invoke(directEndState);
                }
            }
            else
            {
                if (directStartState != null)
                {
                    directStartState.blend(percent, directEndState);
                    blendLocation = percent;
                }
            }
        }

        public SavedMedicalStates getSavedState(String currentSceneName)
        {
            return new SavedMedicalStates(states, currentSceneName);
        }

        public void setStates(SavedMedicalStates states)
        {
            clearStates();
            foreach (MedicalState state in states.getStates())
            {
                this.states.Add(state);
                if (StateAdded != null)
                {
                    StateAdded.Invoke(this, state);
                }
            }
        }

        public void directBlend(MedicalState state, float speed)
        {
            this.directStartState = createState("DirectStart");
            this.directEndState = state;
            blendLocation = 0.0f;
            blendTarget = 1.0f;
            startPlayback();
        }

        public void stopBlending()
        {
            if (playing)
            {
                medicalController.FixedLoopUpdate -= medicalController_FixedLoopUpdate;
                playing = false;
                if (BlendingStopped != null)
                {
                    BlendingStopped.Invoke(this);
                }
            }
        }

        public IEnumerable<MedicalState> States
        {
            get
            {
                return states;
            }
        }

        void medicalController_FixedLoopUpdate(Clock time)
        {
            double nextTime = blendLocation + time.Seconds * blendSpeed;
            if (blendSpeed > 0)
            {
                if (nextTime > blendTarget)
                {
                    nextTime = blendTarget;
                    stopBlending();
                }
            }
            else if (blendSpeed < 0)
            {
                if (nextTime < blendTarget)
                {
                    nextTime = blendTarget;
                    stopBlending();
                }
            }
            blend((float)nextTime);
        }

        private void startPlayback()
        {
            if (!playing)
            {
                medicalController.FixedLoopUpdate += medicalController_FixedLoopUpdate;
                playing = true;
                if (BlendingStarted != null)
                {
                    BlendingStarted.Invoke(this);
                }
            }
        }
    }
}
