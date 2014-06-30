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
    class LockIKLimitAnatomyCommand : AbstractBooleanAnatomyCommand
    {
        [Editable]
        private String uiText = "Lock";

        [Editable]
        private String targetSimObjectName = "this";

        [DoNotSave]
        [DoNotCopy]
        List<BEPUikLimit> ikLimits = new List<BEPUikLimit>();

        public LockIKLimitAnatomyCommand()
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
            foreach(var element in targetSimObject.getElementIter())
            {
                BEPUikLimit limit = element as BEPUikLimit;
                if(limit != null)
                {
                    if(ikLimits.Count == 0)
                    {
                        limit.LockedChanged += limit_LockedChanged;
                    }
                    ikLimits.Add(limit);
                }
            }
            return true;
        }

        public override void destroy()
        {
            if(ikLimits.Count > 0)
            {
                ikLimits[0].LockedChanged -= limit_LockedChanged;
            }
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
                if(ikLimits.Count > 0)
                {
                    return ikLimits[0].Locked;
                }
                return false;
            }
            set
            {
                if(ikLimits.Count > 0 && ikLimits[0].Locked != value)
                {
                    foreach (var limit in ikLimits)
                    {
                        limit.Locked = value;
                    }
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
        }

        protected LockIKLimitAnatomyCommand(LoadInfo info)
        {
            uiText = info.GetString("uiText");
            targetSimObjectName = info.GetString("targetSimObjectName");
        }

        void limit_LockedChanged(BEPUikLimit obj)
        {
            fireBooleanValueChanged(obj.Locked);
        }
    }
}
