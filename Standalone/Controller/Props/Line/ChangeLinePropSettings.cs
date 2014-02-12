using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;
using Engine;

namespace Medical
{
    public class ChangeLinePropSettings : ShowPropSubAction
    {
        private float length;
        private Color color;

        private LineProp line;

        public ChangeLinePropSettings()
        {
            length = 6.0f;
            color = Color.Red;
        }

        public override void started(float timelineTime, Clock clock)
        {
            line = PropSimObject.getElement(LineProp.BehaviorName) as LineProp;
            line.Length = length;
            line.Color = color;
            line.createLine();
        }

        public override void skipTo(float timelineTime)
        {
            line = PropSimObject.getElement(LineProp.BehaviorName) as LineProp;
            line.Length = length;
            line.Color = color;
            line.createLine();
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
                line = PropSimObject.getElement(LineProp.BehaviorName) as LineProp;
                line.Length = length;
                line.Color = color;
                line.createLine();
            }
        }

        public override void editingCompleted()
        {
            line = null;
        }

        public override bool Finished
        {
            get
            {
                return true;
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
                color= value;
                if(line != null)
                {
                    line.Color = value;
                    line.createLine();
                }
            }
        }

        [Editable]
        public float Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
                if (line != null)
                {
                    line.Length = value;
                    line.createLine();
                }
            }
        }

        public override string TypeName
        {
            get
            {
                return "Settings";
            }
        }

        #region Saveable Members

        protected ChangeLinePropSettings(LoadInfo info)
            : base(info)
        {
            color = info.GetColor("color");
            length = info.GetFloat("length");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("color", color);
            info.AddValue("length", length);
        }

        #endregion
    }
}
