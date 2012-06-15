using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    [TimelineActionProperties("Change Arrow Shape")]
    public class ChangeArrowShapeAction : ShowPropSubAction
    {
        private float scale;
        private float tailLength;
        private Arrow arrow;

        public ChangeArrowShapeAction()
        {
            scale = 1.0f;
            tailLength = -15.8f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            arrow = PropSimObject.getElement(Arrow.ArrowBehaviorName) as Arrow;
            arrow.Scale = scale;
            arrow.TailLength = tailLength;
        }

        public override void skipTo(float timelineTime)
        {
            arrow = PropSimObject.getElement(Arrow.ArrowBehaviorName) as Arrow;
            arrow.Scale = scale;
            arrow.TailLength = tailLength;
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
                arrow = PropSimObject.getElement(Arrow.ArrowBehaviorName) as Arrow;
                arrow.Scale = scale;
                arrow.TailLength = tailLength;
            }
        }

        public override void editingCompleted()
        {
            arrow = null;
        }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        [Editable]
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                if (arrow != null)
                {
                    arrow.Scale = value;
                }
            }
        }

        [Editable]
        public float TailLength
        {
            get
            {
                return -tailLength;
            }
            set
            {
                tailLength = -value;
                if (arrow != null)
                {
                    arrow.TailLength = tailLength;
                }
            }
        }

        #region Saveable Members

        protected ChangeArrowShapeAction(LoadInfo info)
            :base (info)
        {
            Scale = info.GetFloat("Scale");
            TailLength = info.GetFloat("TailLength");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("Scale", Scale);
            info.AddValue("TailLength", TailLength);
        }

        #endregion
    }
}
