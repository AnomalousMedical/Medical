using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;
using Logging;
using Engine.Attributes;

namespace Medical
{
    public delegate void MeasurementEvent(Measurement src);

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

        [Editable]
        String deltaSimObjectName;

        [Editable]
        String measurementName;

        [Editable]
        Color color = Color.Blue;

        [Editable]
        Vector3 measurementScale = Vector3.ScaleIdentity;

        SimObject deltaSimObject;

        [DoNotSave]
        Vector3 lastLength = Vector3.ScaleIdentity * float.MinValue;

        [DoNotSave]
        float currentDelta;

        [DoNotCopy]
        [DoNotSave]
        public event MeasurementEvent MeasurementChanged;

        protected override void link()
        {
            deltaSimObject = Owner.getOtherSimObject(deltaSimObjectName);
            if (deltaSimObject == null)
            {
                blacklist("Cannot find delta sim object {0}.", deltaSimObjectName);
            }
            MeasurementController.addMesurement(this);
        }

        protected override void destroy()
        {
            MeasurementController.removeMeasurement(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 diff = (deltaSimObject.Translation - Owner.Translation) * measurementScale;
            if (diff != lastLength)
            {
                lastLength = diff;
                currentDelta = diff.length() * SimulationConfig.UnitsToMM;
                if (MeasurementChanged != null)
                {
                    MeasurementChanged.Invoke(this);
                }
            }
        }

        public float CurrentDelta
        {
            get
            {
                return currentDelta;
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

        internal void draw(MeasurementDrawer drawer)
        {
            drawer.drawLine(color, Owner.Translation, deltaSimObject.Translation);
        }
    }
}
