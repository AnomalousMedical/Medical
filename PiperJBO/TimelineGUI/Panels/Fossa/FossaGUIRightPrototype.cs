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
            return new FossaGUI("RightFossa", "Medical.TimelineGUI.Panels.Fossa.FossaGUIRight.layout", wizard);
        }

        public string Name
        {
            get { return "PiperJBO.FossaGUIRight"; }
        }
    }
}
