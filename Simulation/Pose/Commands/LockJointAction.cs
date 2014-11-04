using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class LockJointAction : BehaviorInterface, PoseCommandAction
    {
        [Editable]
        private String commandName;

        [Editable]
        private String limitSimObjectName = "this";

        [Editable]
        private String limitName;

        [DoNotCopy]
        [DoNotSave]
        private BEPUikLimit limit;

        protected override void link()
        {
            base.link();

            if(commandName == null)
            {
                blacklist("No command name specified.");
            }

            var limitSimObject = Owner.getOtherSimObject(limitSimObjectName);
            if(limitSimObject == null)
            {
                blacklist("Cannot find LimitSimObject named '{0}'", limitSimObjectName);
            }

            limit = limitSimObject.getElement(limitName) as BEPUikLimit;
            if (limit == null)
            {
                blacklist("Cannot find BEPUik limit '{0}' on '{1}'", limitName, limitSimObjectName);
            }

            PoseCommandManager.addAction(this);
        }

        protected override void destroy()
        {
            PoseCommandManager.removeAction(this);
            base.destroy();
        }

        public void posingStarted()
        {
            limit.Locked = true;
        }

        public void posingEnded()
        {
            limit.Locked = false;
        }

        public String CommandName
        {
            get
            {
                return commandName;
            }
        }
    }
}
