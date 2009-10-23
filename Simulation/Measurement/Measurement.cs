using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Logging;

namespace Medical
{
    public class Measurement : Behavior
    {
        #if DEBUG_KEYS
        enum MeasurementEvents
        {
            Print,
        }

        static Measurement()
        {
            MessageEvent printDebugInfo = new MessageEvent(MeasurementEvents.Print);
            printDebugInfo.addButton(KeyboardButtonCode.KC_J);
            DefaultEvents.registerDefaultEvent(printDebugInfo);
        }
#endif

        private const float unitsToMM = 8.467f;

        [Editable]
        String deltaSimObjectName;

        [Editable]
        String measurementName;

        SimObject deltaSimObject;

        protected override void link()
        {
            deltaSimObject = Owner.getOtherSimObject(deltaSimObjectName);
            if (deltaSimObject == null)
            {
                blacklist("Cannot find delta sim object {0}.", deltaSimObjectName);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (eventManager[MeasurementEvents.Print].FirstFrameDown)
            {
                Log.Debug("{0} - {1}", MeasurementName, CurrentDelta);
            }
        }

        public override void drawDebugInfo(Engine.Renderer.DebugDrawingSurface debugDrawing)
        {
            base.drawDebugInfo(debugDrawing);
            debugDrawing.begin("Measurement", Engine.Renderer.DrawingType.LineList);
            debugDrawing.setColor(new Color(1.0f, 0.0f, 1.0f));
            debugDrawing.drawLine(Owner.Translation, deltaSimObject.Translation);
            debugDrawing.end();
        }

        public float CurrentDelta
        {
            get
            {
                return (deltaSimObject.Translation - Owner.Translation).length() * unitsToMM;
            }
        }

        public String MeasurementName
        {
            get
            {
                return measurementName;
            }
            set
            {
                measurementName = value;
            }
        }
    }
}
