using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.Attributes;
using Engine.ObjectManagement;

namespace Medical
{
    public abstract class TransparencyOverrider : BehaviorInterface
    {
        [Editable]
        private String cullSimObjectName = "this";

        [Editable]
        private String cullTransparencyInterfaceName = "Alpha";

        TransparencyInterface cullInterface;

        protected override void link()
        {
            SimObject cullSimObject = Owner.getOtherSimObject(cullSimObjectName);
            if (cullSimObject == null)
            {
                blacklist("Could not find cull SimObject {0}.", cullSimObjectName);
            }

            cullInterface = cullSimObject.getElement(cullTransparencyInterfaceName) as TransparencyInterface;
            if (cullInterface == null)
            {
                blacklist("Could not find cull transparency interface {0} in SimObject {1}.", cullTransparencyInterfaceName, cullSimObject);
            }
            cullInterface.setTransparencyOverrider(this);
        }

        protected override void destroy()
        {
            base.destroy();
            if (cullInterface != null)
            {
                cullInterface.clearTransparencyOverrider(this);
            }
        }

        public abstract float getOverrideTransparency(float workingAlpha, int transparencyState);
    }
}
