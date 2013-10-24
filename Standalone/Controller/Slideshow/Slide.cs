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
        [DoNotSave] //Saved manually
        private String id;

        [DoNotSave]
        private List<SlidePanel> panels = new List<SlidePanel>();

        [DoNotSave]
        private Dictionary<String, SlideAction> triggerActions = new Dictionary<string, SlideAction>();
        
        public Slide()
        {
            id = Guid.NewGuid().ToString("D");
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
                action.addToController(controller);
            }
            context.Controllers.add(controller);

            bool addedButtons = false;
            foreach (RmlSlidePanel panel in panels)
            {
                MyGUIView view = panel.createView(this, name);
                if (!addedButtons)
                {
                    addedButtons = true;
                    if (allowPrevious)
                    {
                        view.Buttons.add(new ButtonDefinition("Previous", "NavigationBug/Previous"));
                    }
                    if (allowNext)
                    {
                        view.Buttons.add(new ButtonDefinition("Next", "NavigationBug/Next"));
                    }
                    view.Buttons.add(new CloseButtonDefinition("Close", "Common/Close"));
                }
                showCommand.addCommand(new ShowViewCommand(view.Name));
                context.Views.add(view);
            }

            controller.Actions.add(showCommand);
            customizeController(controller, showCommand);
        }

        public void addPanel(SlidePanel panel)
        {
            panels.Add(panel);
        }

        protected abstract void customizeController(MvcController controller, RunCommandsAction showCommand);

        /// <summary>
        /// Make a new unique name for the slide, should only need to be done when duplicating a slide for some reason.
        /// </summary>
        public void generateNewUniqueName()
        {
            id = Guid.NewGuid().ToString("D");
        }

        public virtual void cleanup(CleanupFileInfo info, ResourceProvider resourceProvider)
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
            foreach (RmlSlidePanel panel in panels)
            {
                panel.claimFiles(info, resourceProvider, this);
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
            List<SlidePanel> removePanels = new List<SlidePanel>(slide.panels);
            foreach (SlidePanel panel in panels)
            {
                SlidePanel existingPanel = slide.findPanel(panel.ViewLocation);
                if (existingPanel != null)
                {
                    removePanels.Remove(existingPanel);
                    panel.applyToExisting(existingPanel, overwriteContent);
                }
                else
                {
                    slide.panels.Add(panel.clone());
                }
            }

            foreach (SlidePanel remove in removePanels)
            {
                slide.panels.Remove(remove);
            }
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
                return panels;
            }
        }

        public String UniqueName
        {
            get
            {
                return id;
            }
        }

        private SlidePanel findPanel(ViewLocations viewLocation)
        {
            foreach (SlidePanel panel in panels)
            {
                if (panel.ViewLocation == viewLocation)
                {
                    return panel;
                }
            }
            return null;
        }

        protected Slide(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            id = info.GetValueCb("Id", () =>
                {
                    return Guid.NewGuid().ToString("D");
                });
            info.RebuildList("Panel", panels);
            info.RebuildDictionary("TriggerAction", triggerActions);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.AddValue("Id", id.ToString());
            info.ExtractList("Panel", panels);
            info.ExtractDictionary("TriggerAction", triggerActions);
        }
    }
}
