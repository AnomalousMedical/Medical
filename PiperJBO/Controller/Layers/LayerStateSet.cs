using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class LayerStateSet : Saveable
    {
        private List<LayerState> stateOrder = new List<LayerState>(); //A list to maintain state order
        private Dictionary<String, LayerState> stateLookup = new Dictionary<string, LayerState>(); //A dictionary for fast lookup.

        public LayerStateSet()
        {

        }

        public void addState(LayerState state)
        {
            stateOrder.Add(state);
            stateLookup.Add(state.Name, state);
        }

        public void removeState(LayerState state)
        {
            stateOrder.Remove(state);
            stateLookup.Remove(state.Name);
        }

        public void removeState(String name)
        {
            removeState(getState(name));
        }

        public void moveState(LayerState state, int index)
        {
            if (hasState(state.Name))
            {
                stateOrder.Remove(state);
                stateOrder.Insert(index, state);
            }
            else
            {
                throw new Exception("Attempted to move a state that is not part of this LayerStateSet.");
            }
        }

        public bool hasState(String name)
        {
            return stateLookup.ContainsKey(name);
        }

        public LayerState getState(String name)
        {
            LayerState ret;
            stateLookup.TryGetValue(name, out ret);
            return ret;
        }

        public IEnumerable<LayerState> LayerStates
        {
            get
            {
                return stateOrder;
            }
        }

        #region Saveable Members

        private const string STATES = "State";

        protected LayerStateSet(LoadInfo info)
        {
            info.RebuildList<LayerState>(STATES, stateOrder);
            foreach (LayerState state in stateOrder)
            {
                stateLookup.Add(state.Name, state);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<LayerState>(STATES, stateOrder);
        }

        #endregion
    }
}
