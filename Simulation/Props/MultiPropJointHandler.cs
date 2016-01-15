using BulletPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class MultiPropJointHandler : BehaviorInterface
    {
        [Editable]
        private String jointName = "Joint";

        [Editable]
        private String multiPropName;

        [DoNotCopy]
        [DoNotSave]
        private MultiProp multiProp;

        [DoNotCopy]
        [DoNotSave]
        private TypedConstraintElement joint;

        public MultiPropJointHandler()
        {
            
        }

        public MultiPropJointHandler(String jointName, String multiPropName)
        {
            this.jointName = jointName;
            this.multiPropName = multiPropName;
        }

        protected override void constructed()
        {
            joint = Owner.getElement(jointName) as TypedConstraintElement;
            if (joint == null)
            {
                blacklist("Could not find Joint {0}.", jointName);
            }

            multiProp = Owner.getElement(multiPropName) as MultiProp;

            if (multiProp == null)
            {
                blacklist("Cannot find MultiProp '{0}'", multiPropName);
            }

            multiProp.UpdatesStarting += MultiProp_UpdatesStarting;
            multiProp.UpdatesCompleted += MultiProp_UpdatesCompleted;

            base.constructed();
        }

        protected override void willDestroy()
        {
            multiProp.UpdatesStarting -= MultiProp_UpdatesStarting;
            multiProp.UpdatesCompleted -= MultiProp_UpdatesCompleted;
            base.willDestroy();
        }

        private void MultiProp_UpdatesCompleted(MultiProp obj)
        {
            joint.updateCompleted();
        }

        private void MultiProp_UpdatesStarting(MultiProp obj)
        {
            joint.beginUpdate();
        }
    }
}
