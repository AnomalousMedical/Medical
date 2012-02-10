using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Saving;
using Engine.Platform;

namespace Medical
{
    [TimelineActionProperties("Settings")]
    public class ChangePlaneSettings : EditableShowPropSubAction
    {
        private Size2 size;
        private Color color;

        private Plane plane;

        public ChangePlaneSettings()
        {
            size = new Size2(3.0f, 3.0f);
            color = Color.Red;
        }

        public override void started(float timelineTime, Clock clock)
        {
            plane = PropSimObject.getElement(Plane.BehaviorName) as Plane;
            plane.Color = color;
            plane.Size = size;
            plane.createObject();
        }

        public override void skipTo(float timelineTime)
        {
            plane = PropSimObject.getElement(Plane.BehaviorName) as Plane;
            plane.Color = color;
            plane.Size = size;
            plane.createObject();
        }

        public override void stopped(float timelineTime, Clock clock)
        {

        }

        public override void update(float timelineTime, Clock clock)
        {

        }

        public override void editing()
        {
            if (PropSimObject != null)
            {
                plane = PropSimObject.getElement(Plane.BehaviorName) as Plane;
                plane.Color = color;
                plane.Size = size;
                plane.createObject();
            }
        }

        public override void editingCompleted()
        {
            plane = null;
        }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        [Editable]
        private float Width
        {
            get
            {
                return size.Width;
            }
            set
            {
                size.Width = value;
                if (plane != null)
                {
                    plane.Size = size;
                    plane.createObject();
                }
            }
        }

        [Editable]
        private float Height
        {
            get
            {
                return size.Height;
            }
            set
            {
                size.Height = value;
                if (plane != null)
                {
                    plane.Size = size;
                    plane.createObject();
                }
            }
        }

        [Editable]
        private Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (plane != null)
                {
                    plane.Color = value;
                    plane.createObject();
                }
            }
        }

        #region Saveable Members

        protected ChangePlaneSettings(LoadInfo info)
            : base(info)
        {
            size = info.GetValue<Size2>("size");
            color = info.GetColor("color");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("size", size);
            info.AddValue("color", color);
        }

        #endregion
    }
}
