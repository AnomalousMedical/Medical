using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class SceneAnatomyPin : BehaviorInterface, SceneAnatomyControl
    {
        [Editable]
        private String targetSimObjectName = "this";

        [Editable]
        private String targetIKBoneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        BEPUikBone ikBone;

        protected override void link()
        {
            base.link();

            SimObject targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Could not find target SimObject {0}", targetSimObjectName);
            }
            ikBone = targetSimObject.getElement(targetIKBoneName) as BEPUikBone;
            if (ikBone == null)
            {
                blacklist("Could not find target IKBone {0} in SimObject {1}", targetIKBoneName, targetSimObjectName);
            }

            SceneAnatomyControlManager.addControl(this);
        }

        protected override void destroy()
        {
            SceneAnatomyControlManager.removeControl(this);
            base.destroy();
        }

        public bool Active
        {
            get
            {
                return ikBone.Pinned;
            }
            set
            {
                ikBone.Pinned = value;
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                return Owner.Translation;
            }
        }
    }
}
