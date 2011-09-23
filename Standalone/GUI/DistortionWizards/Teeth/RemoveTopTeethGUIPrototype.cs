using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RemoveTopTeethGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public RemoveTopTeethGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new RemoveTeethGUI("Medical.GUI.DistortionWizards.Teeth.RemoveTopTeethGUI.layout", wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.RemoveTopTeethGUI"; }
        }

    }
}
