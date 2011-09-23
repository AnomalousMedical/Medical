using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class RightCondylarGrowthGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RightCondylarGrowthGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new RightCondylarGrowthGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.RightCondylarGrowthGUI"; }
        }
    }
}
