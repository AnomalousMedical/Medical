using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    public delegate void MedicalStateAdded(MedicalStateController controller, MedicalState state, int index);
    public delegate void MedicalStateRemoved(MedicalStateController controller, MedicalState state, int index);
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
        private int currentState = -1;
        //private ImageRenderer imageRenderer;
        //private ImageRendererProperties imageProperties;
        private MedicalController medicalController;

        private float blendLocation = 0.0f;
        private float blendSpeed = 1.0f;
        private float blendTarget = 0.0f;
        private bool playing = false;

        private bool directBlending = false;
        private MedicalState directStartState;
        private int directEndState;

        public MedicalStateController(/*ImageRenderer imageRenderer,*/ MedicalController medicalController)
        {
            //this.imageRenderer = imageRenderer;

            //imageProperties = new ImageRendererProperties();
            //imageProperties.Width = 100;
            //imageProperties.Height = 100;
            //imageProperties.UseWindowBackgroundColor = false;
            //imageProperties.CustomBackgroundColor = BACK_COLOR;
            //imageProperties.AntiAliasingMode = 2;
            //imageProperties.UseActiveViewportLocation = false;
            //imageProperties.UseNavigationStatePosition = true;
            //imageProperties.NavigationStateName = "Midline Anterior";
            //imageProperties.OverrideLayers = true;
            //imageProperties.LayerState = "MandibleSizeLayers";
            //imageProperties.TransparentBackground = true;
            //imageProperties.ShowBackground = false;
            //imageProperties.ShowWatermark = false;            

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

        public MedicalState createAndInsertState(int index, string name)
        {
            MedicalState state = createState(name);
            insertState(index, state);
            return state;
        }

        public MedicalState createState(String name)
        {
            MedicalState state = new MedicalState(name);
            state.update();
            return state;
        }

        public void addState(MedicalState state)
        {
            states.Add(state);
            //if (state.Thumbnail == null)
            //{
            //    state.Thumbnail = imageRenderer.renderImage(imageProperties);
            //}
            if (StateAdded != null)
            {
                StateAdded.Invoke(this, state, states.Count - 1);
            }
        }

        public void insertState(int index, MedicalState state)
        {
            if (index < states.Count)
            {
                states.Insert(index, state);
            }
            else
            {
                states.Add(state);
                index = states.Count - 1;
            }
            if (StateAdded != null)
            {
                StateAdded.Invoke(this, state, index);
            }
        }

        public void destroyState(MedicalState state)
        {
            int index = states.IndexOf(state);
            states.RemoveAt(index);
            if (StateRemoved != null)
            {
                StateRemoved.Invoke(this, state, index);
            }
            state.Dispose();
        }

        public void destroyState(int index)
        {
            destroyState(states[index]);
        }

        public void clearStates()
        {
            currentState = -1;
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
        public void createNormalStateFromScene()
        {
            MedicalState normalState = this.createAndAddState("Normal");
            normalState.Notes.Notes = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}
\viewkind4\uc1\pard\f0\fs17 Normal\par
}";
            normalState.Notes.DataSource = "";
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
            if (directBlending)
            {
                if (percent >= 1.0f)
                {
                    states[directEndState].blend(0, states[directEndState]);
                    blendLocation = directEndState;
                    directBlending = false;
                    if (currentState != directEndState)
                    {
                        currentState = directEndState;
                        if (StateChanged != null)
                        {
                            StateChanged.Invoke(states[directEndState]);
                        }
                    }
                }
                else
                {
                    directStartState.blend(percent, states[directEndState]);
                    blendLocation = percent;
                }
            }
            else
            {
                int startState = (int)percent;
                int endState = startState + 1;
                if (endState < states.Count)
                {
                    states[startState].blend(percent - startState, states[endState]);
                }
                //Be sure to blend if on the exact frame of the last state.
                else if (startState == states.Count - 1 && startState >= 0)
                {
                    states[startState].blend(1.0f, states[startState]);
                }
                if (startState != currentState)
                {
                    currentState = startState;
                    if (StateChanged != null)
                    {
                        StateChanged.Invoke(states[startState]);
                    }
                }
                blendLocation = percent;
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
                addState(state);
            }
        }

        public void blendTo(int index, float speed)
        {
            directBlending = false;
            if (index > blendTarget)
            {
                blendSpeed = speed;
            }
            else
            {
                blendSpeed = -speed;
            }
            blendTarget = index;
            startPlayback();
        }

        public void directBlend(int endIndex, float speed)
        {
            directBlending = true;
            this.directStartState = createState("DirectStart");
            this.directEndState = endIndex;
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

        public MedicalState CurrentState
        {
            get
            {
                if (currentState != -1)
                {
                    return states[currentState];
                }
                return null;
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
