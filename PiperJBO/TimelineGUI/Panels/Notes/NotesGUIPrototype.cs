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
        private ImageRenderer imageRenderer;

        public NotesGUIPrototype(TimelineWizard wizard, ImageRenderer imageRenderer)
        {
            this.wizard = wizard;
            this.imageRenderer = imageRenderer;
        }

        public TimelineGUI getGUI()
        {
            return new NotesGUI(wizard, imageRenderer);
        }

        public string Name
        {
            get { return "PiperJBO.NotesGUI"; }
        }

    }
}
