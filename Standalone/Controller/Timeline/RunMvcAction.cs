using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical
{
    public class RunMvcAction : TimelineInstantAction
    {
        public RunMvcAction()
        {

        }

        public RunMvcAction(String action)
        {
            this.Action = action;
        }

        public override void doAction()
        {
            Timeline.TimelineController.MvcContext.runAction(Action);
        }

        public override void dumpToLog()
        {
            
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public override void cleanup(CleanupInfo cleanupInfo)
        {

        }

        [EditableAction]
        public String Action { get; set; }

        protected RunMvcAction(LoadInfo info)
            :base(info)
        {
            Action = info.GetString("Action");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("Action", Action);
        }
    }
}
