using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Attributes;
using System.IO;
using Medical.GUI.AnomalousMvc;
using Medical.SlideshowActions;

namespace Medical
{
    public abstract class Slide : Saveable
    {
        internal const String SlideActionClass = "SlideAction";

        [DoNotSave] //Saved manually
        private String id;

        private SlideLayoutStrategy layoutStrategy;

        [DoNotSave]
        private Dictionary<String, SlideAction> triggerActions = new Dictionary<string, SlideAction>();
        
        public Slide()
        {
            id = Guid.NewGuid().ToString("D");
            layoutStrategy = new BorderSlideLayoutStrategy();
        }

        public void setupContext(AnomalousMvcContext context, String name, bool allowPrevious, bool allowNext, ResourceProvider resourceProvider)
        {
            MvcController controller = new MvcController(name);
            RunCommandsAction showCommand = new RunCommandsAction("Show");
            showCommand.addCommand(new CloseAllViewsCommand());
            String timelinePath = Path.Combine(UniqueName, "Timeline.tl");
            if (resourceProvider.exists(timelinePath))
            {
                showCommand.addCommand(new PlayTimelineCommand(timelinePath));
            }
            foreach (var action in triggerActions.Values)
            {
                action.addToController(this, controller);
            }
            context.Controllers.add(controller);

            layoutStrategy.createViews(name, showCommand, context, this, allowPrevious, allowNext);            

            controller.Actions.add(showCommand);
            customizeController(controller, showCommand);
        }

        public void addPanel(SlidePanel panel)
        {
            layoutStrategy.addPanel(panel);
        }

        protected abstract void customizeController(MvcController controller, RunCommandsAction showCommand);

        /// <summary>
        /// Make a new unique name for the slide, should only need to be done when duplicating a slide for some reason.
        /// </summary>
        public void generateNewUniqueName()
        {
            id = Guid.NewGuid().ToString("D");
        }

        public virtual void cleanup(CleanupInfo info, ResourceProvider resourceProvider)
        {
            String timelinePath = Path.Combine(UniqueName, "Timeline.tl");
            info.claimFile(timelinePath);
            if (resourceProvider.exists(timelinePath))
            {
                using (Stream stream = resourceProvider.openFile(timelinePath))
                {
                    Timeline timeline = SharedXmlSaver.Load<Timeline>(stream);
                    timeline.cleanup(info);
                }
            }
            info.claimFile(Path.Combine(UniqueName, "Thumb.png"));
            layoutStrategy.claimFiles(info, resourceProvider, this);

            //Clean up actions
            List<String> removeActions = new List<String>(triggerActions.Keys);
            foreach (String action in info.getObjects<String>(Slide.SlideActionClass))
            {
                removeActions.Remove(action);
            }
            foreach (String action in removeActions)
            {
                triggerActions.Remove(action);
            }
            info.clearObjects(Slide.SlideActionClass);

            foreach (var action in triggerActions.Values)
            {
                action.cleanup(this, info, resourceProvider);
            }
        }

        public TemplateSlide createTemplateSlide()
        {
            TemplateSlide slide = new TemplateSlide();
            this.copyLayoutToSlide(slide, true);
            return slide;
        }

        public void copyLayoutToSlide(Slide slide, bool overwriteContent)
        {
            slide.layoutStrategy = layoutStrategy.createDerivedStrategy(slide.layoutStrategy, overwriteContent);
        }

        public void addAction(SlideAction action)
        {
            triggerActions.Add(action.Name, action);
        }

        public void removeAction(SlideAction action)
        {
            triggerActions.Remove(action.Name);
        }

        public void removeAction(String name)
        {
            triggerActions.Remove(name);
        }

        public void replaceAction(SlideAction action)
        {
            if (hasAction(action.Name))
            {
                removeAction(action);
            }
            addAction(action);
        }

        public bool hasAction(String name)
        {
            return triggerActions.ContainsKey(name);
        }

        public SlideAction getAction(String name)
        {
            SlideAction ret;
            triggerActions.TryGetValue(name, out ret);
            return ret;
        }

        public IEnumerable<SlidePanel> Panels
        {
            get
            {
                return layoutStrategy.Panels;
            }
        }

        public SlideLayoutStrategy LayoutStrategy
        {
            get
            {
                return layoutStrategy;
            }
        }

        public String UniqueName
        {
            get
            {
                return id;
            }
        }

        protected Slide(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            id = info.GetValueCb("Id", () =>
                {
                    return Guid.NewGuid().ToString("D");
                });
            info.RebuildDictionary("TriggerAction", triggerActions);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.AddValue("Id", id.ToString());
            info.ExtractDictionary("TriggerAction", triggerActions);
        }
    }
}
