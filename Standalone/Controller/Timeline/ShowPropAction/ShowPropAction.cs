using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine;

namespace Medical
{
    [TimelineActionProperties("Show Prop", 128 / 255f, 0 / 255f, 255 / 255f, GUIType = null)]
    class ShowPropAction : TimelineAction
    {
        private bool finished;
        private SimObject simObject;

        public override void started(float timelineTime, Clock clock)
        {
            finished = false;
            makeProp();
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            simObject.destroy();
            simObject = null;
        }

        public override void update(float timelineTime, Clock clock)
        {
            finished = timelineTime > StartTime + Duration;
        }

        public override void editing()
        {
            makeProp();
        }

        public override void editingCompleted()
        {
            base.editingCompleted();
            simObject.destroy();
            simObject = null;
        }

        public override bool Finished
        {
            get 
            {
                return finished;
            }
        }

        private void makeProp()
        {
            simObject = TimelineController.PropFactory.createProp("Arrow", new Vector3(3, 0, 0), Quaternion.Identity);
        }
    }
}
