using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using OgrePlugin;
using OgreWrapper;

namespace Medical
{
    /// <summary>
    /// This class will do transparency based off another TransparencyInterface
    /// </summary>
    public abstract class TransparencySubInterface : Interface
    {
        [Editable] String parentInterfaceName;

        [DoNotCopy]
        [DoNotSave]
        protected TransparencyInterface parentInterface;

        protected override void link()
        {
            parentInterface = TransparencyController.getTransparencyObject(parentInterfaceName);
            if (parentInterface == null)
            {
                blacklist("Cannot find parent interface \"{0}\".", parentInterfaceName);
            }
            parentInterface.addSubInterface(this);
        }

        protected override void destroy()
        {
            base.destroy();
            if (parentInterface != null)
            {
                parentInterface.removeSubInterface(this);
            }
        }

        internal abstract void setAlpha(float alpha);

        /// <summary>
        /// This function should only be called from the TransparencyInterface.
        /// </summary>
        internal void _disconnectFromInterface()
        {
            parentInterface = null;
        }
    }
}
