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
        String prettyName;

        [Editable]
        String category;

        [Editable]
        int index;

        [Editable]
        Color color = Color.Blue;

        [Editable]
        Vector3 measurementScale = Vector3.ScaleIdentity;

        SimObject deltaSimObject;

        [DoNotSave]
        Vector3 lastLength = Vector3.ScaleIdentity * float.MinValue;

        [DoNotSave]
        float currentDelta;

        [DoNotSave]
        float startingDelta;

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

            startingDelta = calculateDelta().length();
            if(startingDelta == 0.0f) //Handle 0, just be a straight scaler in that case, really pretty unlikely
            {
                startingDelta = 1.0f;
            }
        }

        protected override void destroy()
        {
            MeasurementController.removeMeasurement(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            Vector3 diff = calculateDelta();
            if (diff != lastLength)
            {
                lastLength = diff;
                currentDelta = diff.length();
                if (MeasurementChanged != null)
                {
                    MeasurementChanged.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Current delta between the points in engine units
        /// </summary>
        public float CurrentDelta
        {
            get
            {
                return currentDelta;
            }
        }

        public float StartingDelta
        {
            get
            {
                return startingDelta;
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

        public String PrettyName
        {
            get
            {
                return prettyName;
            }
            set
            {
                prettyName = value;
            }
        }

        public String Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        internal void draw(MeasurementDrawer drawer)
        {
            drawer.drawLine(color, Owner.Translation, deltaSimObject.Translation);
        }

        private Vector3 calculateDelta()
        {
            return (deltaSimObject.Translation - Owner.Translation) * measurementScale;
        }
    }
}
