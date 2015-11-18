using Anomalous.libRocketWidget;
using Engine.Platform;
using libRocketPlugin;
using Logging;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI.AnomalousMvc
{
    class MeasurementDisplay : DataDisplay
    {
        private AnomalousMvcContext context;
        private Element element;
        private Measurement measurement;
        private float lastValue = float.MinValue;
        private RocketWidget rocketWidget;

        public MeasurementDisplay(String name, Element element, AnomalousMvcContext context, RocketWidget rocketWidget)
        {
            this.context = context;
            this.element = element;
            this.rocketWidget = rocketWidget;

            if (MeasurementController.tryGetCalculator(name, out measurement))
            {
                context.OnLoopUpdate += Context_OnLoopUpdate;
            }
            else
            {
                Log.Warning("Could not find a measurement named '{0}'. The measurement will not be displayed.", name);
                element.InnerRml = String.Format("Cannot find measurement '{0}' in scene.", name);
            }
        }

        public override void Dispose()
        {
            context.OnLoopUpdate -= Context_OnLoopUpdate;
            base.Dispose();
        }

        private void Context_OnLoopUpdate(Clock time)
        {
            float currentValue = measurement.CurrentDelta;
            if (currentValue != lastValue)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0} mm", currentValue);
                lastValue = currentValue;
            }
        }
    }
}
