using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.Platform;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// The core is the link from AnomalousMVC to the rest of the system.
    /// </summary>
    public class AnomalousMvcCore
    {
        private GUIManager guiManager;
        private TimelineController timelineController;
        private UpdateTimer updateTimer;

        private ViewHostFactory viewHostFactory;

        public AnomalousMvcCore(UpdateTimer updateTimer, GUIManager guiManager, TimelineController timelineController)
        {
            this.updateTimer = updateTimer;
            this.guiManager = guiManager;
            this.timelineController = timelineController;
        }

        public void changeLeftPanel(View view)
        {

        }

        public void stopTimelines()
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
            timelineController._fireMultiTimelineStopEvent();
        }

        public void stopPlayingExample()
        {
            if (timelineController.Playing)
            {
                timelineController.stopAndStartPlayback(null, false);
            }
        }

        public void playTimeline(String timelineName, bool allowPlaybackStop)
        {
            Timeline timeline = timelineController.openTimeline(timelineName);
            if (timeline != null)
            {
                timeline.AutoFireMultiTimelineStopped = allowPlaybackStop;
                if (timelineController.Playing)
                {
                    timelineController.stopAndStartPlayback(timeline, true);
                }
                else
                {
                    timelineController.startPlayback(timeline);
                }
            }
            else
            {
                Log.Warning("AnomalousMvcCore playback: Error loading timeline '{0}'", timelineName);
                stopTimelines();
            }
        }

    }
}
