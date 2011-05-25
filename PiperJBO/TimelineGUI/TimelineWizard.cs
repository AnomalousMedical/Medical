using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical.GUI
{
    public class TimelineWizard : IDisposable
    {
        //UI
        private BorderLayoutContainer screenLayout;
        private CrossFadeLayoutContainer crossFadeContainer;
        private TimelineGUIButtons timelineGUIButtons;

        //State
        private ShowTimelineGUIAction showGUIAction;
        private GUIManager guiManager;

        public TimelineWizard(UpdateTimer mainTimer, GUIManager guiManager)
        {
            this.guiManager = guiManager;

            screenLayout = new BorderLayoutContainer();
            timelineGUIButtons = new TimelineGUIButtons(this);
            screenLayout.Top = timelineGUIButtons.LayoutContainer;
            crossFadeContainer = new CrossFadeLayoutContainer(mainTimer);
            screenLayout.Center = crossFadeContainer;
        }

        public void Dispose()
        {
            timelineGUIButtons.Dispose();
        }

        public void show(TimelineWizardPanel panel)
        {
            showGUIAction = panel.ShowGUIAction;
            crossFadeContainer.changePanel(panel.Container, 0.25f, null);
            guiManager.changeLeftPanel(screenLayout);
        }

        public void hide()
        {
            crossFadeContainer.changePanel(null, 0.25f, null);
            guiManager.changeLeftPanel(null);
            showGUIAction = null;
        }

        public void finish()
        {
            
        }

        public void next()
        {
            if (showGUIAction != null)
            {
                showGUIAction.showNextTimeline();
                hide();
                //LIFECYCLE, NEED TO DELETE OLD PANEL
            }
        }

        public void previous()
        {
            
        }

        public void cancel()
        {
            
        }
    }
}
