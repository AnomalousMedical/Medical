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
        private List<NavigationState> adjacentStates = new List<NavigationState>();
        private Vector3 lookAt;
        private Vector3 translation;
        private float visualRadius = 10.0f;

        public NavigationState(String name, Vector3 lookAt, Vector3 translation)
        {
            this.name = name;
            this.translation = translation;
            this.lookAt = lookAt;
        }

        public void addAdjacentState(NavigationState adjacent)
        {
            if (adjacent != null && !adjacentStates.Contains(adjacent))
            {
                adjacentStates.Add(adjacent);
            }
        }

        public void addTwoWayAdjacentState(NavigationState adjacent)
        {
            if (adjacent != null)
            {
                addAdjacentState(adjacent);
                adjacent.addAdjacentState(this);
            }
        }

        public void removeAdjacentState(NavigationState adjacent)
        {
            adjacentStates.Remove(adjacent);
        }

        public IEnumerable<NavigationState> AdjacentStates
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
