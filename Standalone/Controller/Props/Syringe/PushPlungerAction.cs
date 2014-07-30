using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class PushPlungerAction : ShowPropSubAction
    {
        private Syringe syringe;
        private float plungePercentage = 1.0f;

        public PushPlungerAction()
        {
            Duration = 1.0f;
        }

        public override void started(float timelineTime, Clock clock)
        {
            syringe = PropSimObject.getElement(Syringe.BehaviorName) as Syringe;
            if (syringe != null)
            {
                syringe.plunge(PlungePercentage, Duration);
            }
            else
            {
                Log.Warning("Prop SimObject does not have a Syringe behavior, cannot push plunger.");
            }
        }

        public override void skipTo(float timelineTime)
        {
            if (timelineTime <= EndTime)
            {
                started(timelineTime, null);
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {

        }

        public override void update(float timelineTime, Clock clock)
        {

        }

        public override void editing(PropEditController propEditController)
        {
            if (PropSimObject != null)
            {
                syringe = PropSimObject.getElement(Syringe.BehaviorName) as Syringe;
                if (syringe != null)
                {
                    syringe.setPlungePosition(PlungePercentage);
                }
            }
        }

        public override void editingCompleted(PropEditController propEditController)
        {
            syringe = null;
        }

        public override bool Finished
        {
            get { return true; }
        }

        [EditableMinMax(0, 1, 0.05f)]
        public float PlungePercentage
        {
            get
            {
                return plungePercentage;
            }
            set
            {
                plungePercentage = value;
                if (syringe != null)
                {
                    syringe.setPlungePosition(value);
                }
            }
        }

        public override string TypeName
        {
            get
            {
                return "Push Plunger";
            }
        }

        #region Saveable

        private const String PLUNGE_PERCENTAGE = "PlungePercentage";

        protected PushPlungerAction(LoadInfo info)
            :base(info)
        {
            PlungePercentage = info.GetFloat(PLUNGE_PERCENTAGE, 1.0f);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue(PLUNGE_PERCENTAGE, PlungePercentage);
        }

        #endregion
    }
}
