using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class DisableObjectAction : BehaviorInterface, PoseCommandAction
    {
        [Editable]
        private String commandName;

        [Editable]
        private String simObjectName = "this";

        [DoNotCopy]
        [DoNotSave]
        private SimObject simObject;

        protected override void link()
        {
            base.link();

            if (commandName == null)
            {
                blacklist("No command name specified.");
            }

            simObject = Owner.getOtherSimObject(simObjectName);
            if (simObject == null)
            {
                blacklist("Cannot find Disabling SimObject named '{0}'", simObjectName);
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
            simObject.Enabled = false;
        }

        public void posingEnded()
        {
            simObject.Enabled = true;
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
