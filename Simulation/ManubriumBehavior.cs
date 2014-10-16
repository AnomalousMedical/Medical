using BEPUik;
using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class ManubriumBehavior : Behavior
    {
        [Editable]
        private String chainStartSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        private BEPUikScene scene;

        [DoNotCopy]
        [DoNotSave]
        List<IKJoint> joints;

        protected override void link()
        {
            base.link();

            var chainStartSimObject = Owner.getOtherSimObject(chainStartSimObjectName);
            if(chainStartSimObject == null)
            {
                blacklist("Cannot find the chain start SimObject '{0}'", chainStartSimObjectName);
            }
            var ikBone = chainStartSimObject.getElement(boneName) as BEPUikBone;
            if(ikBone == null)
            {
                blacklist("Cannot find the ik bone '{0}' in chain start SimObject '{1}'", boneName, chainStartSimObjectName);
            }

            scene = ikBone.Scene;
            scene.Cheat_BgUpdateFinishing += scene_Cheat_BgUpdateFinishing;

            //Find all joints
            joints = scene.findAllJointsFrom(ikBone);

            ikBone.Pinned = true;
        }

        protected override void destroy()
        {
            scene.Cheat_BgUpdateFinishing -= scene_Cheat_BgUpdateFinishing;
            base.destroy();
        }

        void scene_Cheat_BgUpdateFinishing(BEPUikScene obj)
        {
            scene.solveJoints(joints);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            
        }
    }
}
