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
    class DisableObjectAction : PoseCommandActionBase
    {
        [Editable]
        private String simObjectName = "this";

        [DoNotCopy]
        [DoNotSave]
        private SimObject simObject;

        protected override void link()
        {
            simObject = Owner.getOtherSimObject(simObjectName);
            if (simObject == null)
            {
                blacklist("Cannot find Disabling SimObject named '{0}'", simObjectName);
            }

            base.link();
        }

        public override void posingStarted()
        {
            simObject.Enabled = false;
        }

        public override void posingEnded()
        {
            simObject.Enabled = true;
        }
    }
}
