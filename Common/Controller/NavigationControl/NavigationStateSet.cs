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
        private List<NavigationState> stateOrder = new List<NavigationState>();

        public void addState(NavigationState state)
        {
            navigationStates.Add(state.Name, state);
            stateOrder.Add(state);
        }

        public void removeState(NavigationState state)
        {
            navigationStates.Remove(state.Name);
            stateOrder.Remove(state);
            //break all links
            foreach (NavigationState breakLinkState in navigationStates.Values)
            {
                breakLinkState.removeAdjacentState(state);
            }
        }

        public void renameState(NavigationState state, String newName)
        {
            navigationStates.Remove(state.Name);
            state.Name = newName;
            addState(state);
        }

        public void moveState(NavigationState state, int newIndex)
        {
            stateOrder.Remove(state);
            stateOrder.Insert(newIndex, state);
        }

        public void clearStates()
        {
            navigationStates.Clear();
            stateOrder.Clear();
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
            foreach (NavigationState state in stateOrder)
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

        public IEnumerable<NavigationState> States
        {
            get
            {
                return stateOrder;
            }
        }
    }
}
