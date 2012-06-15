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
    [TimelineActionProperties("Settings")]
    public class ChangeCircularHighlightSettings : ShowPropSubAction
    {
        private float minorAxis;
        private float majorAxis;
        private Degree theta;
        private Color color;
        private float thickness;

        private CircularHighlight highlight;

        public ChangeCircularHighlightSettings()
        {
            minorAxis = 1.0f;
            majorAxis = 1.0f;
            theta = 0;
            color = Color.Red;
            thickness = 0.2f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            highlight = PropSimObject.getElement(CircularHighlight.BehaviorName) as CircularHighlight;
            highlight.MajorAxis = majorAxis;
            highlight.MinorAxis = minorAxis;
            highlight.Theta = theta;
            highlight.Color = color;
            highlight.Thickness = thickness;
            highlight.createEllipse();
        }

        public override void skipTo(float timelineTime)
        {
            highlight = PropSimObject.getElement(CircularHighlight.BehaviorName) as CircularHighlight;
            highlight.MajorAxis = majorAxis;
            highlight.MinorAxis = minorAxis;
            highlight.Theta = theta;
            highlight.Color = color;
            highlight.Thickness = thickness;
            highlight.createEllipse();
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
                highlight = PropSimObject.getElement(CircularHighlight.BehaviorName) as CircularHighlight;
                highlight.MajorAxis = majorAxis;
                highlight.MinorAxis = minorAxis;
                highlight.Theta = theta;
                highlight.Color = color;
                highlight.Thickness = thickness;
                highlight.createEllipse();
            }
        }

        public override void editingCompleted()
        {
            highlight = null;
        }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        [Editable]
        private float MinorAxis
        {
            get
            {
                return minorAxis;
            }
            set
            {
                minorAxis= value;
                if(highlight != null)
                {
                    highlight.MinorAxis = value;
                    highlight.createEllipse();
                }
            }
        }

        [Editable]
        private float MajorAxis
        {
            get
            {
                return majorAxis;
            }
            set
            {
                majorAxis= value;
                if(highlight != null)
                {
                    highlight.MajorAxis = value;
                    highlight.createEllipse();
                }
            }
        }

        [Editable]
        private float Theta
        {
            get
            {
                return theta;
            }
            set
            {
                theta = value;
                if(highlight != null)
                {
                    highlight.Theta = theta;
                    highlight.createEllipse();
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
                color= value;
                if(highlight != null)
                {
                    highlight.Color = value;
                    highlight.createEllipse();
                }
            }
        }

        [Editable]
        public float Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
                if (highlight != null)
                {
                    highlight.Thickness = value;
                    highlight.createEllipse();
                }
            }
        }

        #region Saveable Members

        protected ChangeCircularHighlightSettings(LoadInfo info)
            : base(info)
        {
            minorAxis = info.GetFloat("minorAxis");
            majorAxis = info.GetFloat("majorAxis");
            theta = info.GetFloat("theta");
            color = info.GetColor("color");
            thickness = info.GetFloat("thickness", 0.2f);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("minorAxis", minorAxis);
            info.AddValue("majorAxis", majorAxis);
            info.AddValue("theta", (float)theta);
            info.AddValue("color", color);
            info.AddValue("thickness", thickness);
        }

        #endregion
    }
}
