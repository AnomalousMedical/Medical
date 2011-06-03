using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using Engine.Saving;
using Engine.Editing;
using Engine;

namespace Medical
{
    /// <summary>
    /// A TimelineInstantAction to show a GUI.
    /// </summary>
    public class ShowTimelineGUIAction : TimelineInstantAction
    {
        [Editable]
        String testString = "TestStringValue";

        [Editable]
        int testInt = 111;

        TimelineGUI gui;

        public ShowTimelineGUIAction()
        {

        }

        /// <summary>
        /// Show the timeline specified by NextTimeline. If nothing is specified
        /// by this the timeline will shut down.
        /// </summary>
        public void showNextTimeline()
        {
            if (HasNextTimeline)
            {
                TimelineController.startPlayback(TimelineController.openTimeline(NextTimeline));
            }
            else
            {
                TimelineController._fireMultiTimelineStopEvent();
            }
        }

        public void stopTimelines()
        {
            TimelineController._fireMultiTimelineStopEvent();
        }

        public void playSpecificTimeline(String timelineName)
        {
            TimelineController.startPlayback(TimelineController.openTimeline(timelineName));
        }

        public override void doAction()
        {
            gui = TimelineController.GUIFactory.getGUI(GUIName);
            if (gui != null)
            {
                gui.initialize(this);
                gui.show(TimelineController.GUIManager);
            }
            else
            {
                TimelineController._fireMultiTimelineStopEvent();
            }
        }

        public override void dumpToLog()
        {
            Log.Debug("ShowTimelineGUIAction GUI: '{0}' Next Timeline '{1}'", GUIName, NextTimeline);
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            if (info.matchesPattern(NextTimeline))
            {
                info.addMatch(this.GetType(), "Next Timeline for GUI " + GUIName, NextTimeline);
            }
        }

        public EditInterface getEditInterface()
        {
            return ReflectedEditInterface.createEditInterface(this, BehaviorEditMemberScanner.Scanner, "ShowGUI", null);
        }

        public String NextTimeline { get; set; }

        public String GUIName { get; set; }

        public bool HasNextTimeline
        {
            get
            {
                return NextTimeline != null && NextTimeline != String.Empty;
            }
        }

        #region Saving

        protected ShowTimelineGUIAction(LoadInfo info)
            : base(info)
        {
            NextTimeline = info.GetString("NextTimeline", null);
            GUIName = info.GetString("GUIName", null);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("NextTimeline", NextTimeline);
            info.AddValue("GUIName", GUIName);
        }

        #endregion
    }
}
