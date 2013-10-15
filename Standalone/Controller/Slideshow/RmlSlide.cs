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
    public class RmlSlide : Slide
    {
        private String rml;
        private int size = 50;
        private ViewSizeStrategy sizeStrategy = ViewSizeStrategy.Auto;

        [DoNotSave] //Saved manually
        private String id;
        
        public RmlSlide()
        {
            id = Guid.NewGuid().ToString("D");
        }

        public View createView(String name, bool allowPrevious, bool allowNext)
        {
            RawRmlView view = new RawRmlView(name)
            {
                Rml = this.Rml,
                FakePath = UniqueName + "/index.rml",
                WidthSizeStrategy = this.SizeStrategy,
                Size = new IntSize2(Size, Size),
            };
            if (allowPrevious)
            {
                view.Buttons.add(new ButtonDefinition("Previous", "NavigationBug/Previous"));
            }
            if (allowNext)
            {
                view.Buttons.add(new ButtonDefinition("Next", "NavigationBug/Next"));
            }
            view.Buttons.add(new CloseButtonDefinition("Close", "Common/Close"));
            return view;
        }

        public MvcController createController(String name, String viewName, ResourceProvider resourceProvider)
        {
            MvcController controller = new MvcController(name);
            RunCommandsAction showCommand = new RunCommandsAction("Show");
            showCommand.addCommand(new ShowViewCommand(viewName));
            String timelinePath = Path.Combine(UniqueName, "Timeline.tl");
            if (resourceProvider.exists(timelinePath))
            {
                showCommand.addCommand(new PlayTimelineCommand(timelinePath));
            }
            controller.Actions.add(showCommand);
            customizeController(controller, showCommand);

            return controller;
        }

        protected virtual void customizeController(MvcController controller, RunCommandsAction showCommand)
        {

        }

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
            XDocument rmlDoc = XDocument.Parse(rml);
            var images = from query in rmlDoc.Descendants("img") 
                         where query.Attribute("src") != null 
                         select query.Attribute("src").Value;

            foreach (String image in images)
            {
                info.claimFile(Path.Combine(UniqueName, image));
            }
        }

        public void updateToVersion(int version)
        {
            if (version == 2)
            {
                rml = rml.Replace("href=\"/MasterTemplate.trml\"", "href=\"../MasterTemplate.trml\"");
            }
        }

        public String UniqueName
        {
            get
            {
                return id;
            }
        }

        [Editable]
        public String Rml
        {
            get
            {
                return rml;
            }
            set
            {
                rml = value;
            }
        }

        [Editable]
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        [Editable]
        public ViewSizeStrategy SizeStrategy
        {
            get
            {
                return sizeStrategy;
            }
            set
            {
                sizeStrategy = value;
            }
        }

        protected RmlSlide(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            id = info.GetValueCb("Id", () =>
                {
                    return Guid.NewGuid().ToString("D");
                });
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.AddValue("Id", id.ToString());
        }
    }
}
