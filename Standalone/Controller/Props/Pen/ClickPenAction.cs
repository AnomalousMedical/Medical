using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    [TimelineActionProperties("Click Pen")]
    public class ClickPenAction : EditableShowPropSubAction
    {
        private Pen pen;

        public override void started(float timelineTime, Clock clock)
        {
            pen = PropSimObject.getElement(Pen.PenBehaviorName) as Pen;
            pen.Clicked = !pen.Clicked;
        }

        public override void skipTo(float timelineTime)
        {
            pen = PropSimObject.getElement(Pen.PenBehaviorName) as Pen;
            pen.Clicked = !pen.Clicked;
        }

        public override void stopped(float timelineTime, Engine.Platform.Clock clock)
        {
            
        }

        public override void update(float timelineTime, Engine.Platform.Clock clock)
        {
            
        }

        public override void editing()
        {
            
        }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        public override float Duration
        {
            get
            {
                return 0.8f;
            }
            set
            {
                
            }
        }
    }
}
