using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using System.IO;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;

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
                return "Editor/EmptyViewIcon";
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
                String name = PathExtensions.RemoveExtension(fullPath);
                if (mvcContext.Views.hasItem(name))
                {
                    MessageBox.show(String.Format("A view named '{0}' already exists. Would you like to overwrite it?", name), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            createView(mvcContext, name);
                        }
                    });
                }
                else
                {
                    createView(mvcContext, name);
                }
                if (mvcContext.Controllers.hasItem(name))
                {
                    MessageBox.show(String.Format("A controller named '{0}' already exists. Would you like to overwrite it?", name), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            createController(mvcContext, name);
                        }
                    });
                }
                else
                {
                    createController(mvcContext, name);
                }
                editorController.saveAllCachedResources();
            }
        }

        private static void createController(AnomalousMvcContext mvcContext, String name)
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

        private static void createView(AnomalousMvcContext mvcContext, String name)
        {
            RmlView view = new RmlView(name);
            view.Buttons.add(new CloseButtonDefinition("Close", name + "/Close"));
            mvcContext.Views.add(view);
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

        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
                }
                return editInterface;
            }
        }
    }
}
