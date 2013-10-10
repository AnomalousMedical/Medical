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
    //based on http://www.darwin3d.com/gdm1998.htm#gdm0998
    class IKChain : Behavior
    {
        const double ROTATE_SPEED = 1.0;		// SPEED OF ROTATION
        const int EFFECTOR_POS = 5;		// THIS CHAIN HAS 5 LINKS
        const int MAX_IK_TRIES = 100;		// TIMES THROUGH THE CCD LOOP (TRIES = # / LINKS) 
        const float IK_POS_THRESH = 1.0f;	// THRESHOLD FOR SUCCESS

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
            joints.Add((IKJoint)baseObj.getElement("IKJoint"));

            SimObject middleObj = Owner.getOtherSimObject("Middle");
            joints.Add((IKJoint)middleObj.getElement("IKJoint"));

            SimObject endObj = Owner.getOtherSimObject("End");
            joints.Add((IKJoint)endObj.getElement("IKJoint"));

            SimObject endEffectorObj = Owner.getOtherSimObject("EndEffector");
            joints.Add((IKJoint)endEffectorObj.getElement("IKJoint"));

            targetObj = Owner.getOtherSimObject("Target");

            goals.Add(new IKGoal());

            IKJoint parent = null;
            foreach (IKJoint j in joints)
            {
                j.computeLocalChainOffsets(parent);
                parent = j;
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            //if (eventManager[Events.Step].FirstFrameDown)
            //{
            //    step = true;
            //}

            //if (!step)
            //{
            //    return;
            //}

            //step = autoStep;

            //Sync world transforms of root chain object
            joints[0].computeLocalChainOffsets(null);

            //Update World position of chain based on any transforms that would have happened to the base

            updateJointTransforms();

            //Argument to CCD3d algo
            Vector3 endPos = targetObj.Translation;

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
                        turnAngle = Math.Acos(cosAngle);

                        //skipping degree conversion and damping

                        aquat = new Quaternion(crossResult, (float)turnAngle);
                        joints[link].LocalRotation *= aquat;

                        //skipping restrictions

                        // RECALC ALL THE MATRICES WITHOUT DRAWING ANYTHING
                        updateJointTransforms();
                    }

                    if (--link < 0) link = EFFECTOR_POS - 1;	// START OF THE CHAIN, RESTART
                }


            } while (tries++ < MAX_IK_TRIES && curEnd.distance2(ref desiredEnd) > IK_POS_THRESH);

            if (tries == MAX_IK_TRIES)
            {
                //return false;
            }
            else
            {
                //return true;
            }

            //End CCD3d Algo
            //Sync new positions back to sim object
            foreach (IKJoint jj in joints)
            {
                jj.updateSimObjectPosition();
            }
        }

        private void updateJointTransforms()
        {
            IKJoint parent = null;
            foreach (IKJoint j in joints)
            {
                j.updateWorldTransforms(parent);
                parent = j;
            }
        }
    }
}
