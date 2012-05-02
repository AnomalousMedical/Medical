using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.RmlTimeline.Actions
{
    public partial class RmlGuiActionManager : Saveable
    {
        private List<RmlGuiAction> actions = new List<RmlGuiAction>();

        public RmlGuiActionManager()
        {

        }

        public void addAction(RmlGuiAction action)
        {
            actions.Add(action);
            if (editInterface != null)
            {
                addActionDefinition(action);
            }
        }

        public void removeAction(RmlGuiAction action)
        {
            actions.Remove(action);
            if(editInterface != null)
            {
                removeActionDefinition(action);
            }
        }

        public void insertAction(int index, RmlGuiAction action)
        {
            actions.Insert(index, action);
            if (editInterface != null)
            {
                refreshActionDefinitions();
            }
        }

        public void runAction(String name, RmlTimelineGUI gui)
        {
            foreach (RmlGuiAction action in actions)
            {
                if (action.Name == name)
                {
                    action.runCommands(gui);
                    break;
                }
            }
        }

        protected RmlGuiActionManager(LoadInfo info)
        {
            info.RebuildList<RmlGuiAction>("Actions", actions);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<RmlGuiAction>("Actions", actions);
        }
    }

    //EditInterface functions
    partial class RmlGuiActionManager
    {
        private EditInterface editInterface;
        private EditInterfaceManager<RmlGuiAction> actionEdits;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Actions");
                EditablePropertyInfo propertyInfo = new EditablePropertyInfo();
                propertyInfo.addColumn(new EditablePropertyColumn("Name", true));
                propertyInfo.addColumn(new EditablePropertyColumn("Value", false));
                editInterface.setPropertyInfo(propertyInfo);
                editInterface.addCommand(new EditInterfaceCommand("Add Action", createNewAction));

                actionEdits = new EditInterfaceManager<RmlGuiAction>(editInterface);
                actionEdits.addCommand(new EditInterfaceCommand("Remove", deleteAction));
                actionEdits.addCommand(new EditInterfaceCommand("Rename", renameAction));
                actionEdits.addCommand(new EditInterfaceCommand("Move Up", moveUp));
                actionEdits.addCommand(new EditInterfaceCommand("Move Down", moveDown));

                foreach (RmlGuiAction action in actions)
                {
                    addActionDefinition(action);
                }
            }
            return editInterface;
        }

        private void createNewAction(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasAction(input))
                {
                    RmlGuiAction action = new RmlGuiAction(input);
                    addAction(action);
                    return true;
                }
                errorPrompt = String.Format("An action named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void deleteAction(EditUICallback callback, EditInterfaceCommand command)
        {
            RmlGuiAction action = actionEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeAction(action);
        }

        private void renameAction(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.getInputString("Enter a new name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasAction(input))
                {
                    //This works, but need to update the EditInterface
                    EditInterface editInterface = callback.getSelectedEditInterface();
                    RmlGuiAction action = actionEdits.resolveSourceObject(editInterface);
                    action.Name = input;
                    editInterface.setName(String.Format("{0} - Action", input));
                    return true;
                }
                errorPrompt = String.Format("An action named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void moveUp(EditUICallback callback, EditInterfaceCommand command)
        {
            RmlGuiAction action = actionEdits.resolveSourceObject(callback.getSelectedEditInterface());
            int index = actions.IndexOf(action) - 1;
            if (index >= 0)
            {
                removeAction(action);
                insertAction(index, action);
            }
        }

        private void moveDown(EditUICallback callback, EditInterfaceCommand command)
        {
            RmlGuiAction field = actionEdits.resolveSourceObject(callback.getSelectedEditInterface());
            int index = actions.IndexOf(field) + 1;
            if (index < actions.Count)
            {
                removeAction(field);
                insertAction(index, field);
            }
        }

        private void addActionDefinition(RmlGuiAction action)
        {
            actionEdits.addSubInterface(action, action.getEditInterface());
        }

        private void removeActionDefinition(RmlGuiAction action)
        {
            actionEdits.removeSubInterface(action);
        }

        private void refreshActionDefinitions()
        {
            actionEdits.clearSubInterfaces();
            foreach (RmlGuiAction action in actions)
            {
                action.getEditInterface().clearCommands();
                actionEdits.addSubInterface(action, action.getEditInterface());
            }
        }

        private bool hasAction(String name)
        {
            foreach (RmlGuiAction action in actions)
            {
                if (action.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
