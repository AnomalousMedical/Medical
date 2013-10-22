using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Attributes;
using System.IO;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    public abstract class Slide : Saveable
    {
        [DoNotSave] //Saved manually
        private String id;

        [DoNotSave]
        private List<SlidePanel> panels = new List<SlidePanel>();
        
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
            //Need to save timeline files somehow
            foreach (RmlSlidePanel panel in panels)
            {
                panel.claimFiles(info, resourceProvider, this);
            }
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

        protected Slide(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            id = info.GetValueCb("Id", () =>
                {
                    return Guid.NewGuid().ToString("D");
                });
            info.RebuildList("Panel", panels);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.AddValue("Id", id.ToString());
            info.ExtractList("Panel", panels);
        }
    }
}
