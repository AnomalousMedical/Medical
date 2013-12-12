using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medical
{
    public class Slideshow : Saveable
    {
        public const String SlideThumbName = "Thumb.png";
        public const int BaseSlideScale = 1017;
        public const int CurrentVersion = 2;

        private static TaskController additionalTasks = new TaskController();

        public static TaskController AdditionalTasks
        {
            get
            {
                return additionalTasks;
            }
        }

        [DoNotSave]
        private List<Slide> slides = new List<Slide>();

        private int version;

        public Slideshow()
        {
            version = CurrentVersion;
        }

        public void addSlide(Slide slide)
        {
            slides.Add(slide);
        }

        public void removeSlide(Slide slide)
        {
            slides.Remove(slide);
        }

        public void removeAt(int index)
        {
            slides.RemoveAt(index);
        }

        public void insertSlide(Slide before, Slide insert)
        {
            insertSlide(slides.IndexOf(before), insert);
        }

        public void insertSlide(int index, Slide slide)
        {
            slides.Insert(index, slide);
        }

        public int indexOf(Slide slide)
        {
            return slides.IndexOf(slide);
        }

        public Slide get(int index)
        {
            return slides[index];
        }

        public AnomalousMvcContext createContext(ResourceProvider resourceProvider, GUIManager guiManager, int startIndex = 0)
        {
            AnomalousMvcContext mvcContext;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(SlideshowProps.BaseContextProperties.File))
            {
                mvcContext = SharedXmlSaver.Load<AnomalousMvcContext>(resourceStream);
            }
            NavigationModel navModel = (NavigationModel)mvcContext.Models[SlideshowProps.BaseContextProperties.NavigationModel];
            int i = 0;
            int lastSlideIndex = slides.Count - 1;
            foreach(Slide slide in slides)
            {
                String slideName = slide.UniqueName;
                slide.setupContext(mvcContext, slideName, resourceProvider);

                NavigationLink link = new NavigationLink(slideName, null, slideName + "/Show");
                navModel.addNavigationLink(link);

                ++i;
            }

            RunCommandsAction runCommands = (RunCommandsAction)mvcContext.Controllers["Common"].Actions["Start"];
            runCommands.addCommand(new NavigateToIndexCommand()
            {
                Index = startIndex
            });

            ClosingTaskbar taskbar = new ClosingTaskbar();
            SingleChildChainLink taskbarLink = new SingleChildChainLink("SlideTaskbar", taskbar);
            taskbar.Close += () => mvcContext.runAction("Common/Close");
            taskbar.addItem(new TaskTaskbarItem(new CallbackTask("Slideshow.Back", "Back", "SlideshowIcons/Back", "None", (arg) =>
            {
                mvcContext.runAction("NavigationBug/Previous");
            })));
            taskbar.addItem(new TaskTaskbarItem(new CallbackTask("Slideshow.Forward", "Forward", "SlideshowIcons/Forward", "None", (arg) =>
            {
                mvcContext.runAction("NavigationBug/Next");
            })));

            foreach (Task task in additionalTasks.Tasks)
            {
                taskbar.addItem(new TaskTaskbarItem(task));
            }
            
            mvcContext.Blurred += (ctx) =>
            {
                guiManager.deactivateLink("SlideTaskbar");
                guiManager.removeLinkFromChain(taskbarLink);
            };
            mvcContext.Focused += (ctx) =>
            {
                guiManager.addLinkToChain(taskbarLink);
                guiManager.pushRootContainer("SlideTaskbar");
            };
            mvcContext.RemovedFromStack += (ctx) =>
            {
                taskbar.Dispose();
            };

            return mvcContext;
        }

        public void cleanup(CleanupInfo cleanupInfo, ResourceProvider resourceProvider)
        {
            cleanupInfo.defineObjectClass(Slide.SlideActionClass);
            foreach (Slide slide in slides)
            {
                slide.cleanup(cleanupInfo, resourceProvider);
            }
        }

        public IEnumerable<Slide> Slides
        {
            get
            {
                return slides.AsReadOnly();
            }
        }

        public int Count
        {
            get
            {
                return slides.Count;
            }
        }

        public int Version
        {
            get
            {
                return version;
            }
        }

        public void updateToVersion(int newVersion)
        {
            if (newVersion <= CurrentVersion && newVersion > version)
            {
                foreach (Slide slide in slides)
                {
                    slide.updateToVersion(version, newVersion);
                }
                version = newVersion;
            }
        }

        protected Slideshow(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            info.RebuildList("Slides", slides);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.ExtractList("Slides", slides);
        }
    }
}
