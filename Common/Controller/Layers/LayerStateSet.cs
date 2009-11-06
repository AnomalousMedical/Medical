using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class LayerStateSet : Saveable, IDisposable
    {
        private Dictionary<String, LayerState> states = new Dictionary<string, LayerState>();

        public LayerStateSet()
        {

        }

        public void Dispose()
        {
            foreach (LayerState state in states.Values)
            {
                state.Dispose();
            }
        }

        public void addState(LayerState state)
        {
            states.Add(state.Name, state);
        }

        public void removeState(LayerState state)
        {
            states.Remove(state.Name);
        }

        public void removeState(String name)
        {
            states.Remove(name);
        }

        public bool hasState(String name)
        {
            return states.ContainsKey(name);
        }

        public LayerState getState(String name)
        {
            LayerState ret;
            states.TryGetValue(name, out ret);
            return ret;
        }

        public IEnumerable<String> LayerStateNames
        {
            get
            {
                return states.Keys;
            }
        }

        public IEnumerable<LayerState> LayerStates
        {
            get
            {
                return states.Values;
            }
        }

        #region Saveable Members

        private const string STATES = "State";

        protected LayerStateSet(LoadInfo info)
        {
            info.RebuildDictionary<String, LayerState>(STATES, states);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, LayerState>(STATES, states);
        }

        #endregion
    }
}
