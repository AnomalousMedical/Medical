using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine.Platform;

namespace Medical
{
    [TimelineActionProperties("Set Transparency")]
    public class SetPropTransparencyAction : ShowPropSubAction
    {
        private float transparency = 1.0f;

        private bool finished = false;
        private PropFadeBehavior propBehavior;

        public SetPropTransparencyAction()
        {
            Duration = 1.0f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            propBehavior = PropSimObject.getElement(PropFactory.FadeBehaviorName) as PropFadeBehavior;
            propBehavior.fade(transparency, Duration);
        }

        public override void skipTo(float timelineTime)
        {
            propBehavior = PropSimObject.getElement(PropFactory.FadeBehaviorName) as PropFadeBehavior;
            if (timelineTime <= EndTime)
            {
                //Figure out how transparent we should be
                //This is pretty screwy right now, but whatever can fix later.
                if (Duration != 0.0f)
                {
                    float partialFade = ((timelineTime - StartTime) / Duration) * transparency;
                    propBehavior.CurrentTransparency += (transparency - propBehavior.CurrentTransparency) * partialFade;
                }
                propBehavior.fade(transparency, EndTime - timelineTime);
            }
            else
            {
                propBehavior.fade(transparency, 0.0f);
                finished = true;
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime >= StartTime + Duration;
        }

        public override void editing()
        {
            //movePreviewProp(Translation, Rotation);
        }

        public override bool Finished
        {
            get { return finished; }
        }

        [Editable]
        public float Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                transparency = value;
            }
        }

        #region Saveable Members

        private const String TRANSPARENCY = "transparency";

        protected SetPropTransparencyAction(LoadInfo info)
            :base (info)
        {
            transparency = info.GetFloat(TRANSPARENCY, transparency);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(TRANSPARENCY, transparency);
        }

        #endregion
    }
}
