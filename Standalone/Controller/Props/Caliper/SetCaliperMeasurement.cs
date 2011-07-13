using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    [TimelineActionProperties("Set Measurement")]
    public class SetCaliperMeasurement : EditableShowPropSubAction
    {
        private Caliper caliper;
        private float millimeters = 0.0f;

        public SetCaliperMeasurement()
        {

        }

        public override void started(float timelineTime, Clock clock)
        {
            caliper = PropSimObject.getElement(Caliper.BehaviorName) as Caliper;
            caliper.moveToMeasurement(Millimeters, Duration);
        }

        public override void skipTo(float timelineTime)
        {
            caliper = PropSimObject.getElement(Caliper.BehaviorName) as Caliper;
            caliper.setMeasurement(Millimeters * ((timelineTime - StartTime) / Duration));
            caliper.moveToMeasurement(Millimeters, Duration - (timelineTime - StartTime));
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
                caliper = PropSimObject.getElement(Caliper.BehaviorName) as Caliper;
                caliper.setMeasurement(Millimeters);
            }
        }

        public override void editingCompleted()
        {
            caliper = null;
        }

        public override bool Finished
        {
            get { return true; }
        }

        [Editable]
        public float Millimeters
        {
            get
            {
                return millimeters;
            }
            set
            {
                millimeters = value;
                if (caliper != null)
                {
                    caliper.setMeasurement(millimeters);
                }
            }
        }

        #region Saveable Members

        protected SetCaliperMeasurement(LoadInfo info)
            :base (info)
        {
            Millimeters = info.GetFloat("Millimeters");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("Millimeters", Millimeters);
        }

        #endregion
    }
}
