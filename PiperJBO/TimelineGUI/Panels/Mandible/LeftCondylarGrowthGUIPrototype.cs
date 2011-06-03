using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LeftCondylarGrowthGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public LeftCondylarGrowthGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new LeftCondylarGrowthGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return null;
        }

        public string Name
        {
            get { return "PiperJBO.LeftCondylarGrowthGUI"; }
        }
    }
}
