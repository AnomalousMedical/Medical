using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void MedicalStateAdded(MedicalStateController controller, MedicalState state, int index);
    public delegate void MedicalStateRemoved(MedicalStateController controller, MedicalState state, int index);
    public delegate void MedicalStatesCleared(MedicalStateController controller);

    public class MedicalStateController
    {
        public event MedicalStateAdded StateAdded;
        public event MedicalStateRemoved StateRemoved;
        public event MedicalStatesCleared StatesCleared;

        private List<MedicalState> states = new List<MedicalState>();

        public MedicalState createState(String name)
        {
            MedicalState state = new MedicalState(name);
            state.update();
            addState(state);
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
            else if (startState == states.Count - 1 && startState > 0)
            {
                states[startState].blend(1.0f, states[startState]);
            }
        }
    }
}
