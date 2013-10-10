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

            //doIk();            
            CCD3d(targetObj.Translation);

            //Sync new positions back to sim object
            foreach (IKJoint jj in joints)
            {
                jj.updateSimObjectPosition();
            }
        }

        //Newer 3d game engine design method
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

        //CCD 3d Method from Darwin3d
        //http://www.darwin3d.com/gdm1998.htm#gdm0998

        [Editable]
        int MAX_IK_TRIES = 100;		// TIMES THROUGH THE CCD LOOP (TRIES = # / LINKS) 

        [Editable]
        float IK_POS_THRESH = 1.0f;	// THRESHOLD FOR SUCCESS

        private bool CCD3d(Vector3 endPos)
        {
            //Start of CCD3d algo
            int EFFECTOR_POS = joints.Count - 1;
            Vector3 rootPos, curEnd, desiredEnd, targetVector, curVector, crossResult;
            double cosAngle, turnAngle, turnDeg; //Probably change this from double
            int link, tries;
            Quaternion aquat;

            //START AT THE LAST LINK IN THE CHAIN
            link = EFFECTOR_POS - 1;
            tries = 0;						// LOOP COUNTER SO I KNOW WHEN TO QUIT
            do
            {
                // THE COORDS OF THE X,Y,Z POSITION OF THE ROOT OF THIS BONE IS IN THE MATRIX
                // TRANSLATION PART WHICH IS IN THE 12,13,14 POSITION OF THE MATRIX
                //we have this directly from the sim object
                rootPos = joints[link].WorldTranslation;

                // POSITION OF THE END EFFECTOR
                curEnd = joints[EFFECTOR_POS].WorldTranslation;

                // DESIRED END EFFECTOR POSITION
                desiredEnd = endPos;

                // SEE IF I AM ALREADY CLOSE ENOUGH
                if (curEnd.distance2(ref desiredEnd) > IK_POS_THRESH)
                {
                    // CREATE THE VECTOR TO THE CURRENT EFFECTOR POS
                    curVector = curEnd - rootPos;

                    // CREATE THE DESIRED EFFECTOR POSITION VECTOR
                    targetVector = endPos - rootPos;

                    // NORMALIZE THE VECTORS (EXPENSIVE, REQUIRES A SQRT)
                    curVector.normalize();
                    targetVector.normalize();

                    // THE DOT PRODUCT GIVES ME THE COSINE OF THE DESIRED ANGLE
                    cosAngle = targetVector.dot(ref curVector);

                    // IF THE DOT PRODUCT RETURNS 1.0, I DON'T NEED TO ROTATE AS IT IS 0 DEGREES
                    if (cosAngle < 0.99999)
                    {
                        // USE THE CROSS PRODUCT TO CHECK WHICH WAY TO ROTATE
                        crossResult = curVector.cross(ref targetVector);
                        crossResult.normalize();
                        if (!crossResult.isNumber())
                        {
                            crossResult = Vector3.UnitZ;
                        }
                        turnAngle = Math.Acos(cosAngle);

                        //skipping degree conversion and damping

                        aquat = new Quaternion(crossResult, (float)turnAngle);
                        joints[link].LocalRotation *= aquat;

                        //skipping restrictions

                        // RECALC ALL THE MATRICES WITHOUT DRAWING ANYTHING
                        // In our case this updates all the IKJoint world transforms
                        updateJointTransforms();
                    }

                    if (--link < 0) link = EFFECTOR_POS - 1;	// START OF THE CHAIN, RESTART
                }
            } while (tries++ < MAX_IK_TRIES && curEnd.distance2(ref desiredEnd) > IK_POS_THRESH);

            return tries != MAX_IK_TRIES;
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
