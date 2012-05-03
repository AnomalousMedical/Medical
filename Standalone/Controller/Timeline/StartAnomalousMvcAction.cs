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
            
        }

        public override void dumpToLog()
        {
            
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(context.getEditInterface());
        }

        protected StartAnomalousMvcAction(LoadInfo info)
            :base(info)
        {
            context = info.GetValue<AnomalousMvcContext>("Context");
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue("Context", context);
        }
    }
}
