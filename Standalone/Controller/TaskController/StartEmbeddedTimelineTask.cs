using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Medical.Controller.AnomalousMvc;
using Medical.Controller.AnomalousMvc.Legacy;

namespace Medical
{
    public class StartEmbeddedTimelineTask : Task
    {
        private String startTimeline;
        private TimelineController timelineController;
        private Assembly assembly;
        private String resourceRoot;
        private AnomalousMvcCore mvcCore;

        public StartEmbeddedTimelineTask(String uniqueName, String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String startTimeline, TimelineController timelineController, AnomalousMvcCore mvcCore)
            : this(uniqueName, name, iconName, category, typeInAssembly, resourceRoot, startTimeline, timelineController, mvcCore, DEFAULT_WEIGHT)
        {

        }

        public StartEmbeddedTimelineTask(String uniqueName, String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String startTimeline, TimelineController timelineController, AnomalousMvcCore mvcCore, int weight)
            :base(uniqueName, name, iconName, category)
        {
            this.mvcCore = mvcCore;
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
            //Have to load the timeline to know if it is fullscreen, technicly this loads it twice, but this code will be gone eventually
            Timeline start = timelineController.openTimeline(startTimeline);

            //Build a MvcContext
            AnomalousMvcContext context = new AnomalousMvcContext();
            context.StartupAction = "Common/Start";
            context.ShutdownAction = "Common/Shutdown";
            MvcController controller = new MvcController("Common");
            RunCommandsAction action = new RunCommandsAction("Start");
            PlayLegacyTimelineCommand playLegacyTimeline = new PlayLegacyTimelineCommand();
            playLegacyTimeline.Timeline = startTimeline;
            HideMainInterfaceCommand hideMainInterface = new HideMainInterfaceCommand();
            hideMainInterface.ShowSharedGui = !start.Fullscreen;
            action.addCommand(hideMainInterface);
            action.addCommand(playLegacyTimeline);
            controller.Actions.add(action);
            context.Controllers.add(controller);

            mvcCore.startRunningContext(context);
            //timelineController.TEMP_MVC_CORE.hideMainInterface(!start.Fullscreen);
            //timelineController.startPlayback(start);
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
