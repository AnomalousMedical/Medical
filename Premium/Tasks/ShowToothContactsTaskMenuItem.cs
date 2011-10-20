using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ShowToothContactsTask : Task
    {
        public ShowToothContactsTask(int weight)
            : base("Medical.ShowOcclusalContacts", "Show Occlusal Contacts", "ShowTeethContactsIcon", TaskMenuCategories.Simulation)
        {
            this.Weight = weight;
            this.ShowOnTimelineTaskbar = true;
        }

        public override void clicked()
        {
            ShowOnTaskbar = TeethController.HighlightContacts = !TeethController.HighlightContacts;
            if (!TeethController.HighlightContacts)
            {
                fireItemClosed();
            }
        }

        public override bool Active
        {
            get { return TeethController.HighlightContacts; }
        }
    }
}
