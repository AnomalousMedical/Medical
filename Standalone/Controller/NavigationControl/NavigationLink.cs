using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public enum NavigationButtons
    {
        Left,
        Right,
        Up,
        Down,
        ZoomIn,
        ZoomOut,
    }

    public class NavigationLink
    {
        public static NavigationButtons GetOppositeButton(NavigationButtons button)
        {
            switch (button)
            {
                case NavigationButtons.Down:
                    return NavigationButtons.Up;
                case NavigationButtons.Up:
                    return NavigationButtons.Down;
                case NavigationButtons.Right:
                    return NavigationButtons.Left;
                case NavigationButtons.Left:
                    return NavigationButtons.Right;
                case NavigationButtons.ZoomIn:
                    return NavigationButtons.ZoomOut;
                case NavigationButtons.ZoomOut:
                    return NavigationButtons.ZoomIn;
            }
            throw new Exception("Invalid button");
        }

        private NavigationState destination;
        private NavigationButtons button;
        private float visualRadius = 10.0f;
        private Vector3 radiusStartOffset = Vector3.Zero;

        public NavigationLink(NavigationState destination, NavigationButtons button)
        {
            this.destination = destination;
            this.button = button;
        }

        public NavigationLink(NavigationState destination, NavigationButtons button, float visualRadius, Vector3 radiusStartOffset)
        {
            this.destination = destination;
            this.button = button;
            this.visualRadius = visualRadius;
            this.radiusStartOffset = radiusStartOffset;
        }

        public NavigationButtons Button
        {
            get
            {
                return button;
            }
            set
            {
                button = value;
            }
        }

        public NavigationState Destination
        {
            get
            {
                return destination;
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

        public Vector3 RadiusStartOffset
        {
            get
            {
                return radiusStartOffset;
            }
            set
            {
                radiusStartOffset = value;
            }
        }
    }
}
