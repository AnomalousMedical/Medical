using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Muscles
{
    public class MovementSequence
    {
        String name;
        private List<MovementSequenceState> states = new List<MovementSequenceState>();
        private float duration;
        private int currentIndex = 0;

        public MovementSequence(String name)
        {
            this.name = name;
        }

        public void addState(MovementSequenceState state)
        {
            if (states.Count > 0)
            {
                MovementSequenceState previous = states[states.Count - 1];
                state.TimeIndex = previous.TimeIndex + previous.Duration;
            }
            else
            {
                state.TimeIndex = 0.0f;
            }
            state.Index = states.Count;
            duration += state.Duration;
            states.Add(state);
        }

        public void setPosition(float time)
        {
            if (states.Count > 0)
            {
                time %= duration;
                MovementSequenceState currentState = states[currentIndex];
                if (!currentState.isTimePart(time))
                {
                    currentState = findStateForTime(time);
                }
                currentState.blend(findNextState(currentState), time);
            }
        }

        private MovementSequenceState findStateForTime(float time)
        {
            foreach (MovementSequenceState state in states)
            {
                if (state.isTimePart(time))
                {
                    return state;
                }
            }
            return states[0];
        }

        private MovementSequenceState findNextState(MovementSequenceState currentState)
        {
            if (currentState.Index + 1 < states.Count)
            {
                return states[currentState.Index + 1];
            }
            else
            {
                return states[0];
            }
        }

        public int NumStates
        {
            get
            {
                return states.Count;
            }
        }

        public float Duration
        {
            get
            {
                return duration;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }
    }
}
