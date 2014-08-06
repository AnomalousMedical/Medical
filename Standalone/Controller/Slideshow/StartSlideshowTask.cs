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
using Engine.Saving.XMLSaver;
using MyGUIPlugin;

namespace Medical
{
    public class StartSlideshowTask : DDPluginTask
    {
        public StartSlideshowTask(String uniqueName, String name, String iconName, String category)
            : base(uniqueName, name, iconName, category)
        {
            ShowOnTaskbar = false;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            VirtualFilesystemResourceProvider resourceProvider = new VirtualFilesystemResourceProvider(Plugin.PluginRootFolder);
            try
            {
                Slideshow slideshow;
                using (Stream stream = resourceProvider.openFile(SlideshowFile))
                {
                    slideshow = SharedXmlSaver.Load<Slideshow>(stream);
                }
                if (slideshow.Version == Slideshow.CurrentVersion)
                {
                    AnomalousMvcContext context = slideshow.createContext(resourceProvider, Plugin.GuiManager);
                    context.RuntimeName = UniqueName;
                    context.setResourceProvider(resourceProvider);
                    Plugin.TimelineController.setResourceProvider(resourceProvider);
                    Plugin.MvcCore.startRunningContext(context);
                }
                else
                {
                    MessageBox.show(String.Format("Cannot run slideshow \"{0}.\" It was created in a different version of Anomalous Medical.\nYou will need to download an updated version.", Name), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                    InlineRmlUpgradeCache.removeSlideshowPanels(slideshow);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Cannot load context '{0}'\nReason: {1}", SlideshowFile, ex.Message);
            }
        }

        [EditableFile("*.show", "Slideshow Files")]
        public String SlideshowFile { get; set; }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        protected StartSlideshowTask(LoadInfo info)
            : base(info)
        {
            SlideshowFile = info.GetString("SlideshowFile");
            ShowOnTaskbar = false;
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("SlideshowFile", SlideshowFile);
        }
    }
}
