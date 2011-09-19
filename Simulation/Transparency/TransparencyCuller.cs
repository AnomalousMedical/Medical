using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using OgreWrapper;
using Engine;
using Engine.Platform;
using OgrePlugin;
using Engine.ObjectManagement;

namespace Medical
{
    /// <summary>
    /// This class will hide an object based on the transparency of another object.
    /// </summary>
    class TransparencyCuller : TransparencySubInterface
    {
        [Editable]
        private String cullSimObjectName = "this";

        [Editable]
        private String cullTransparencyInterfaceName = "Alpha";

#if ENABLE_TRANSPARENCY_CULLER
        TransparencyInterface cullInterface;

        protected override void constructed()
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
        }

        internal override void setAlpha(float alpha)
        {
            if (alpha == 1.0f)
            {
                cullInterface.OverrideAlpha = 0.0f;
            }
            else
            {
                cullInterface.clearOverrideAlpha();
            }
        }
#else
        internal override void setAlpha(float alpha)
        {
            
        }
#endif
    }
}
