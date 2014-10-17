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
    class ManubriumBehavior : BehaviorInterface
    {
        [Editable]
        private String chainStartSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [DoNotCopy]
        [DoNotSave]
        private BEPUikSolver solver;

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

            solver = ikBone.Scene.getSolver("Arms");
            solver.BeforeUpdate += solver_BeforeUpdate;

            ikBone.Pinned = true;
        }

        protected override void destroy()
        {
            solver.BeforeUpdate -= solver_BeforeUpdate;
            base.destroy();
        }

        void solver_BeforeUpdate(BEPUikSolver solver)
        {

        }
    }
}
