using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class NavigationController
    {
        private List<NavigationState> navigationStates = new List<NavigationState>();

        public NavigationController()
        {
            
        }

        public void addState(NavigationState state)
        {
            navigationStates.Add(state);
        }

        public void removeState(NavigationState state)
        {
            navigationStates.Remove(state);
        }

        public NavigationState findClosestState(Vector3 position)
        {
            NavigationState closest = null;
            float closestDistanceSq = float.MaxValue;
            foreach (NavigationState state in navigationStates)
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
    }
}
