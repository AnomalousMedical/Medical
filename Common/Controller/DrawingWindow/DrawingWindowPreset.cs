using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public enum DrawingWindowPosition
    {
        Left,
        Top,
        Right,
        Bottom,
    }

    public class DrawingWindowPreset
    {
        String name;
        Vector3 position;
        Vector3 lookAt;
        String parentWindow;
        DrawingWindowPosition windowPosition = DrawingWindowPosition.Top;

        public DrawingWindowPreset(String name, Vector3 position, Vector3 lookAt)
        {
            this.name = name;
            this.position = position;
            this.lookAt = lookAt;
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
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

        public String ParentWindow
        {
            get
            {
                return parentWindow;
            }
            set
            {
                parentWindow = value;
            }
        }

        public DrawingWindowPosition WindowPosition
        {
            get
            {
                return windowPosition;
            }
            set
            {
                windowPosition = value;
            }
        }
    }
}
