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
    /// This command will allow a anatomy identifier to pin an ik bone attached to it.
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

        public override void destroy()
        {
            
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
