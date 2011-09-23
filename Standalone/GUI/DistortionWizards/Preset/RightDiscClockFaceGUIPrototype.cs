using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class RightDiscClockFaceGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RightDiscClockFaceGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new PresetStateGUI("RightDisc", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.RightDiscClockFaceGUI"; }
        }
    }
}
