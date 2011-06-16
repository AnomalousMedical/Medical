using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.GUI
{
    class TimelineWizardPanelData : AbstractTimelineGUIData
    {
        public TimelineWizardPanelData()
        {

        }

        [Editable]
        public String NextTimeline { get; set; }

        protected TimelineWizardPanelData(LoadInfo info)
            :base(info)
        {

        }
    }
}
