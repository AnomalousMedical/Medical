using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Muscles
{
    public class MovementSequence : IComparer<MovementSequenceState>, Saveable
    {
        private List<MovementSequenceState> states = new List<MovementSequenceState>();
        private float duration;
        private int currentIndex = 0;
        private String name = "";

        public MovementSequence()
        {
            
        }

        public void addState(MovementSequenceState state)
        {
            states.Add(state);
            sortStates();
        }

        public void deleteState(MovementSequenceState movementSequenceState)
        {
            states.Remove(movementSequenceState);
            currentIndex = 0;
        }

        public void sortStates()
        {
            states.Sort(this);
        }

        public void setPosition(float time)
        {
            if (states.Count > 0)
            {
                time %= duration;
                MovementSequenceState currentState = states[currentIndex]; ;
                MovementSequenceState nextState;
                if (currentIndex + 1 < states.Count)
                {
                    nextState = states[currentIndex + 1];
                    if (time < currentState.StartTime || time >= nextState.StartTime)
                    {
                        findStates(time, out currentState, out nextState);
                    }
                }
                else
                {
                    nextState = states[0];
                    if (time < currentState.StartTime)
                    {
                        findStates(time, out currentState, out nextState);
                    }
                }
                currentState.blend(nextState, time, duration);
            }
        }

        private void findStates(float time, out MovementSequenceState currentState, out MovementSequenceState nextState)
        {
            int i;
            for (i = 0; i < states.Count - 1; ++i)
            {
                if (time >= states[i].StartTime && time < states[i + 1].StartTime)
                {
                    break;
                }
            }
            currentIndex = i;
            currentState = states[currentIndex];
            if (currentIndex + 1 < states.Count)
            {
                nextState = states[currentIndex + 1];
            }
            else
            {
                nextState = states[0];
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
            set
            {
                duration = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public IEnumerable<MovementSequenceState> States
        {
            get
            {
                return states;
            }
        }

        #region IComparer<float> Members

        public int Compare(MovementSequenceState x, MovementSequenceState y)
        {
            if (x.StartTime < y.StartTime)
            {
                return -1;
            }
            else if (x.StartTime > y.StartTime)
            {
                return 1;
            }
            return 0;
        }

        #endregion

        #region Saveable Members

        private const String STATES = "State";
        private const String DURATION = "Duration";
        private const String NAME = "Name";

        protected MovementSequence(LoadInfo info)
        {
            info.RebuildList<MovementSequenceState>(STATES, states);
            duration = info.GetFloat(DURATION);
            name = info.GetString(NAME);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(DURATION, duration);
            info.ExtractList<MovementSequenceState>(STATES, states);
        }

        #endregion
    }
}
