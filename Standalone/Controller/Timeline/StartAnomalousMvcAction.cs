using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    class StartAnomalousMvcAction : TimelineInstantAction
    {
        public StartAnomalousMvcAction()
        {
            
        }

        public override void doAction()
        {
            AnomalousMvcCore core = Timeline.TimelineController.TEMP_MVC_CORE;
            AnomalousMvcContext context = core.loadContext(Context);
            context.runAction(StartAction);
        }

        public override void dumpToLog()
        {
            
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        [Editable]
        public String Context { get; set; }

        [Editable]
        public String StartAction { get; set; }

        protected StartAnomalousMvcAction(LoadInfo info)
            :base(info)
        {
            StartAction = info.GetString("StartAction");
            Context = info.GetString("Context");
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue("Context", Context);
            info.AddValue("StartAction", StartAction);
        }
    }
}
