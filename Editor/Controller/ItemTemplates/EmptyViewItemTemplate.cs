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

        [Editable]
        public bool BackButton { get; set; }

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
                            mvcContext.Views.remove(mvcContext.Views[name]);
                            createView(mvcContext, name, BackButton);
                        }
                    });
                }
                else
                {
                    createView(mvcContext, name, BackButton);
                }
                if (mvcContext.Controllers.hasItem(name))
                {
                    MessageBox.show(String.Format("A controller named '{0}' already exists. Would you like to overwrite it?", name), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            mvcContext.Controllers.remove(mvcContext.Controllers[name]);
                            createController(mvcContext, name, BackButton);
                        }
                    });
                }
                else
                {
                    createController(mvcContext, name, BackButton);
                }
                editorController.saveAllCachedResources();
            }
        }

        public virtual void reset()
        {
            Name = null;
            BackButton = false;
        }

        private static void createController(AnomalousMvcContext mvcContext, String name, bool backButton)
        {
            MvcController controller = new MvcController(name);
            RunCommandsAction show = new RunCommandsAction("Show");
            show.addCommand(new ShowViewCommand(name));
            if (backButton)
            {
                show.addCommand(new RecordBackAction());
            }
            controller.Actions.add(show);
            RunCommandsAction close = new RunCommandsAction("Close");
            close.addCommand(new CloseViewCommand());
            controller.Actions.add(close);
            mvcContext.Controllers.add(controller);
        }

        private static void createView(AnomalousMvcContext mvcContext, String name, bool backButton)
        {
            RmlView view = new RmlView(name);
            if (backButton)
            {
                view.Buttons.add(new ButtonDefinition("Back", "Common/GoBack"));
            }
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
