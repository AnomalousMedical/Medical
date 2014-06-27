using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using BEPUikPlugin;

namespace Medical
{
    /// <summary>
    /// This class will adjust transparency for an object that is part of
    /// another object. It will either track the parent object or be disabled
    /// entirely.
    /// </summary>
    class PinIKAnatomyCommand : AbstractBooleanAnatomyCommand
    {
        [Editable]
        private String uiText = "Pin";

        [Editable]
        private String targetSimObjectName = "this";

        [Editable]
        private String targetIKBoneName = "IKBone";

        BEPUikBone ikBone;

        public PinIKAnatomyCommand()
        {

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage)
        {
            SimObject targetSimObject = owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                errorMessage = String.Format("Could not find target SimObject {0}", targetSimObjectName);
                return false;
            }
            ikBone = targetSimObject.getElement(targetIKBoneName) as BEPUikBone;
            if(ikBone == null)
            {
                errorMessage = String.Format("Could not find target IKBone {0} in SimObject {1}", targetIKBoneName, targetSimObjectName);
                return false;
            }
            return true;
        }

        public override AnatomyCommandUIType UIType
        {
            get { return AnatomyCommandUIType.Boolean; }
        }

        [DoNotCopy]
        public override bool BooleanValue
        {
            get
            {
                return ikBone.Pinned;
            }
            set
            {
                if (ikBone.Pinned != value)
                {
                    ikBone.Pinned = value;
                    fireBooleanValueChanged(value);
                }
            }
        }

        public override string UIText
        {
            get { return uiText; }
        }

        public override bool allowDisplay(AnatomyCommandPermissions permissions)
        {
            return (permissions & AnatomyCommandPermissions.Posing) != 0;
        }

        public override AnatomyCommand createTagGroupCommand()
        {
            CompoundAnatomyCommand compoundCommand = new CompoundAnatomyCommand(UIType, UIText);
            compoundCommand.addSubCommand(this);
            return compoundCommand;
        }

        public override void addToTagGroupCommand(AnatomyCommand tagGroupCommand)
        {
            ((CompoundAnatomyCommand)tagGroupCommand).addSubCommand(this);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue("uiText", uiText);
            info.AddValue("targetSimObjectName", targetSimObjectName);
            info.AddValue("targetIKBoneName", targetIKBoneName);
        }

        protected PinIKAnatomyCommand(LoadInfo info)
        {
            uiText = info.GetString("uiText");
            targetSimObjectName = info.GetString("targetSimObjectName");
            targetIKBoneName = info.GetString("targetIKBoneName");
        }
    }
}
