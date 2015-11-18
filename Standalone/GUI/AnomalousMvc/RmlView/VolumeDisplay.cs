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

        public VolumeDisplay(String volumeName, Element element, AnomalousMvcContext context, RocketWidget rocketWidget)
        {
            this.context = context;
            this.element = element;
            this.rocketWidget = rocketWidget;

            if (VolumeController.tryGetCalculator(volumeName, out calculator))
            {
                context.OnLoopUpdate += Context_OnLoopUpdate;
            }
            else
            {
                Log.Warning("Could not find a volume measurement named '{0}'. The volume will not be displayed.", volumeName);
                element.InnerRml = String.Format("Cannot find volume '{0}' in scene.", volumeName);
            }
        }

        public override void Dispose()
        {
            context.OnLoopUpdate -= Context_OnLoopUpdate;
            base.Dispose();
        }

        private void Context_OnLoopUpdate(Clock time)
        {
            float currentVolume = calculator.CurrentVolume;
            if (currentVolume != lastVolume)
            {
                rocketWidget.renderOnNextFrame();
                element.InnerRml = String.Format("{0} eu^3", currentVolume);
                lastVolume = currentVolume;
            }
        }
    }
}
