using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RemoveBottomTeethGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RemoveBottomTeethGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new RemoveTeethGUI("Medical.TimelineGUI.Panels.Teeth.RemoveBottomTeethGUI.layout", wizard);
        }

        public string Name
        {
            get { return "PiperJBO.RemoveBottomTeethGUI"; }
        }

    }
}
