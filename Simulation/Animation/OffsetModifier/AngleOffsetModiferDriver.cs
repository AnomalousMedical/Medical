using Engine;
using Engine.Attributes;
using Engine.Behaviors.Animation;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class AngleOffsetModiferDriver : Interface
    {
        [Editable]
        String firstAngleSimObjectName;

        [Editable]
        String firstAngleBroadcasterName = "PositionBroadcaster";

        [Editable]
        String secondAngleSimObjectName;

        [Editable]
        String secondAngleBroadcasterName = "PositionBroadcaster";

        [Editable]
        String targetSimObjectName = "this";

        [Editable]
        String targetSequenceName = "OffsetModifierSequence";

        [Editable]
        Vector3 testVector = Vector3.Up;

        [Editable]
        float maximumAngle;

        [DoNotCopy]
        [DoNotSave]
        OffsetModifierSequence sequence;

        [DoNotCopy]
        [DoNotSave]
        SimObject firstAngleSimObject;

        [DoNotCopy]
        [DoNotSave]
        PositionBroadcaster firstAngleBroadcaster;

        [DoNotCopy]
        [DoNotSave]
        SimObject secondAngleSimObject;

        [DoNotCopy]
        [DoNotSave]
        PositionBroadcaster secondAngleBroadcaster;

        [DoNotCopy]
        [DoNotSave]
        float startingAngle;

        protected override void constructed()
        {
            base.constructed();
        }

        protected override void link()
        {
            base.link();

            firstAngleSimObject = Owner.getOtherSimObject(firstAngleSimObjectName);
            if (firstAngleSimObject == null)
            {
                blacklist("The first angle SimObject '{0}' could not be found.", firstAngleSimObjectName);
            }

            firstAngleBroadcaster = firstAngleSimObject.getElement(firstAngleBroadcasterName) as PositionBroadcaster;
            if(firstAngleBroadcaster == null)
            {
                blacklist("The first angle SimObject '{0}' does not have a position broadcaster named '{1}'.", firstAngleSimObjectName, firstAngleBroadcasterName);
            }

            secondAngleSimObject = Owner.getOtherSimObject(secondAngleSimObjectName);
            if (secondAngleSimObject == null)
            {
                blacklist("The second angle SimObject '{0}' could not be found.", secondAngleSimObjectName);
            }

            secondAngleBroadcaster = secondAngleSimObject.getElement(secondAngleBroadcasterName) as PositionBroadcaster;
            if (firstAngleBroadcaster == null)
            {
                blacklist("The second angle SimObject '{0}' does not have a position broadcaster named '{1}'.", secondAngleSimObjectName, secondAngleBroadcasterName);
            }

            SimObject targetSimObject = Owner.getOtherSimObject(targetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("The target SimObject '{0}' could not be found.", targetSimObjectName);
            }

            sequence = targetSimObject.getElement(targetSequenceName) as OffsetModifierSequence;
            if (sequence == null)
            {
                blacklist("The target SimObject '{0}' does not have a OffsetModifierSequence named '{1}'.", targetSimObjectName, targetSequenceName);
            }

            startingAngle = getAngle();

            firstAngleBroadcaster.PositionChanged += angleBroadcaster_PositionChanged;
            secondAngleBroadcaster.PositionChanged += angleBroadcaster_PositionChanged;
        }

        protected override void destroy()
        {
            firstAngleBroadcaster.PositionChanged -= angleBroadcaster_PositionChanged;
            secondAngleBroadcaster.PositionChanged -= angleBroadcaster_PositionChanged;
            base.destroy();
        }

        void angleBroadcaster_PositionChanged(SimObject obj)
        {
            float blend = (getAngle() - startingAngle) / maximumAngle;
            if(blend < 0.0f)
            {
                blend = 0.0f;
            }
            else if(blend > 1.0f)
            {
                blend = 1.0f;
            }
            sequence.blend(blend);
        }

        float getAngle()
        {
            Vector3 localAngle = secondAngleSimObject.Translation.toLocalTrans(firstAngleSimObject.Translation, firstAngleSimObject.Rotation);
            return testVector.angle(ref localAngle);
        }
    }
}
