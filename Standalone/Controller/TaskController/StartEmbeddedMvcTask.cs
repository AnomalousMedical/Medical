using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    public class StartEmbeddedMvcTask : Task
    {
        private String mvcContextName;
        private TimelineController timelineController;
        private Assembly assembly;
        private String resourceRoot;
        private AnomalousMvcCore mvcCore;
        private AnomalousMvcContext context;
        private bool sortFiles = false;

        public StartEmbeddedMvcTask(String uniqueName, String name, String iconName, String category, Type typeInAssembly, String resourceRoot, String mvcContextName, TimelineController timelineController, AnomalousMvcCore mvcCore, bool sortFiles = false, int weight = DEFAULT_WEIGHT)
            :base(uniqueName, name, iconName, category)
        {
            this.mvcCore = mvcCore;
            this.ShowOnTaskbar = false;
            this.Weight = weight;
            this.mvcContextName = mvcContextName;
            this.timelineController = timelineController;
            this.assembly = typeInAssembly.Assembly;
            this.resourceRoot = resourceRoot;
            this.sortFiles = sortFiles;
        }

        public override void clicked(TaskPositioner positioner)
        {
            EmbeddedResourceProvider embeddedResourceProvider = new EmbeddedResourceProvider(assembly, resourceRoot);
            if (sortFiles)
            {
                embeddedResourceProvider.sortFiles(NaturalSortAlgorithm.CompareFunc);
            }
            timelineController.setResourceProvider(embeddedResourceProvider);

            //Load and run the mvc context
            context = mvcCore.loadContext(embeddedResourceProvider.openFile(mvcContextName));
            context.RuntimeName = UniqueName;
            context.setResourceProvider(embeddedResourceProvider);
            mvcCore.startRunningContext(context);
        }

        public override bool Active
        {
            get { return false; }
        }
    }
}
