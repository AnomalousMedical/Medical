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
    class VolumeDisplay : DataDisplay
    {
        private AnomalousMvcContext context;
        private Element element;
        private VolumeCalculator calculator;
        private float lastVolume = float.MinValue;
        private RocketWidget rocketWidget;

        public VolumeDisplay(Element element, AnomalousMvcContext context, RocketWidget rocketWidget)
        {
            String volumeName = element.GetAttributeString("target");

            this.context = context;
            this.element = element;
            this.rocketWidget = rocketWidget;

            if (VolumeController.tryGetCalculator(volumeName, out calculator))
            {
                switch(element.GetAttributeString("units"))
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
                Log.Warning("Could not find a volume measurement named '{0}'. The volume will not be displayed.", volumeName);
                element.InnerRml = String.Format("Cannot find volume '{0}' in scene.", volumeName);
            }
        }

        public override void Dispose()
        {
            context.OnLoopUpdate -= Context_OnLoopUpdatePercent;
            context.OnLoopUpdate -= Context_OnLoopUpdateCm;
            context.OnLoopUpdate -= Context_OnLoopUpdateMm;
            base.Dispose();
        }

        private void Context_OnLoopUpdateCm(Clock time)
        {
            float currentVolume = SimulationConfig.GetCm(calculator.CurrentVolume);
            if (currentVolume != lastVolume)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0} cm^3", currentVolume);
                lastVolume = currentVolume;
            }
        }

        private void Context_OnLoopUpdateMm(Clock time)
        {
            float currentVolume = SimulationConfig.GetMm(calculator.CurrentVolume);
            if (currentVolume != lastVolume)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0} mm^3", currentVolume);
                lastVolume = currentVolume;
            }
        }

        private void Context_OnLoopUpdatePercent(Clock time)
        {
            float currentVolume = calculator.CurrentVolume;
            if (currentVolume != lastVolume)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0}%", currentVolume / calculator.InitialVolume * 100.0f);
                lastVolume = currentVolume;
            }
        }
    }
}
