using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ProfileDistortionGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public ProfileDistortionGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new ProfileDistortionGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new ProfileDistortionGUIData();
        }

        public string Name
        {
            get { return "PiperJBO.ProfileDistortionGUI"; }
        }
    }
}
