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
    public class TransparencyAnatomyCommand : AbstractNumericAnatomyCommand, TransparencyChanger
    {
        internal const String UI_TEXT = "Transparency";

        internal delegate void TransparencyAnatomySmoothBlendDelegate(float alpha, float transparencyChangeMultiplier, EasingFunction easingFunction);
        internal event TransparencyAnatomySmoothBlendDelegate SmoothBlendApplied;

        [Editable]
        private String transparencyInterfaceName = "Alpha";

        [DoNotCopy]
        private TransparencyInterface transparencyInterface;

        public TransparencyAnatomyCommand()
        {

        }

        public override bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage)
        {
            transparencyInterface = owner.getElement(transparencyInterfaceName) as TransparencyInterface;
            if (transparencyInterface == null)
            {
                errorMessage = String.Format("Could not find TransparencyInterface named {0}", transparencyInterfaceName);
                return false;
            }
            parentAnatomy.TransparencyChanger = this;
            return true;
        }

        public void smoothBlend(float alpha, float duration, EasingFunction easingFunction)
        {
            if (alpha != transparencyInterface.CurrentAlpha)
            {
                transparencyInterface.timedBlend(alpha, duration, easingFunction);
                if (SmoothBlendApplied != null)
                {
                    SmoothBlendApplied.Invoke(alpha, duration, easingFunction);
                }
                fireNumericValueChanged(alpha);
            }
        }

        [DoNotCopy]
        public float CurrentAlpha
        {
            get
            {
                return NumericValue;
            }
            set
            {
                NumericValue = value;
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
                    fireNumericValueChanged(value);
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
                return AnatomyCommandUIType.Numeric;
            }
        }

        public override string UIText
        {
            get
            {
                return UI_TEXT;
            }
        }

        public override AnatomyCommand createTagGroupCommand()
        {
            CompoundTransparencyAnatomyCommand compoundCommand = new CompoundTransparencyAnatomyCommand();
            compoundCommand.addSubCommand(this);
            return compoundCommand;
        }

        public override void addToTagGroupCommand(AnatomyCommand tagGroupCommand)
        {
            ((CompoundTransparencyAnatomyCommand)tagGroupCommand).addSubCommand(this);
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
