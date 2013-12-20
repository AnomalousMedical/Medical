﻿using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medical
{
    class SlideshowRuntime
    {
        public const String SlideTaskbarName = "SlideTaskbar";
        private const String PreviousTaskName = "SlideshowIcons/Back";
        private const String PreviousTaskDisabledName = "SlideshowIcons/BackInactive";
        private const String NextTaskName = "SlideshowIcons/Forward";
        private const String NextTaskDisabledName = "SlideshowIcons/ForwardInactive";

        private AnomalousMvcContext mvcContext;
        private CallbackTask previousTask;
        private CallbackTask nextTask;
        private NavigationModel navModel;
        private ClosingTaskbar taskbar;
        private SingleChildChainLink taskbarLink;

        public SlideshowRuntime(List<Slide> slides, ResourceProvider resourceProvider, GUIManager guiManager, int startIndex, TaskController additionalTasks)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(SlideshowProps.BaseContextProperties.File))
            {
                mvcContext = SharedXmlSaver.Load<AnomalousMvcContext>(resourceStream);
            }
            navModel = (NavigationModel)mvcContext.Models[SlideshowProps.BaseContextProperties.NavigationModel];
            foreach (Slide slide in slides)
            {
                String slideName = slide.UniqueName;
                slide.setupContext(mvcContext, slideName, resourceProvider);

                NavigationLink link = new NavigationLink(slideName, null, slideName + "/Show");
                navModel.addNavigationLink(link);
            }

            RunCommandsAction runCommands = (RunCommandsAction)mvcContext.Controllers["Common"].Actions["Start"];
            runCommands.addCommand(new NavigateToIndexCommand()
            {
                Index = startIndex
            });

            taskbar = new ClosingTaskbar();
            taskbarLink = new SingleChildChainLink(SlideTaskbarName, taskbar);
            taskbar.Close += () => mvcContext.runAction("Common/Close");
            previousTask = new CallbackTask("Slideshow.Back", "Back", PreviousTaskName, "None", arg =>
            {
                mvcContext.runAction("NavigationBug/Previous");
                setNavigationIcons();
            });
            nextTask = new CallbackTask("Slideshow.Forward", "Forward", NextTaskName, "None", arg =>
            {
                mvcContext.runAction("NavigationBug/Next");
                setNavigationIcons();
            });
            taskbar.addItem(new TaskTaskbarItem(previousTask));
            taskbar.addItem(new TaskTaskbarItem(nextTask));

            foreach (Task task in additionalTasks.Tasks)
            {
                taskbar.addItem(new TaskTaskbarItem(task));
            }

            mvcContext.Blurred += (ctx) =>
            {
                guiManager.deactivateLink(SlideTaskbarName);
                guiManager.removeLinkFromChain(taskbarLink);
            };
            mvcContext.Focused += (ctx) =>
            {
                guiManager.addLinkToChain(taskbarLink);
                guiManager.pushRootContainer(SlideTaskbarName);
                setNavigationIcons();
            };
            mvcContext.RemovedFromStack += (ctx) =>
            {
                taskbar.Dispose();
            };
        }

        public AnomalousMvcContext Context
        {
            get
            {
                return mvcContext;
            }
        }

        private void setNavigationIcons()
        {
            previousTask.setIcon(navModel.HasPrevious ? PreviousTaskName : PreviousTaskDisabledName);
            nextTask.setIcon(navModel.HasNext ? NextTaskName : NextTaskDisabledName);
        }
    }
}
