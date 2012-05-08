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
        private EmbeddedResourceProvider embeddedResourceProvider;
        private AnomalousMvcContext context;

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
            embeddedResourceProvider = new EmbeddedResourceProvider(assembly, resourceRoot);
            timelineController.setResourceProvider(embeddedResourceProvider);
            timelineController.LEGACY_MultiTimelineStoppedEvent += timelineController_LEGACY_MultiTimelineStoppedEvent;
            //Have to load the timeline to know if it is fullscreen, technicly this loads it twice, but this code will be gone eventually
            Timeline start = timelineController.openTimeline(startTimeline);

            //Build a MvcContext
            context = new AnomalousMvcContext();
            context.setResourceProvider(embeddedResourceProvider);
            context.AllowShutdown = false;
            context.StartupAction = "Common/Start";
            context.ShutdownAction = "Common/Shutdown";
            MvcController controller = new MvcController("Common");
            RunCommandsAction startAction = new RunCommandsAction("Start");
            PlayLegacyTimelineCommand playLegacyTimeline = new PlayLegacyTimelineCommand();
            playLegacyTimeline.Timeline = startTimeline;
            HideMainInterfaceCommand hideMainInterface = new HideMainInterfaceCommand();
            hideMainInterface.ShowSharedGui = !start.Fullscreen;
            startAction.addCommand(hideMainInterface);
            startAction.addCommand(playLegacyTimeline);
            controller.Actions.add(startAction);
            RunCommandsAction shutdownAction = new RunCommandsAction("Shutdown");
            shutdownAction.addCommand(new ShowMainInterfaceCommand());
            controller.Actions.add(shutdownAction);
            context.Controllers.add(controller);

            mvcCore.startRunningContext(context);
        }

        void timelineController_LEGACY_MultiTimelineStoppedEvent(object sender, EventArgs e)
        {
            context.AllowShutdown = true;
            context.shutdown();
            timelineController.setResourceProvider(null);
            embeddedResourceProvider.Dispose();
            embeddedResourceProvider = null;
            timelineController.LEGACY_MultiTimelineStoppedEvent -= timelineController_LEGACY_MultiTimelineStoppedEvent;
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
