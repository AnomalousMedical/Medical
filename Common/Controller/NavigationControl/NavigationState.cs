using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class NavigationState
    {
        private String name;
        private List<NavigationLink> adjacentStates = new List<NavigationLink>();
        private Vector3 lookAt;
        private Vector3 translation;
        private float visualRadius = 10.0f;

        public NavigationState(String name, Vector3 lookAt, Vector3 translation)
        {
            this.name = name;
            this.translation = translation;
            this.lookAt = lookAt;
        }

        public void addAdjacentState(NavigationState adjacent, NavigationButtons button)
        {
            if (adjacent != null)
            {
                bool allowAdd = true;
                foreach (NavigationLink link in adjacentStates)
                {
                    if (link.Destination == adjacent)
                    {
                        allowAdd = false;
                        break;
                    }
                }
                if (allowAdd)
                {
                    adjacentStates.Add(new NavigationLink(adjacent, button));
                }
            }
        }

        public void addTwoWayAdjacentState(NavigationState adjacent, NavigationButtons button)
        {
            if (adjacent != null)
            {
                addAdjacentState(adjacent, button);
                adjacent.addAdjacentState(this, NavigationLink.GetOppositeButton(button));
            }
        }

        public void addTwoWayAdjacentState(NavigationState adjacent)
        {
            addTwoWayAdjacentState(adjacent, NavigationButtons.Up);
        }

        public void removeAdjacentState(NavigationState adjacent)
        {
            NavigationLink matchingLink = null;
            foreach (NavigationLink link in adjacentStates)
            {
                if (link.Destination == adjacent)
                {
                    matchingLink = link;
                    break;
                }
            }
            if (matchingLink != null)
            {
                adjacentStates.Remove(matchingLink);
            }
        }

        internal IEnumerable<NavigationLink> AdjacentStates
        {
            get
            {
                return adjacentStates;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return lookAt;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public float VisualRadius
        {
            get
            {
                return visualRadius;
            }
            set
            {
                visualRadius = value;
            }
        }
    }
}
