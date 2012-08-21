using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Attributes;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.Controller.AnomalousMvc
{
    public partial class ControllerActionCollection : SaveableEditableItemCollection<ControllerAction>
    {
        public ControllerActionCollection()
        {

        }

        protected ControllerActionCollection(LoadInfo info)
            :base(info)
        {

        }
    }

    partial class ControllerActionCollection
    {
        class ActionItemTemplate : AddItemTemplateAdapter
        {
            private ControllerAction action;

            public ActionItemTemplate(String typeName, String imageName, String group, ControllerAction action)
                : base(typeName, imageName, group)
            {
                this.action = action;
            }

            public void addAction(ControllerActionCollection actions)
            {
                if (actions.hasItem(Name))
                {
                    MessageBox.show(String.Format("An action named '{0}' already exists. Would you like to overwrite it?", Name), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                        {
                            if (result == MessageBoxStyle.Yes)
                            {
                                actions.remove(actions[Name]);
                                doAddAction(actions);
                            }
                        });
                }
                else
                {
                    doAddAction(actions);
                }
            }

            private void doAddAction(ControllerActionCollection actions)
            {
                ControllerAction copy = CopySaver.Default.copy<ControllerAction>(action);
                copy.Name = Name;
                actions.add(copy);
            }
        }

        static LinkedList<ActionItemTemplate> actionTemplates;

        static void createActionTemplates()
        {
            if (actionTemplates == null)
            {
                actionTemplates = new LinkedList<ActionItemTemplate>();
                RunCommandsAction action;
                
                action = new RunCommandsAction("Empty");
                actionTemplates.AddLast(new ActionItemTemplate("Empty Action", "MvcContextEditor/RunCommandsIcon", "Empty", action));

                action = new RunCommandsAction("CameraLayersState");
                action.addCommand(new MoveCameraCommand());
                action.addCommand(new ChangeLayersCommand());
                action.addCommand(new ChangeMedicalStateCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Camera, Layers, Medical State Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));

                action = new RunCommandsAction("CameraLayers");
                action.addCommand(new MoveCameraCommand());
                action.addCommand(new ChangeLayersCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Camera, Layers Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));

                action = new RunCommandsAction("CameraMedicalState");
                action.addCommand(new MoveCameraCommand());
                action.addCommand(new ChangeMedicalStateCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Camera, Medical State Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));

                action = new RunCommandsAction("LayersMedicalState");
                action.addCommand(new ChangeLayersCommand());
                action.addCommand(new ChangeMedicalStateCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Layers, Medical State Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));

                action = new RunCommandsAction("Camera");
                action.addCommand(new MoveCameraCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Camera Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));

                action = new RunCommandsAction("Layers");
                action.addCommand(new ChangeLayersCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Layers Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));

                action = new RunCommandsAction("MedicalState");
                action.addCommand(new ChangeMedicalStateCommand());
                actionTemplates.AddLast(new ActionItemTemplate("Medical State Action", "MvcContextEditor/RunCommandsIcon", "Templates", action));
            }
        }

        public override void customizeEditInterface(Engine.Editing.EditInterface editInterface, Engine.Editing.EditInterfaceManager<ControllerAction> itemEdits)
        {
            createActionTemplates();
            editInterface.addCommand(new EditInterfaceCommand("Add Action", (callback, caller) =>
            {
                Medical.GUI.AddItemDialog.AddItem(actionTemplates, itemTemplate =>
                {
                    ((ActionItemTemplate)itemTemplate).addAction(this);
                });
            }));
        }
    }
}
