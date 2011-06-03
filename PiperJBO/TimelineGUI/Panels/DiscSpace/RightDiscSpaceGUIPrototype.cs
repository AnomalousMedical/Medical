using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class RightDiscSpaceGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RightDiscSpaceGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new DiscSpaceGUI("RightDiscSpace", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return null;
        }

        public string Name
        {
            get { return "PiperJBO.RightDiscSpaceGUI"; }
        }
    }
}
