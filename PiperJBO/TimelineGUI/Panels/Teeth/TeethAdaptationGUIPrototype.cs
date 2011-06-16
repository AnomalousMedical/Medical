using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TeethAdaptationGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public TeethAdaptationGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new TeethAdaptationGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.TeethAdaptationGUI"; }
        }
    }
}
