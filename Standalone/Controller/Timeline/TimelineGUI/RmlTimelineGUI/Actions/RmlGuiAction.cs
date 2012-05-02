using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.RmlTimeline.Actions
{
    public partial class RmlGuiAction : Saveable
    {
        private String name;

        [DoNotSave]
        private List<RmlGuiActionCommand> commands = new List<RmlGuiActionCommand>();

        public RmlGuiAction(String name)
        {
            this.name = name;
        }

        public void addCommand(RmlGuiActionCommand command)
        {
            commands.Add(command);
            commandAdded(command);
        }

        public void removeCommand(RmlGuiActionCommand command)
        {
            commands.Remove(command);
            commandRemoved(command);
        }

        public void runCommands(RmlTimelineGUI gui)
        {
            foreach (RmlGuiActionCommand command in commands)
            {
                command.execute(gui);
            }
        }

        protected RmlGuiAction(LoadInfo info)
        {
            name = info.GetString("Name");
            info.RebuildList<RmlGuiActionCommand>("Command", commands);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", name);
            info.ExtractList<RmlGuiActionCommand>("Command", commands);
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }

    partial class RmlGuiAction
    {
        private EditInterface editInterface;

        [DoNotSave]
        private EditInterfaceManager<RmlGuiActionCommand> editInterfaceManager = null;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface(String.Format("{0} - Action", name), null);

                editInterface.addCommand(new EditInterfaceCommand("Add Move Camera", addMoveCamera));
                editInterface.addCommand(new EditInterfaceCommand("Add Change Layers", addChangeLayers));
                editInterface.addCommand(new EditInterfaceCommand("Add Change Medical State", addChangeMedicalState));
                editInterface.addCommand(new EditInterfaceCommand("Add Play Timeline", addPlayTimeline));
                editInterface.addCommand(new EditInterfaceCommand("Add Stop Timeline", addStopTimeline));
                editInterface.addCommand(new EditInterfaceCommand("Add Close GUI", addCloseGUI));

                editInterfaceManager = new EditInterfaceManager<RmlGuiActionCommand>(editInterface);
                editInterfaceManager.addCommand(new EditInterfaceCommand("Remove", removeCommand));

                foreach (RmlGuiActionCommand command in commands)
                {
                    commandAdded(command);
                }
            }

            return editInterface;
        }

        private void addMoveCamera(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new MoveCameraCommand());
        }

        private void addChangeLayers(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new ChangeLayersCommand());
        }

        private void addChangeMedicalState(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new ChangeMedicalStateCommand());
        }

        private void addPlayTimeline(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new PlayTimelineCommand());
        }

        private void addStopTimeline(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new StopTimelineCommand());
        }

        private void addCloseGUI(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new CloseGuiCommand());
        }

        private void removeCommand(EditUICallback callback, EditInterfaceCommand caller)
        {
            removeCommand(editInterfaceManager.resolveSourceObject(callback.getSelectedEditInterface()));
        }

        void commandAdded(RmlGuiActionCommand command)
        {
            if (editInterfaceManager != null)
            {
                editInterfaceManager.addSubInterface(command, command.EditInterface);
            }
        }

        void commandRemoved(RmlGuiActionCommand command)
        {
            if (editInterfaceManager != null)
            {
                editInterfaceManager.removeSubInterface(command);
            }
        }
    }
}
