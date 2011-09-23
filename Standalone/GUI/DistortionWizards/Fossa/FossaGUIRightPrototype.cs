using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class FossaGUIRightPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public FossaGUIRightPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new FossaGUI("RightFossa", "Medical.GUI.DistortionWizards.Fossa.FossaGUIRight.layout", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.FossaGUIRight"; }
        }
    }
}
