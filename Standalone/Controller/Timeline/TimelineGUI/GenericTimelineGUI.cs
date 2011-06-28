using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Medical.GUI;

namespace Medical
{
    /// <summary>
    /// This class provides a way of having a timeline GUI that is created in
    /// its prototype and destroyed when it is finished. It can also play other
    /// timelines and close itself.
    /// </summary>
    /// <typeparam name="TimelineGUIDataType">The type of the TimelineGUIData associated with this class. It can be TimelineGUIData if no type is defined.</typeparam>
    public class GenericTimelineGUI<TimelineGUIDataType> : MyGUITimelineGUI
        where TimelineGUIDataType : TimelineGUIData
    {
        private TimelineGUIDataType guiData;
        private ShowTimelineGUIAction showTimelineAction;
        private GUIManager guiManager;

        public GenericTimelineGUI(String layoutFile)
            : base(layoutFile)
        {

        }

        //public override void Dispose()
        //{
        //    Logging.Log.Debug("GenericTimelineGUI {0} destroyed.", GetType().Name);
        //    base.Dispose();
        //}

        public override void initialize(ShowTimelineGUIAction showTimelineAction)
        {
            this.showTimelineAction = showTimelineAction;
            guiData = (TimelineGUIDataType)showTimelineAction.GUIData;
        }

        public override sealed void show(GUIManager guiManager)
        {
            this.guiManager = guiManager;
            guiManager.changeLeftPanel(layoutContainer);
            onShown();
        }

        protected virtual void onShown()
        {

        }

        public override sealed void hide(GUIManager guiManager)
        {
            close();
        }

        public void playExampleTimeline(String timeline)
        {
            showTimelineAction.playTimeline(timeline, false);
        }

        public void closeAndPlayTimeline(String timeline)
        {
            close();
            showTimelineAction.playTimeline(timeline);
        }

        public void closeAndReturnToMainGUI()
        {
            close();
            showTimelineAction.stopTimelines();
        }

        /// <summary>
        /// Helper to close the window, does not signal timeline playback stop.
        /// </summary>
        private void close()
        {
            //Here we change the left panel back to null and have an anoymous delegate that calls Dispose when the animation is completed.
            guiManager.changeLeftPanel(null, delegate(LayoutContainer oldChild)
            {
                //The lifecycle is to create a new gui in the prototype so it must be disposed here when we are done with it.
                Dispose();
            });
        }

        public TimelineGUIDataType GUIData
        {
            get
            {
                return guiData;
            }
        }
    }
}
