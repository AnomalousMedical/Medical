using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class IKChainParentBehavior : BehaviorInterface
    {
        [Editable]
        private String chainStartSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [Editable]
        private String solverName;

        [DoNotCopy]
        [DoNotSave]
        private BEPUikSolver solver;

        [DoNotCopy]
        [DoNotSave]
        Vector3 lastTranslation;

        [DoNotCopy]
        [DoNotSave]
        Quaternion lastRotation;

        [DoNotCopy]
        [DoNotSave]
        bool moved = false;

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

            solver = ikBone.Scene.getSolver(solverName);
            if(solver == null)
            {
                blacklist("Solver '{0}' cannot be found.", solverName);
            }
            solver.BeforeUpdate += solver_BeforeUpdate;

            lastTranslation = Owner.Translation;
            lastRotation = Owner.Rotation;
        }

        protected override void destroy()
        {
            solver.BeforeUpdate -= solver_BeforeUpdate;
            base.destroy();
        }

        protected override void positionUpdated()
        {
            moved = true;
            base.positionUpdated();
        }

        void solver_BeforeUpdate(BEPUikSolver solver)
        {
            //Need way to identify that we are moving as part of ik solving
            if (moved)
            {
                //solver.hack_movePinnedBonesAndControls(Owner.Translation - lastTranslation, lastRotation * Owner.Rotation.inverse());
                moved = false;
                lastTranslation = Owner.Translation;
                lastRotation = Owner.Rotation;

                //was calling the following 
                /*
                         public void hack_movePinnedBonesAndControls(Engine.Vector3 trans, Engine.Quaternion rot)
        {
            foreach(var bone in bones)
            {
                if(bone.Pinned)
                {
                    //nothing
                }
            }
            foreach(var control in controls)
            {
                BEPUikDragControl drag = control as BEPUikDragControl;
                if(drag != null)
                {
                    drag.TargetPosition = drag.Owner.Translation + trans;
                }
            }
        }
                 */
            }
        }
    }
}
