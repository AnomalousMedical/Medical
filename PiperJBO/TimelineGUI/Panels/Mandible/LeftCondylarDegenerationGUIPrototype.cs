using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LeftCondylarDegenerationGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public LeftCondylarDegenerationGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new LeftCondylarDegenerationGUI(wizard);
        }

        public string Name
        {
            get { return "PiperJBO.LeftCondylarDegenerationGUI"; }
        }
    }
}
