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
        private AnomalousMvcContext context;

        public StartAnomalousMvcAction()
        {
            context = new AnomalousMvcContext();
        }

        public override void doAction()
        {
            context._setCore(Timeline.TimelineController.TEMP_MVC_CORE);
            context.runAction(StartAction);
        }

        public override void dumpToLog()
        {
            
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        [Editable]
        public String StartAction { get; set; }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(context.getEditInterface());
        }

        protected StartAnomalousMvcAction(LoadInfo info)
            :base(info)
        {
            context = info.GetValue<AnomalousMvcContext>("Context");
            StartAction = info.GetString("StartAction");
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue("Context", context);
            info.AddValue("StartAction", StartAction);
        }
    }
}
