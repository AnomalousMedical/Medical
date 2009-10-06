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
        private bool hidden;

        public NavigationState(String name, Vector3 lookAt, Vector3 translation, bool hidden)
        {
            this.name = name;
            this.translation = translation;
            this.lookAt = lookAt;
            this.hidden = hidden;
        }

        public void addAdjacentState(NavigationState adjacent, NavigationButtons button)
        {
            addAdjacentState(adjacent, button, 10.0f);
        }

        public void addAdjacentState(NavigationState adjacent, NavigationButtons button, float radius)
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
                    adjacentStates.Add(new NavigationLink(adjacent, button, radius));
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

        public Vector3 LookAt
        {
            get
            {
                return lookAt;
            }
            set
            {
                lookAt = value;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            internal set
            {
                name = value;
            }
        }

        public bool Hidden
        {
            get
            {
                return hidden;
            }
            set
            {
                hidden = value;
            }
        }

        public IEnumerable<NavigationLink> AdjacentStates
        {
            get
            {
                return adjacentStates;
            }
        }
    }
}
