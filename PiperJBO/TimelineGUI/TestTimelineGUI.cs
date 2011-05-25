using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical
{
    class TestTimelineGUIPrototype : TimelineGUIFactoryPrototype
    {
        public TimelineGUI createGUI()
        {
            return new TestTimelineGUI();
        }

        public string Name
        {
            get { return "PiperJBO.TestTimelineGUI"; }
        }
    }

    public class TestTimelineGUI : Dialog, TimelineGUI
    {
        private ShowTimelineGUIAction showGUIAction;

        public TestTimelineGUI()
            : base("Medical.TimelineGUI.TestTimelineGUI.layout")
        {
            Button nextButton = window.findWidget("NextButton") as Button;
            nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);
        }

        public void initialize(ShowTimelineGUIAction showGUIAction)
        {
            this.showGUIAction = showGUIAction;
        }

        public void show(GUIManager guiManager)
        {
            this.Visible = true;
        }

        public void hide(GUIManager guiManager)
        {
            this.Visible = false;
        }

        protected override void onClosed(EventArgs args)
        {
            base.onClosed(args);
            this.Dispose();
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            showGUIAction.showNextTimeline();
            this.close();
        }
    }
}
