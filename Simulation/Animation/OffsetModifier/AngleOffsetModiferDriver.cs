using Engine;
using Engine.Attributes;
using Engine.Behaviors.Animation;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class AngleOffsetModiferDriver : BehaviorInterface, BlendDriver
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
        Vector3 firstTestVector = Vector3.Up;

        [Editable]
        Vector3 secondTestVector = Vector3.Up;

        [Editable]
        float maximumAngle;

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

        [DoNotCopy]
        [DoNotSave]
        float blend = 0.0f;

        [DoNotCopy]
        [DoNotSave]
        public event Action<BlendDriver> BlendAmountChanged;

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

            startingAngle = getAngle();

            firstAngleBroadcaster.PositionChanged += angleBroadcaster_PositionChanged;
            secondAngleBroadcaster.PositionChanged += angleBroadcaster_PositionChanged;

            addToDebugDrawing();
        }

        protected override void destroy()
        {
            firstAngleBroadcaster.PositionChanged -= angleBroadcaster_PositionChanged;
            secondAngleBroadcaster.PositionChanged -= angleBroadcaster_PositionChanged;
            base.destroy();
        }

        public float BlendAmount
        {
            get
            {
                return blend;
            }
        }

        void angleBroadcaster_PositionChanged(SimObject obj)
        {
            blend = (getAngle() - startingAngle) / maximumAngle;
            if(blend < 0.0f)
            {
                blend = 0.0f;
            }
            else if(blend > 1.0f)
            {
                blend = 1.0f;
            }
            if(BlendAmountChanged != null)
            {
                BlendAmountChanged.Invoke(this);
            }
        }

        public override void drawDebugInfo(DebugDrawingSurface debugDrawing)
        {
            Vector3 localAngle = Quaternion.quatRotate(firstAngleSimObject.Rotation.inverse(), Quaternion.quatRotate(secondAngleSimObject.Rotation, secondTestVector));
            
            debugDrawing.begin(String.Format("AngleOffsetModifierDriver_{0}_{1}", Owner.Name, Name), DrawingType.LineList);
            debugDrawing.Color = Color.Green;
            debugDrawing.drawLine(firstAngleSimObject.Translation, firstAngleSimObject.Translation + Quaternion.quatRotate(firstAngleSimObject.Rotation, firstTestVector));
            debugDrawing.Color = Color.Red;
            debugDrawing.drawLine(firstAngleSimObject.Translation, firstAngleSimObject.Translation + Quaternion.quatRotate(firstAngleSimObject.Rotation, localAngle));

            debugDrawing.end();
        }

        float getAngle()
        {
            Vector3 localAngle = Quaternion.quatRotate(firstAngleSimObject.Rotation.inverse(), Quaternion.quatRotate(secondAngleSimObject.Rotation, secondTestVector));
            //Logging.Log.Debug("{0} angle {1}", Owner.Name, firstTestVector.angle(ref localAngle));
            return firstTestVector.angle(ref localAngle);
        }
    }
}
