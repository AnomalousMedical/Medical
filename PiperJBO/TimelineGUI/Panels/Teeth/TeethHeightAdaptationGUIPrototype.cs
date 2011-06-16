using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TeethHeightAdaptationGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public TeethHeightAdaptationGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new TeethHeightAdaptationGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.TeethHeightAdaptationGUI"; }
        }
    }
}
