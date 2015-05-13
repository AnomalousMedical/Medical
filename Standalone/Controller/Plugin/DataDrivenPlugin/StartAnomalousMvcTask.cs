﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;
using System.IO;
using Logging;
using Medical.Editor;
using MyGUIPlugin;
using Anomalous.GuiFramework;

namespace Medical
{
    public class StartAnomalousMvcTask : DDPluginTask
    {
        public StartAnomalousMvcTask(String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {
            ShowOnTaskbar = false;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            if (Plugin.AtlasPluginManager.allDependenciesLoadedFor(this.Plugin))
            {
                String fullContextPath = Path.Combine(Plugin.PluginRootFolder, ContextFile).Replace('\\', '/');
                VirtualFilesystemResourceProvider resourceProvider = new VirtualFilesystemResourceProvider(Path.GetDirectoryName(fullContextPath));
                try
                {
                    AnomalousMvcContext context;
                    using (Stream stream = resourceProvider.openFile(Path.GetFileName(fullContextPath)))
                    {
                        context = Plugin.MvcCore.loadContext(stream);
                    }
                    context.RuntimeName = UniqueName;
                    context.setResourceProvider(resourceProvider);
                    Plugin.TimelineController.setResourceProvider(resourceProvider);
                    Plugin.MvcCore.startRunningContext(context);
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot load context '{0}'\nReason: {1}", ContextFile, ex.Message);
                }
            }
            else
            {
                MessageBox.show("Additional files needed to run this task. Would you like to download them now?", "Files Needed", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                {
                    if (result == MessageBoxStyle.Yes)
                    {
                        Plugin.AtlasPluginManager.requestDependencyDownloadFor(this.Plugin);
                    }
                });
            }
        }

        [EditableFile("*.mvc", "MVC Files")]
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
            ShowOnTaskbar = false;
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("ContextFile", ContextFile);
        }
    }
}
