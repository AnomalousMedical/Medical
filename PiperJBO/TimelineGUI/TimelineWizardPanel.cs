using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    /// <summary>
    /// A single panel in a TimelineWizard.
    /// 
    /// It must be shown with the TimelineWizard show function or it will not be
    /// disposed properly. That class handles the memory management and has more
    /// info in its description.
    /// </summary>
    public class TimelineWizardPanel : MyGUITimelineGUI
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

        public virtual void setSceneProperties(MedicalController medicalController, SimulationScene simScene)
        {

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
