using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;
using System.IO;
using Logging;

namespace Medical
{
    class StartAnomalousMvcTask : DDPluginTask
    {
        public StartAnomalousMvcTask(String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {
            ShowOnTaskbar = false;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            VirtualFilesystemResourceProvider resourceProvider = new VirtualFilesystemResourceProvider(Plugin.PluginRootFolder);
            try
            {
                AnomalousMvcContext context;
                using (Stream stream = resourceProvider.openFile(ContextFile))
                {
                    context = Plugin.MvcCore.loadContext(stream);
                }
                context.setResourceProvider(resourceProvider);
                Plugin.TimelineController.setResourceProvider(resourceProvider);
                Plugin.MvcCore.startRunningContext(context);
            }
            catch (Exception ex)
            {
                Log.Error("Cannot load context '{0}'\nReason: {1}", ContextFile, ex.Message);
            }
        }

        [Editable]
        public String ContextFile { get; set; }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        protected StartAnomalousMvcTask(LoadInfo info)
            :base(info)
        {
            ContextFile = info.GetString("ContextFile");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("ContextFile", ContextFile);
        }
    }
}
