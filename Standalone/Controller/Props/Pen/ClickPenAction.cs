using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    public class ClickPenAction : ShowPropSubAction
    {
        private Pen pen;

        public ClickPenAction()
        {

        }

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

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override void editing(PropEditController propEditController)
        {
            
        }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        public override string TypeName
        {
            get
            {
                return "Click Pen";
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

        #region Saveable Members

        protected ClickPenAction(LoadInfo info)
            :base (info)
        {
            
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }

        #endregion
    }
}
