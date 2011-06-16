﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

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
        protected TimelineWizard timelineWizard;

        public TimelineWizardPanel(String layoutFile, TimelineWizard timelineWizard)
            :base(layoutFile)
        {
            this.timelineWizard = timelineWizard;
        }

        public override void initialize(ShowTimelineGUIAction showGUIAction)
        {
            this.ShowGUIAction = showGUIAction;
            TimelineWizardPanelData panelData = ShowGUIAction.GUIData as TimelineWizardPanelData;
            if (panelData != null)
            {
                NextTimeline = panelData.NextTimeline;
            }
            else
            {
                Log.Warning("Could not find TimelineWizardPanelData or subclass in panel {0}.", showGUIAction.GUIName);
            }
        }

        public override void show(GUIManager guiManager)
        {
            timelineWizard.show(this);
        }

        public override void hide(GUIManager guiManager)
        {
            timelineWizard.hide();
        }

        public virtual void opening(MedicalController medicalController, SimulationScene simScene)
        {

        }

        public virtual void closing()
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

        public String NextTimeline { get; private set; }
    }
}
