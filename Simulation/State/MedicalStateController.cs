using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void MedicalStateAdded(MedicalStateController controller, MedicalState state, int index);
    public delegate void MedicalStateRemoved(MedicalStateController controller, MedicalState state, int index);
    public delegate void MedicalStatesCleared(MedicalStateController controller);
    public delegate void MedicalStateChanged(MedicalState state);

    public class MedicalStateController
    {
        public event MedicalStateAdded StateAdded;
        public event MedicalStateRemoved StateRemoved;
        public event MedicalStatesCleared StatesCleared;
        public event MedicalStateChanged StateChanged;

        private List<MedicalState> states = new List<MedicalState>();
        private int currentState = -1;

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

        public void removeState(MedicalState state)
        {
            int index = states.IndexOf(state);
            states.RemoveAt(index);
            if (StateRemoved != null)
            {
                StateRemoved.Invoke(this, state, index);
            }
        }

        public void clearStates()
        {
            currentState = -1;
            states.Clear();
            if (StatesCleared != null)
            {
                StatesCleared.Invoke(this);
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
            normalState.Notes.DataSource = "Articulometrics";
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
    }
}
