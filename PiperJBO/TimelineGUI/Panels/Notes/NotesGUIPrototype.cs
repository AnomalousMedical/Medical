using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical.GUI
{
    class NotesGUIPrototype : TimelineGUIFactoryPrototype
    {
        private TimelineWizard wizard;

        public NotesGUIPrototype(TimelineWizard wizard)
        {
            this.wizard = wizard;
        }

        public TimelineGUI getGUI()
        {
            return new NotesGUI(wizard);
        }

        public TimelineGUIData getGUIData()
        {
            return new TimelineWizardPanelData();
        }

        public string Name
        {
            get { return "PiperJBO.NotesGUI"; }
        }

    }
}
