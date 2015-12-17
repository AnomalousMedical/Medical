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

        public MeasurementDisplay(Element element, AnomalousMvcContext context, RocketWidget rocketWidget)
        {
            this.context = context;
            this.element = element;
            this.rocketWidget = rocketWidget;

            String target = element.GetAttributeString("target");

            if (MeasurementController.tryGetCalculator(target, out measurement))
            {
                switch (element.GetAttributeString("units"))
                {
                    case "percent":
                        context.OnLoopUpdate += Context_OnLoopUpdatePercent;
                        break;
                    case "centimeters":
                        context.OnLoopUpdate += Context_OnLoopUpdateCm;
                        break;
                    case "millimeters":
                    default:
                        context.OnLoopUpdate += Context_OnLoopUpdateMm;
                        break;
                }
            }
            else
            {
                Log.Warning("Could not find a measurement named '{0}'. The measurement will not be displayed.", target);
                element.InnerRml = String.Format("Cannot find measurement '{0}' in scene.", target);
            }
        }

        public override void Dispose()
        {
            context.OnLoopUpdate -= Context_OnLoopUpdatePercent;
            context.OnLoopUpdate -= Context_OnLoopUpdateCm;
            context.OnLoopUpdate -= Context_OnLoopUpdateMm;
            base.Dispose();
        }

        private void Context_OnLoopUpdatePercent(Clock time)
        {
            float currentValue = measurement.CurrentDelta;
            if (currentValue != lastValue)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0}%", currentValue / measurement.StartingDelta * 100.0f);
                lastValue = currentValue;
            }
        }

        private void Context_OnLoopUpdateCm(Clock time)
        {
            float currentValue = SimulationConfig.GetCm(measurement.CurrentDelta);
            if (currentValue != lastValue)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0} cm", currentValue);
                lastValue = currentValue;
            }
        }

        private void Context_OnLoopUpdateMm(Clock time)
        {
            float currentValue = SimulationConfig.GetMm(measurement.CurrentDelta);
            if (currentValue != lastValue)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0} mm", currentValue);
                lastValue = currentValue;
            }
        }
    }
}
