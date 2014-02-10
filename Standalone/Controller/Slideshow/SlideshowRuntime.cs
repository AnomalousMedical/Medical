using Engine.Platform;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.Platform;
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
        enum Events
        {
            Next,
            Back,
            ZoomIn,
            ZoomOut,
            Close
        }

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
        private SlideDisplayManager displayManager;
        private EventContext eventContext;
        private GUIManager guiManager;

        public SlideshowRuntime(List<Slide> slides, ResourceProvider resourceProvider, GUIManager guiManager, int startIndex, TaskController additionalTasks)
        {
            this.guiManager = guiManager;

            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(SlideshowProps.BaseContextProperties.File))
            {
                mvcContext = SharedXmlSaver.Load<AnomalousMvcContext>(resourceStream);
            }
            navModel = (NavigationModel)mvcContext.Models[SlideshowProps.BaseContextProperties.NavigationModel];
            displayManager = new SlideDisplayManager();
            foreach (Slide slide in slides)
            {
                String slideName = slide.UniqueName;
                slide.setupContext(mvcContext, slideName, resourceProvider, displayManager);

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
            taskbar.Close += close;
            previousTask = new CallbackTask("Slideshow.Back", "Back", PreviousTaskName, "None", arg =>
            {
                back();
            });
            nextTask = new CallbackTask("Slideshow.Forward", "Forward", NextTaskName, "None", arg =>
            {
                next();
            });
            taskbar.addItem(new TaskTaskbarItem(previousTask));
            taskbar.addItem(new TaskTaskbarItem(nextTask));
            taskbar.addItem(new TaskTaskbarItem(new CallbackTask("Slideshow.ToggleMode", "Toggle Display Mode", "SlideshowIcons/NormalVectorToggle", "None", arg =>
            {
                displayManager.VectorMode = !displayManager.VectorMode;
                guiManager.layout();
            })));
            taskbar.addItem(new TaskTaskbarItem(new CallbackTask("Slideshow.ZoomIn", "Zoom In", "SlideshowIcons/ZoomIn", "None", arg =>
            {
                zoomIn();
            })));
            taskbar.addItem(new TaskTaskbarItem(new CallbackTask("Slideshow.ZoomOut", "Zoom Out", "SlideshowIcons/ZoomOut", "None", arg =>
            {
                zoomOut();
            })));

            eventContext = new EventContext();
            MessageEvent nextEvent = new MessageEvent(Events.Next);
            nextEvent.addButton(KeyboardButtonCode.KC_RIGHT);
            nextEvent.FirstFrameUpEvent += eventManager =>
            {
                next();
            };
            eventContext.addEvent(nextEvent);

            MessageEvent backEvent = new MessageEvent(Events.Back);
            backEvent.addButton(KeyboardButtonCode.KC_LEFT);
            backEvent.FirstFrameUpEvent += eventManager =>
            {
                back();
            };
            eventContext.addEvent(backEvent);

            MessageEvent zoomInEvent = new MessageEvent(Events.ZoomIn);
            zoomInEvent.addButton(KeyboardButtonCode.KC_EQUALS);
            zoomInEvent.FirstFrameUpEvent += eventManager =>
            {
                zoomIn();
            };
            eventContext.addEvent(zoomInEvent);

            MessageEvent zoomOutEvent = new MessageEvent(Events.ZoomOut);
            zoomOutEvent.addButton(KeyboardButtonCode.KC_MINUS);
            zoomOutEvent.FirstFrameUpEvent += eventManager =>
            {
                zoomOut();
            };
            eventContext.addEvent(zoomOutEvent);

            MessageEvent closeEvent = new MessageEvent(Events.Close);
            closeEvent.addButton(KeyboardButtonCode.KC_ESCAPE);
            closeEvent.FirstFrameUpEvent += eventManager =>
            {
                ThreadManager.invoke(close); //Delay so we do not modify the input collection
            };
            eventContext.addEvent(closeEvent);

            foreach (Task task in additionalTasks.Tasks)
            {
                taskbar.addItem(new TaskTaskbarItem(task));
            }

            mvcContext.Blurred += (ctx) =>
            {
                guiManager.deactivateLink(SlideTaskbarName);
                guiManager.removeLinkFromChain(taskbarLink);
                GlobalContextEventHandler.disableEventContext(eventContext);
            };
            mvcContext.Focused += (ctx) =>
            {
                guiManager.addLinkToChain(taskbarLink);
                guiManager.pushRootContainer(SlideTaskbarName);
                setNavigationIcons();
                GlobalContextEventHandler.setEventContext(eventContext);
            };
            mvcContext.RemovedFromStack += (ctx) =>
            {
                taskbar.Dispose();
            };
        }

        private void close()
        {
            mvcContext.runAction("Common/Close");
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

        private void zoomOut()
        {
            if (displayManager.AdditionalZoomMultiple > 1.7f)
            {
                displayManager.AdditionalZoomMultiple = 1.7f;
            }
            else if (displayManager.AdditionalZoomMultiple > 1.3f)
            {
                displayManager.AdditionalZoomMultiple = 1.3f;
            }
            else if (displayManager.AdditionalZoomMultiple > 1.0f)
            {
                displayManager.AdditionalZoomMultiple = 1.0f;
            }
            else if (displayManager.AdditionalZoomMultiple > 0.8f)
            {
                displayManager.AdditionalZoomMultiple = 0.8f;
            }
            else
            {
                displayManager.AdditionalZoomMultiple = 0.5f;
            }
            guiManager.layout();
        }

        private void zoomIn()
        {
            if (displayManager.AdditionalZoomMultiple < 0.8f)
            {
                displayManager.AdditionalZoomMultiple = 0.8f;
            }
            else if (displayManager.AdditionalZoomMultiple < 1.0f)
            {
                displayManager.AdditionalZoomMultiple = 1.0f;
            }
            else if (displayManager.AdditionalZoomMultiple < 1.3f)
            {
                displayManager.AdditionalZoomMultiple = 1.3f;
            }
            else if (displayManager.AdditionalZoomMultiple < 1.7f)
            {
                displayManager.AdditionalZoomMultiple = 1.7f;
            }
            else
            {
                displayManager.AdditionalZoomMultiple = 2.0f;
            }
            guiManager.layout();
        }

        private void next()
        {
            mvcContext.runAction("NavigationBug/Next");
            setNavigationIcons();
        }

        private void back()
        {
            mvcContext.runAction("NavigationBug/Previous");
            setNavigationIcons();
        }
    }
}
