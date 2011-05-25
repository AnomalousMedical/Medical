using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineGUIButtons : IDisposable
    {
        private Layout stateWizardButtons;
        private Widget stateWizardButtonsRoot;

        private Button cancelButton;
        private Button previousButton;
        private Button nextButton;
        private Button finishButton;

        private MyGUILayoutContainer layoutContainer;
        private TimelineWizard panelManager;

        public TimelineGUIButtons(TimelineWizard panelManager)
        {
            this.panelManager = panelManager;

            stateWizardButtons = LayoutManager.Instance.loadLayout("Medical.TimelineGUI.TimelineGUIButtons.layout");
            stateWizardButtonsRoot = stateWizardButtons.getWidget(0);

            cancelButton = stateWizardButtonsRoot.findWidget("StateWizardButtons/Cancel") as Button;
            previousButton = stateWizardButtonsRoot.findWidget("StateWizardButtons/Previous") as Button;
            nextButton = stateWizardButtonsRoot.findWidget("StateWizardButtons/Next") as Button;
            finishButton = stateWizardButtonsRoot.findWidget("StateWizardButtons/Finish") as Button;

            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
            previousButton.MouseButtonClick += new MyGUIEvent(previousButton_MouseButtonClick);
            nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
            finishButton.MouseButtonClick += new MyGUIEvent(finishButton_MouseButtonClick);

            layoutContainer = new MyGUILayoutContainer(stateWizardButtonsRoot);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(stateWizardButtons);
        }

        public MyGUILayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        public void setPreviousButtonActive(bool active)
        {
            previousButton.Enabled = active;
        }

        public void setNextButtonActive(bool active)
        {
            nextButton.Enabled = active;
        }

        void finishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            panelManager.finish();
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            panelManager.next();
        }

        void previousButton_MouseButtonClick(Widget source, EventArgs e)
        {
            panelManager.previous();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            panelManager.cancel();
        }
    }
}
