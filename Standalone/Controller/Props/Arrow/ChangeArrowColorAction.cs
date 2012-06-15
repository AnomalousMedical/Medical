using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    [TimelineActionProperties("Change Color")]
    public class ChangeArrowColorAction : ShowPropSubAction
    {
        private Arrow arrow;
        private Color arrowColor;

        public ChangeArrowColorAction()
        {
            ArrowColor = Color.Green;
        }

        public override void started(float timelineTime, Engine.Platform.Clock clock)
        {
            arrow = PropSimObject.getElement(Arrow.ArrowBehaviorName) as Arrow;
            arrow.Color = ArrowColor;
        }

        public override void skipTo(float timelineTime)
        {
            arrow = PropSimObject.getElement(Arrow.ArrowBehaviorName) as Arrow;
            arrow.Color = ArrowColor;
        }

        public override void stopped(float timelineTime, Engine.Platform.Clock clock)
        {
            
        }

        public override void update(float timelineTime, Engine.Platform.Clock clock)
        {
            
        }

        public override void editing()
        {
            if (PropSimObject != null)
            {
                arrow = PropSimObject.getElement(Arrow.ArrowBehaviorName) as Arrow;
                arrow.Color = ArrowColor;
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
        public Color ArrowColor
        {
            get
            {
                return arrowColor;
            }
            set
            {
                arrowColor = value;
                if (arrow != null)
                {
                    arrow.Color = value;
                }
            }
        }

        #region Saveable Members

        protected ChangeArrowColorAction(LoadInfo info)
            :base (info)
        {
            ArrowColor = info.GetColor("ArrowColor");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("ArrowColor", ArrowColor);
        }

        #endregion
    }
}
