using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class LeftDiscSpaceGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public LeftDiscSpaceGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new DiscSpaceGUI("LeftDiscSpace", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new DiscSpaceGUIData();
        }

        public string Name
        {
            get { return "PiperJBO.LeftDiscSpaceGUI"; }
        }
    }
}
