using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Medical
{
    public class StartEmbeddedTimelineTask : Task
    {
        private String startTimeline;
        private TimelineController timelineController;
        private Assembly assembly;
        private String resourceRoot;

        public StartEmbeddedTimelineTask(String uniqueName, String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String startTimeline, TimelineController timelineController)
            : this(uniqueName, name, iconName, category, typeInAssembly, resourceRoot, startTimeline, timelineController, DEFAULT_WEIGHT)
        {

        }

        public StartEmbeddedTimelineTask(String uniqueName, String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String startTimeline, TimelineController timelineController, int weight)
            :base(uniqueName, name, iconName, category)
        {
            this.ShowOnTaskbar = false;
            this.Weight = weight;
            this.startTimeline = startTimeline;
            this.timelineController = timelineController;
            this.assembly = typeInAssembly.Assembly;
            this.resourceRoot = resourceRoot;
        }

        public override void clicked(TaskPositioner positioner)
        {
            timelineController.ResourceProvider = new TimelineEmbeddedResourceProvider(assembly, resourceRoot); ;
            Timeline start = timelineController.openTimeline(startTimeline);
            timelineController.TEMP_MVC_CORE.hideMainInterface(!start.Fullscreen);
            timelineController.startPlayback(start);
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
