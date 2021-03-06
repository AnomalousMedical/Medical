﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine;

namespace Medical
{
    /// <summary>
    /// This class will adjust transparency for an object that is part of
    /// another object. It will either track the parent object or be disabled
    /// entirely.
    /// </summary>
    class TransparencySectionAnatomyCommand : AbstractBooleanAnatomyCommand
    {
        [Editable]
        private String uiText;

        [Editable]
        private String targetSimObjectName;

        [Editable]
        private String targetTransparencyInterfaceName = "Alpha";

        private bool trackingTransparency = true;

        [DoNotCopy]
        [DoNotSave]
        TransparencyInterface transparencyInterface;

        [DoNotCopy]
        [DoNotSave]
        TransparencyAnatomyCommand transparencyAnatomyCommand = null;

        public TransparencySectionAnatomyCommand()
        {

        }

        public override bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage)
        {
            SimObject targetSimObject = owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                errorMessage = String.Format("Could not find target SimObject {0}", targetSimObjectName);
                return false;
            }
            transparencyInterface = targetSimObject.getElement(targetTransparencyInterfaceName) as TransparencyInterface;
            if (transparencyInterface == null)
            {
                errorMessage = String.Format("Could not find target TransparencyInterface '{0}' in '{1}'", targetTransparencyInterfaceName, targetSimObject.Name);
                return false;
            }
            foreach (AnatomyCommand command in parentAnatomy.Commands)
            {
                if (command.UIText == TransparencyAnatomyCommand.UI_TEXT)
                {
                    transparencyAnatomyCommand = (TransparencyAnatomyCommand)command;
                    break;
                }
            }
            if (transparencyAnatomyCommand == null)
            {
                errorMessage = String.Format("Can not have a TransparencySectionAnatomyCommand on AnatomyIdentifier {0} because it has no TransparencyAnatomyCommand.", parentAnatomy.AnatomicalName);
                return false;
            }
            transparencyAnatomyCommand.NumericValueChanged += transparencyAnatomyCommand_NumericValueChanged;
            transparencyAnatomyCommand.SmoothBlendApplied += transparencyAnatomyCommand_SmoothBlendApplied;
            return true;
        }

        public override void destroy()
        {
            transparencyAnatomyCommand.NumericValueChanged -= transparencyAnatomyCommand_NumericValueChanged;
            transparencyAnatomyCommand.SmoothBlendApplied -= transparencyAnatomyCommand_SmoothBlendApplied;
        }

        public override AnatomyCommandUIType UIType
        {
            get { return AnatomyCommandUIType.Boolean; }
        }

        public override bool allowDisplay(AnatomyCommandPermissions permissions)
        {
            return (permissions &= AnatomyCommandPermissions.PremiumActive) == AnatomyCommandPermissions.PremiumActive;
        }

        [DoNotCopy]
        public override bool BooleanValue
        {
            get
            {
                return trackingTransparency;
            }
            set
            {
                if (trackingTransparency != value)
                {
                    trackingTransparency = value;
                    if (trackingTransparency)
                    {
                        transparencyInterface.CurrentAlpha = transparencyAnatomyCommand.NumericValue;
                    }
                    else
                    {
                        transparencyInterface.CurrentAlpha = 0.0f;
                    }
                    fireBooleanValueChanged(value);
                }
            }
        }

        public override string UIText
        {
            get { return uiText; }
        }

        void transparencyAnatomyCommand_NumericValueChanged(AnatomyCommand command, float value)
        {
            if (trackingTransparency)
            {
                transparencyInterface.CurrentAlpha = value;
            }
            else
            {
                transparencyInterface.CurrentAlpha = 0.0f;
            }
        }

        void transparencyAnatomyCommand_SmoothBlendApplied(float alpha, float duration, EasingFunction easingFunction)
        {
            if (trackingTransparency)
            {
                transparencyInterface.timedBlend(alpha, duration, easingFunction);
            }
            else
            {
                transparencyInterface.timedBlend(0.0f, duration, easingFunction);
            }
        }

        #region Saving

        public override void getInfo(SaveInfo info)
        {
            info.AddValue("uiText", uiText);
            info.AddValue("targetSimObjectName", targetSimObjectName);
            info.AddValue("targetTransparencyInterfaceName", targetTransparencyInterfaceName);
        }

        protected TransparencySectionAnatomyCommand(LoadInfo info)
        {
            uiText = info.GetString("uiText");
            targetSimObjectName = info.GetString("targetSimObjectName");
            targetTransparencyInterfaceName = info.GetString("targetTransparencyInterfaceName");
        }

        #endregion
    }
}
