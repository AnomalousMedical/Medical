using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public abstract class TimelineWizardPanel : MyGUITimelineGUI
    {
        private TimelineWizard timelineWizard;

        public TimelineWizardPanel(String layoutFile, TimelineWizard timelineWizard)
            :base(layoutFile)
        {
            this.timelineWizard = timelineWizard;
        }

        public override void initialize(ShowTimelineGUIAction showGUIAction)
        {
            this.ShowGUIAction = showGUIAction;
        }

        public override void show(GUIManager guiManager)
        {
            timelineWizard.show(this);
        }

        public override void hide(GUIManager guiManager)
        {
            timelineWizard.hide();
        }

        public ShowTimelineGUIAction ShowGUIAction { get; set; }

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }
    }
}
