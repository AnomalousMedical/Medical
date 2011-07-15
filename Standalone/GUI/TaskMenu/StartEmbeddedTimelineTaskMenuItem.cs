using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Medical.GUI
{
    /// <summary>
    /// This class can be used by plugins to start their main timelines.
    /// </summary>
    public class StartEmbeddedTimelineTaskMenuItem : TaskMenuItem
    {
        private String startTimeline;
        private TimelineController timelineController;
        private Assembly assembly;
        private String resourceRoot;

        public StartEmbeddedTimelineTaskMenuItem(String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String startTimeline, TimelineController timelineController)
            : this(name, iconName, category, typeInAssembly, resourceRoot, startTimeline, timelineController, DEFAULT_WEIGHT)
        {

        }

        public StartEmbeddedTimelineTaskMenuItem(String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String startTimeline, TimelineController timelineController, int weight)
            :base(name, iconName, category)
        {
            this.ShowOnTaskbar = false;
            this.Weight = weight;
            this.startTimeline = startTimeline;
            this.timelineController = timelineController;
            this.assembly = typeInAssembly.Assembly;
            this.resourceRoot = resourceRoot;
        }

        public override void clicked()
        {
            timelineController.ResourceProvider = new TimelineEmbeddedResourceProvider(assembly, resourceRoot); ;
            Timeline start = timelineController.openTimeline(startTimeline);
            timelineController.startPlayback(start);
        }
    }
}
