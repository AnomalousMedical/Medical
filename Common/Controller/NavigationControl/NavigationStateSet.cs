using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;

namespace Medical
{
    public class NavigationStateSet
    {
        private Dictionary<String, NavigationState> navigationStates = new Dictionary<String, NavigationState>();

        public void addState(NavigationState state)
        {
            navigationStates.Add(state.Name, state);
        }

        public void removeState(NavigationState state)
        {
            navigationStates.Remove(state.Name);
        }

        public void clearStates()
        {
            navigationStates.Clear();
        }

        public NavigationState getState(String name)
        {
            NavigationState state;
            navigationStates.TryGetValue(name, out state);
            if (state == null)
            {
                Log.Warning("Could not find Navigation State \"{0}\".", name);
            }
            return state;
        }

        public NavigationState findClosestState(Vector3 position)
        {
            NavigationState closest = null;
            float closestDistanceSq = float.MaxValue;
            foreach (NavigationState state in navigationStates.Values)
            {
                float distanceSq = (position - state.Translation).length2();
                if (distanceSq < closestDistanceSq)
                {
                    closestDistanceSq = distanceSq;
                    closest = state;
                }
            }
            return closest;
        }

        internal IEnumerable<String> StateNames
        {
            get
            {
                return navigationStates.Keys;
            }
        }

        internal IEnumerable<NavigationState> States
        {
            get
            {
                return navigationStates.Values;
            }
        }
    }
}
