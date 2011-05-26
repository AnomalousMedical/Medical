using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class RightDopplerGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RightDopplerGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new DopplerGUI("RightDoppler", wizard);
        }

        public string Name
        {
            get { return "PiperJBO.RightDopplerGUI"; }
        }
    }
}
