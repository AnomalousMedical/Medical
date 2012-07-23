using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using System.IO;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;

namespace Medical
{
    class EmptyViewItemTemplate : ProjectItemTemplate
    {
        private RmlTypeController rmlTypeController;
        private MvcTypeController mvcTypeController;

        public EmptyViewItemTemplate(RmlTypeController rmlTypeController, MvcTypeController mvcTypeController)
        {
            this.rmlTypeController = rmlTypeController;
            this.mvcTypeController = mvcTypeController;
        }

        public string TypeName
        {
            get
            {
                return "Empty View";
            }
        }

        public string ImageName
        {
            get
            {
                return "";
            }
        }

        public string Group
        {
            get
            {
                return "Template";
            }
        }

        [Editable]
        public String Name { get; set; }

        public void createItem(string path, EditorController editorController)
        {
            String fullPath = Path.Combine(path, Name);
            rmlTypeController.createRmlFileSafely(fullPath);
            AnomalousMvcContext mvcContext = mvcTypeController.CurrentObject;
            if (mvcContext != null)
            {
                String extension = Path.GetExtension(fullPath);
                String name = fullPath;
                if (!String.IsNullOrEmpty(extension))
                {
                    name = name.Replace(extension, "");
                }
                if (!mvcContext.Views.hasItem(name))
                {
                    RmlView view = new RmlView(name);
                    view.Buttons.add(new CloseButtonDefinition("Close", name + "/Close"));
                    mvcContext.Views.add(view);
                }
                if (!mvcContext.Controllers.hasItem(name))
                {
                    MvcController controller = new MvcController(name);
                    RunCommandsAction show = new RunCommandsAction("Show");
                    show.addCommand(new ShowViewCommand(name));
                    controller.Actions.add(show);
                    RunCommandsAction close = new RunCommandsAction("Close");
                    close.addCommand(new CloseViewCommand());
                    controller.Actions.add(close);
                    mvcContext.Controllers.add(controller);
                }
            }
        }

        public bool isValid(out string errorMessage)
        {
            if (String.IsNullOrEmpty(Name))
            {
                errorMessage = "Please enter a name.";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                return ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
            }
        }
    }
}
