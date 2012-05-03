﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public partial class RunCommandsAction : ControllerAction
    {
        [DoNotSave]
        private List<ActionCommand> commands = new List<ActionCommand>();

        public RunCommandsAction(String name)
            :base(name)
        {

        }

        public void addCommand(ActionCommand command)
        {
            commands.Add(command);
            commandAdded(command);
        }

        public void removeCommand(ActionCommand command)
        {
            commands.Remove(command);
            commandRemoved(command);
        }

        public override void execute(AnomalousMvcContext context)
        {
            foreach (ActionCommand command in commands)
            {
                command.execute(context);
            }
        }

        protected RunCommandsAction(LoadInfo info)
            :base(info)
        {
            info.RebuildList<ActionCommand>("Command", commands);
        }

        public override void getInfo(SaveInfo info)
        {
            info.ExtractList<ActionCommand>("Command", commands);
            base.getInfo(info);
        }
    }

    partial class RunCommandsAction
    {
        [DoNotSave]
        private EditInterfaceManager<ActionCommand> editInterfaceManager = null;

        protected override EditInterface createEditInterface()
        {
            EditInterface editInterface = new EditInterface(String.Format("{0} - Action", Name), null);

            editInterface.addCommand(new EditInterfaceCommand("Add Move Camera", addMoveCamera));
            editInterface.addCommand(new EditInterfaceCommand("Add Change Layers", addChangeLayers));
            editInterface.addCommand(new EditInterfaceCommand("Add Change Medical State", addChangeMedicalState));
            editInterface.addCommand(new EditInterfaceCommand("Add Play Timeline", addPlayTimeline));
            editInterface.addCommand(new EditInterfaceCommand("Add Stop Timeline", addStopTimeline));
            editInterface.addCommand(new EditInterfaceCommand("Add Close View", addCloseGUI));
            editInterface.addCommand(new EditInterfaceCommand("Add Show View", addShowGUI));

            editInterfaceManager = new EditInterfaceManager<ActionCommand>(editInterface);
            editInterfaceManager.addCommand(new EditInterfaceCommand("Remove", removeCommand));

            foreach (ActionCommand command in commands)
            {
                commandAdded(command);
            }

            return editInterface;
        }

        private void addMoveCamera(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new MoveCameraCommand());
        }

        private void addChangeLayers(EditUICallback callback, EditInterfaceCommand command)
        {
            ChangeLayersCommand changeLayers = new ChangeLayersCommand();
            changeLayers.Layers.captureState();
            addCommand(changeLayers);
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
            addCommand(new CloseViewCommand());
        }

        private void addShowGUI(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new ShowViewCommand());
        }

        private void removeCommand(EditUICallback callback, EditInterfaceCommand caller)
        {
            removeCommand(editInterfaceManager.resolveSourceObject(callback.getSelectedEditInterface()));
        }

        void commandAdded(ActionCommand command)
        {
            if (editInterfaceManager != null)
            {
                editInterfaceManager.addSubInterface(command, command.EditInterface);
            }
        }

        void commandRemoved(ActionCommand command)
        {
            if (editInterfaceManager != null)
            {
                editInterfaceManager.removeSubInterface(command);
            }
        }
    }
}
