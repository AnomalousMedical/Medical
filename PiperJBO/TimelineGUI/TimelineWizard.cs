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
        private TimelineWizardPanel currentPanel;
        private TimelineWizardPanel lastPanel;
        private GUIManager guiManager;
        private bool wizardInterfaceShown = false;

        public TimelineWizard(UpdateTimer mainTimer, GUIManager guiManager)
        {
            this.guiManager = guiManager;

            screenLayout = new BorderLayoutContainer();
            timelineGUIButtons = new TimelineGUIButtons(this);
            screenLayout.Top = timelineGUIButtons.LayoutContainer;
            crossFadeContainer = new CrossFadeLayoutContainer(mainTimer);
            screenLayout.Center = crossFadeContainer;

            timelineGUIButtons.setPreviousButtonActive(false);
        }

        public void Dispose()
        {
            timelineGUIButtons.Dispose();
        }

        public void show(TimelineWizardPanel panel)
        {
            lastPanel = currentPanel;
            currentPanel = panel;
            if (!wizardInterfaceShown)
            {
                guiManager.changeLeftPanel(screenLayout);
                wizardInterfaceShown = true;
            }
            crossFadeContainer.changePanel(panel.Container, 0.25f, animationCompleted);
            timelineGUIButtons.setNextButtonActive(panel.ShowGUIAction.HasNextTimeline);
        }

        public void hide()
        {
            lastPanel = currentPanel;
            currentPanel = null;
            crossFadeContainer.changePanel(null, 0.25f, animationCompleted);
            guiManager.changeLeftPanel(null);
            wizardInterfaceShown = false;
        }

        public void finish()
        {
            if (currentPanel != null)
            {
                currentPanel.ShowGUIAction.stopTimelines();
                hide();
            }
        }

        public void next()
        {
            if (currentPanel != null)
            {
                currentPanel.ShowGUIAction.showNextTimeline();
            }
        }

        public void previous()
        {
            //Does nothing right now
        }

        public void cancel()
        {
            if (currentPanel != null)
            {
                currentPanel.ShowGUIAction.stopTimelines();
                hide();
            }
        }

        private void animationCompleted(LayoutContainer oldChild)
        {
            if (lastPanel != null)
            {
                lastPanel.Dispose();
                lastPanel = null;
            }
        }
    }
}
