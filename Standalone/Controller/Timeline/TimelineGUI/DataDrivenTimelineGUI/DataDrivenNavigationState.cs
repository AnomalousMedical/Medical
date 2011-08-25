using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class DataDrivenNavigationState
    {
        private List<TimelineEntry> timelines = new List<TimelineEntry>();
        private int currentTimeline = 0;
        private String menuTimeline;

        public DataDrivenNavigationState(String menuTimeline)
        {
            this.menuTimeline = menuTimeline;
        }

        public void addTimeline(TimelineEntry timeline)
        {
            timelines.Add(timeline);
        }

        public void configureGUI(AbstractTimelineGUI gui)
        {
            gui.clearNavigationBar();
            foreach (TimelineEntry timeline in timelines)
            {
                gui.addToNavigationBar(timeline.Timeline, timeline.Name, timeline.ImageKey);
            }
            if (timelines.Count > 0)
            {
                gui.showNavigationBar();
            }
            else
            {
                gui.hideNavigationBar();
            }
        }

        public String getNameForTimeline(String timeline)
        {
            foreach (TimelineEntry entry in timelines)
            {
                if (entry.Timeline == timeline)
                {
                    return entry.Name;
                }
            }
            return null;
        }

        public String CurrentTimeline
        {
            get
            {
                if (currentTimeline < timelines.Count)
                {
                    return timelines[currentTimeline].Timeline;
                }
                return null;
            }
            set
            {
                currentTimeline = 0;
                foreach (TimelineEntry timeline in timelines)
                {
                    if (timeline.Timeline == value)
                    {
                        break;
                    }
                    ++currentTimeline;
                }
            }
        }

        public String PreviousTimeline
        {
            get
            {
                if (currentTimeline > 0)
                {
                    return timelines[currentTimeline - 1].Timeline;
                }
                return null;
            }
        }

        public String NextTimeline
        {
            get
            {
                if (currentTimeline + 1 < timelines.Count)
                {
                    return timelines[currentTimeline + 1].Timeline;
                }
                return null;
            }
        }

        public String MenuTimeline
        {
            get
            {
                return menuTimeline;
            }
        }
    }
}
