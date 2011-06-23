using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Move")]
    public class MovePropAction : ShowPropSubAction
    {
        private Vector3 startTranslation;
        private Quaternion startRotation;
        private Vector3 endTranslation = Vector3.Zero;
        private Quaternion endRotation = Quaternion.Identity;

        private bool finished = false;
        private PropFadeBehavior propBehavior;

        public MovePropAction()
        {
            Duration = 1.0f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            startTranslation = this.PropSimObject.Translation;
            startRotation = this.PropSimObject.Rotation;
            finished = false;
            propBehavior = PropSimObject.getElement(PropFactory.FadeBehaviorName) as PropFadeBehavior;
        }

        public override void skipTo(float timelineTime)
        {
            if (timelineTime <= EndTime)
            {
                started(timelineTime, null);
            }
            else
            {
                propBehavior = PropSimObject.getElement(PropFactory.FadeBehaviorName) as PropFadeBehavior;
                propBehavior.changePosition(endTranslation, endRotation);
                finished = true;
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            float percentage = (timelineTime - StartTime) / Duration;
            if (percentage > 1.0f)
            {
                percentage = 1.0f;
            }
            propBehavior.changePosition(startTranslation.lerp(ref endTranslation, ref percentage), startRotation.slerp(ref endRotation, percentage));
            finished = timelineTime >= StartTime + Duration;
        }

        public override void editing()
        {
            movePreviewProp(Translation, Rotation);
        }

        public override void reverseSides()
        {
            Translation = new Vector3(-Translation.x, Translation.y, Translation.z);
            //Vector3 euler = Rotation.getEuler();
            //Rotation = new Quaternion(euler.x, -euler.y, euler.z);
        }

        public override bool Finished
        {
            get { return finished; }
        }

        public Vector3 Translation
        {
            get
            {
                return endTranslation;
            }
            set
            {
                endTranslation = value;
                movePreviewProp(Translation, Rotation);
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return endRotation;
            }
            set
            {
                endRotation = value;
                movePreviewProp(Translation, Rotation);
            }
        }

        #region Saveable Members

        private const String END_TRANSLATION = "endTranslation";
        private const String END_ROTATION = "endRotation";

        protected MovePropAction(LoadInfo info)
            :base (info)
        {
            endTranslation = info.GetVector3(END_TRANSLATION, endTranslation);
            endRotation = info.GetQuaternion(END_ROTATION, endRotation);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(END_TRANSLATION, endTranslation);
            info.AddValue(END_ROTATION, endRotation);
        }

        #endregion
    }
}
