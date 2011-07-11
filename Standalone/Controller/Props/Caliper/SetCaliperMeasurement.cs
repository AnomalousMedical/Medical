using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Saving;

namespace Medical
{
    [TimelineActionProperties("Set Measurement")]
    class SetCaliperMeasurement : EditableShowPropSubAction
    {
        private bool finished = false;

        public SetCaliperMeasurement()
        {

        }

        public override void started(float timelineTime, Clock clock)
        {
            
        }

        public override void skipTo(float timelineTime)
        {
            
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
            get { return finished; }
        }

        #region Saveable Members

        protected SetCaliperMeasurement(LoadInfo info)
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
