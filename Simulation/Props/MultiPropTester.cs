using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Platform;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    class MultiPropTester : Behavior
    {
        [Editable]
        private String multiPropName;

        [DoNotCopy]
        [DoNotSave]
        private float currentScale = 0.0f;

        [DoNotCopy]
        [DoNotSave]
        private MultiProp multiProp;

        [DoNotCopy]
        [DoNotSave]
        private MultiPropSection woot1;

        [DoNotCopy]
        [DoNotSave]
        private MultiPropSection woot2;

        [DoNotCopy]
        [DoNotSave]
        private MultiPropSection woot3;

        protected override void constructed()
        {
            multiProp = Owner.getElement(multiPropName) as MultiProp;

            if(multiProp == null)
            {
                blacklist("Cannot find MultiProp '{0}'", multiPropName);
            }

            woot1 = multiProp.addSection(new MultiPropSection("Woot1", "Box016.mesh", "Box016", new Vector3(-1, 0, 0), Quaternion.Identity, Vector3.ScaleIdentity));
            woot2 = multiProp.addSection(new MultiPropSection("Woot2", "Box016.mesh", "Box016", new Vector3(1, 0, 0), Quaternion.Identity, Vector3.ScaleIdentity));
            woot3 = multiProp.addSection(new MultiPropSection("Woot3", "PerfTooth01.mesh", "Tooth1collision", new Vector3(0, 0, 1), Quaternion.Identity, Vector3.ScaleIdentity));

            base.constructed();
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            multiProp.beginUpdates();

            currentScale += 0.1f * clock.DeltaSeconds;
            currentScale %= 1.0f;

            woot3.Scale = new Vector3(1, currentScale, 1);
            woot3.updatePosition();

            woot2.Translation = new Vector3(currentScale, 0, 0);
            woot2.updatePosition();

            multiProp.finishUpdates();
        }
    }
}
