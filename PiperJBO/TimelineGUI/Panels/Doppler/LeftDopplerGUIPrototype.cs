using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LeftDopplerGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public LeftDopplerGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new DopplerGUI("LeftDoppler", wizard);
        }

        public string Name
        {
            get { return "PiperJBO.LeftDopplerGUI"; }
        }
    }
}
