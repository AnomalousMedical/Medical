using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.ObjectManagement;
using Engine.Attributes;
using Engine.Editing;
using Logging;

namespace Medical
{
    class TransparencyAnatomyCommand : AbstractNumericAnatomyCommand
    {
        [Editable]
        private String transparencyInterfaceName = "Alpha";

        [DoNotCopy]
        private TransparencyInterface transparencyInterface;

        public TransparencyAnatomyCommand()
        {

        }

        public override void link(SimObject owner)
        {
            transparencyInterface = owner.getElement(transparencyInterfaceName) as TransparencyInterface;
            Valid = transparencyInterface != null;
            if (!Valid)
            {
                Log.Error("Could not find TransparencyInterface named {0} in SimObject {1}.", transparencyInterfaceName, owner.Name);
            }
        }

        [DoNotCopy]
        public override float NumericValue
        {
            get
            {
                return transparencyInterface.CurrentAlpha;
            }
            set
            {
                if (transparencyInterface.CurrentAlpha != value)
                {
                    transparencyInterface.CurrentAlpha = value;
                    
                }
            }
        }

        public override float NumericValueMin
        {
            get
            {
                return 0.0f;
            }
        }

        public override float NumericValueMax
        {
            get
            {
                return 1.0f;
            }
        }

        public override AnatomyCommandUIType UIType
        {
            get
            {
                return AnatomyCommandUIType.Sliding;
            }
        }

        #region Saveable

        private const String TRANSPARENCY_INTERFACE_NAME = "TransparencyInterfaceName";

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(TRANSPARENCY_INTERFACE_NAME, transparencyInterfaceName);
        }

        protected TransparencyAnatomyCommand(LoadInfo info)
        {
            transparencyInterfaceName = info.GetString(TRANSPARENCY_INTERFACE_NAME);
        }

        #endregion
    }
}
