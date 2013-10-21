using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Medical
{
    public abstract class RmlSlide : Slide
    {
        [DoNotSave] //Saved manually
        private String id;

        [DoNotSave]
        private List<RmlSlidePanel> panels = new List<RmlSlidePanel>();
        
        public RmlSlide()
        {
            id = Guid.NewGuid().ToString("D");
        }

        public void setupContext(AnomalousMvcContext context, String name, bool allowPrevious, bool allowNext, ResourceProvider resourceProvider)
        {
            MvcController controller = new MvcController(name);
            RunCommandsAction showCommand = new RunCommandsAction("Show");
            String timelinePath = Path.Combine(UniqueName, "Timeline.tl");
            if (resourceProvider.exists(timelinePath))
            {
                showCommand.addCommand(new PlayTimelineCommand(timelinePath));
            }
            context.Controllers.add(controller);

            bool addedButtons = false;
            foreach (RmlSlidePanel panel in panels)
            {
                RawRmlView view = new RawRmlView(panel.createViewName(name))
                {
                    Rml = panel.Rml,
                    FakePath = UniqueName + "/index.rml",
                    WidthSizeStrategy = panel.SizeStrategy,
                    Size = new IntSize2(panel.Size, panel.Size),
                    ViewLocation = panel.ViewLocation
                };
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

        public void addPanel(String rml, ViewLocations location, ViewSizeStrategy sizeStrategy, int size)
        {
            RmlSlidePanel panel = new RmlSlidePanel()
            {
                Rml = rml,
                ViewLocation = location,
                SizeStrategy = sizeStrategy,
                Size = size,
            };
            panels.Add(panel);
        }

        protected abstract void customizeController(MvcController controller, RunCommandsAction showCommand);

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

        public IEnumerable<RmlSlidePanel> Panels
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

        protected RmlSlide(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            id = info.GetValueCb("Id", () =>
                {
                    return Guid.NewGuid().ToString("D");
                });
            info.RebuildList("Panel", panels);
            if (info.hasValue("rml"))
            {
                RmlSlidePanel panel = new RmlSlidePanel();
                panel.Rml = info.GetString("rml");
                panel.ViewLocation = ViewLocations.Left;
                panel.SizeStrategy = ViewSizeStrategy.Auto;
                panels.Add(panel);
            }
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.AddValue("Id", id.ToString());
            info.ExtractList("Panel", panels);
        }
    }
}
