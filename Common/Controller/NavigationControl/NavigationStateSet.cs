using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Logging;

namespace Medical
{
    public class NavigationStateSet : IDisposable
    {
        private Dictionary<String, NavigationState> navigationStates = new Dictionary<String, NavigationState>();
        private NavigationMenus menus = new NavigationMenus();

        public void Dispose()
        {
            menus.Dispose();
        }

        public void addState(NavigationState state)
        {
            if(!navigationStates.ContainsKey(state.Name))
            {
                navigationStates.Add(state.Name, state);
            }
            else
            {
                Log.Warning("Attempted to add duplicate navigation state {0}.", state.Name);
            }
        }

        public void removeState(NavigationState state)
        {
            navigationStates.Remove(state.Name);
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
            
        }

        public void mergeStates(NavigationStateSet toMerge)
        {
            //Copy all states
            foreach (NavigationState sourceState in toMerge.States)
            {
                addState(new NavigationState(sourceState.Name, sourceState.LookAt, sourceState.Translation, sourceState.Hidden));
            }
            //Copy all links
            foreach (NavigationState sourceState in toMerge.States)
            {
                NavigationState copiedState = getState(sourceState.Name);
                foreach (NavigationLink link in copiedState.AdjacentStates)
                {
                    copiedState.addAdjacentState(getState(link.Destination.Name), link.Button, link.VisualRadius, link.RadiusStartOffset);
                }
            }
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

        public NavigationState findClosestNonHiddenState(Vector3 position)
        {
            NavigationState closest = null;
            float closestDistanceSq = float.MaxValue;
            foreach (NavigationState state in navigationStates.Values)
            {
                if (!state.Hidden)
                {
                    float distanceSq = (position - state.Translation).length2();
                    if (distanceSq < closestDistanceSq)
                    {
                        closestDistanceSq = distanceSq;
                        closest = state;
                    }
                }
            }
            return closest;
        }

        public IEnumerable<NavigationState> States
        {
            get
            {
                return navigationStates.Values;
            }
        }

        public NavigationMenus Menus
        {
            get
            {
                return menus;
            }
        }
    }
}
