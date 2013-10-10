using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.IK
{
    class IKChain : Behavior
    {
        enum Events
        {
            Step
        }

        static IKChain()
        {
            MessageEvent step = new MessageEvent(Events.Step);
            step.addButton(KeyboardButtonCode.KC_SPACE);
            DefaultEvents.registerDefaultEvent(step);
        }

        [Editable]
        private int iterations = 16;
        private List<IKGoal> goals = new List<IKGoal>();
        private List<IKJoint> joints = new List<IKJoint>();

        SimObject targetObj;

        bool step = false;
        [Editable]
        bool autoStep = false;

        protected override void constructed()
        {
            base.constructed();
        }

        protected override void link()
        {
            base.link();

            SimObject baseObj = Owner.getOtherSimObject("Base");
            IKJoint baseJoint = (IKJoint)baseObj.getElement("IKJoint");
            joints.Add(baseJoint);

            SimObject middleObj = Owner.getOtherSimObject("Middle");
            IKJoint middleJoint = (IKJoint)middleObj.getElement("IKJoint");
            middleJoint.Parent = baseJoint;
            joints.Add(middleJoint);

            SimObject endObj = Owner.getOtherSimObject("End");
            IKJoint endJoint = (IKJoint)endObj.getElement("IKJoint");
            endJoint.Parent = middleJoint;
            joints.Add(endJoint);

            SimObject endEffectorObj = Owner.getOtherSimObject("EndEffector");
            IKJoint endEffectorJoint = (IKJoint)endEffectorObj.getElement("IKJoint");
            endEffectorJoint.Parent = endJoint;
            joints.Add(endEffectorJoint);

            targetObj = Owner.getOtherSimObject("Target");

            goals.Add(new IKGoal(targetObj, endEffectorJoint));

            foreach (IKJoint j in joints)
            {
                j.computeLocalChainOffsets();
            }
            step = autoStep;
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (eventManager[Events.Step].FirstFrameDown)
            {
                step = true;
            }

            if (!step)
            {
                return;
            }

            step = autoStep;

            //Sync world transforms of root chain object
            joints[0].computeLocalChainOffsets();

            //Update World position of chain based on any transforms that would have happened to the base
            updateJointTransforms();

            doIk();            

            //Sync new positions back to sim object
            foreach (IKJoint jj in joints)
            {
                jj.updateSimObjectPosition();
            }
        }

        private void doIk()
        {
            int iter, i, j, k;
            IKJoint joint;
            int mNumJoints = joints.Count;

            for (iter = 0; iter < iterations; ++iter)
            {
                for (k = 0; k < mNumJoints; ++k)
                {
                    int r = mNumJoints - 1 - k;
                    joint = joints[r];

                    //Skipping translation for now

                    for (i = 0; i < 3; ++i)
                    {
                        if (joint.AllowRotation(i))
                        {
                            if (joint.UpdateLocalR(i, goals))
                            {
                                for (j = r; j < mNumJoints; ++j)
                                {
                                    joints[j].updateWorldTransforms();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void updateJointTransforms(int startIndex)
        {
            for (int i = startIndex; i < joints.Count; i++)
            {
                joints[i].updateWorldTransforms();
            }
        }

        private void updateJointTransforms()
        {
            foreach (IKJoint j in joints)
            {
                j.updateWorldTransforms();
            }
        }
    }
}
