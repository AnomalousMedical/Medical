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
using Engine.Attributes;

namespace Medical
{
    /// <summary>
    /// This class will hide an object based on the transparency of another object.
    /// </summary>
    class TransparencyCuller : TransparencyOverrider
    {
        [Editable]
        String watchingInterfaceName;

        [DoNotCopy]
        [DoNotSave]
        protected TransparencyInterface watchingInterface;

        [Editable]
        private float hiddenMinValue = 0.9f;

        protected override void customLoad(Engine.Saving.LoadInfo info)
        {
            base.customLoad(info);
            if (watchingInterfaceName == null)
            {
                //legacy support for transparency culler definitions that used parentInterfaceName
                watchingInterfaceName = info.GetString("parentInterfaceName", watchingInterfaceName);
            }
        }

        protected override void link()
        {
            base.link();
            watchingInterface = TransparencyController.getTransparencyObject(watchingInterfaceName);
            if (watchingInterface == null)
            {
                blacklist("Cannot find watching interface \"{0}\".", watchingInterfaceName);
            }
        }

        public override float getOverrideTransparency(float workingAlpha, int transparencyState)
        {
            if (watchingInterface.getCurrentTransparency(transparencyState) > hiddenMinValue)
            {
                return 0.0f;
            }
            else
            {
                return workingAlpha;
            }
        }
    }
}
