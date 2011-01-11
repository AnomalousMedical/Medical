using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Push Plunger")]
    public class PushPlungerAction : ShowPropSubAction
    {
        private Syringe syringe;

        public PushPlungerAction()
        {
            PlungePercentage = 1.0f;
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

        public override void stopped(float timelineTime, Clock clock)
        {

        }

        public override void update(float timelineTime, Clock clock)
        {

        }

        public override void editing()
        {

        }

        public override bool Finished
        {
            get { return true; }
        }

        public float PlungePercentage { get; set; }

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
