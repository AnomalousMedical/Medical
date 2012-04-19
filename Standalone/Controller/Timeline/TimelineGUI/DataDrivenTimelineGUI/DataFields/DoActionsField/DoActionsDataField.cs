using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    public partial class DoActionsDataField : DataField
    {
        [DoNotSave]
        private List<DoActionsDataFieldCommand> commands = new List<DoActionsDataFieldCommand>();

        public DoActionsDataField(String name)
            :base(name)
        {

        }

        public void addCommand(DoActionsDataFieldCommand command)
        {
            commands.Add(command);
            commandAdded(command);
        }

        public void removeCommand(DoActionsDataFieldCommand command)
        {
            commands.Remove(command);
            commandRemoved(command);
        }

        public void doActions(DataDrivenTimelineGUI gui)
        {
            foreach (DoActionsDataFieldCommand command in commands)
            {
                command.doAction(gui);
            }
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        public override string Type
        {
            get
            {
                return "Do Actions";
            }
        }

        protected DoActionsDataField(LoadInfo info)
            :base(info)
        {
            info.RebuildList<DoActionsDataFieldCommand>("Command", commands);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<DoActionsDataFieldCommand>("Command", commands);
        }
    }

    partial class DoActionsDataField
    {
        [DoNotSave]
        private EditInterfaceManager<DoActionsDataFieldCommand> editInterfaceManager = null;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Add Move Camera", addMoveCamera));
            editInterface.addCommand(new EditInterfaceCommand("Add Change Layers", addChangeLayers));
            editInterface.addCommand(new EditInterfaceCommand("Add Change Medical State", addChangeMedicalState));
            editInterface.addCommand(new EditInterfaceCommand("Add Play Example Timeline", addPlayExampleTimeline));
            editInterface.addCommand(new EditInterfaceCommand("Add Stop Timeline Playback", addStopTimelinePlayback));

            editInterfaceManager = new EditInterfaceManager<DoActionsDataFieldCommand>(editInterface);
            editInterfaceManager.addCommand(new EditInterfaceCommand("Remove", removeCommand));

            foreach (DoActionsDataFieldCommand command in commands)
            {
                commandAdded(command);
            }
        }

        private void addMoveCamera(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new MoveCameraDoAction());
        }

        private void addChangeLayers(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new ChangeLayersDoAction());
        }

        private void addChangeMedicalState(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new ChangeMedicalStateDoAction());
        }

        private void addPlayExampleTimeline(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new PlayExampleTimelineDoAction());
        }

        private void addStopTimelinePlayback(EditUICallback callback, EditInterfaceCommand command)
        {
            addCommand(new StopTimelinePlaybackDoAction());
        }

        private void removeCommand(EditUICallback callback, EditInterfaceCommand caller)
        {
            removeCommand(editInterfaceManager.resolveSourceObject(callback.getSelectedEditInterface()));
        }

        void commandAdded(DoActionsDataFieldCommand command)
        {
            if (editInterfaceManager != null)
            {
                editInterfaceManager.addSubInterface(command, command.EditInterface);
            }
        }

        void commandRemoved(DoActionsDataFieldCommand command)
        {
            if (editInterfaceManager != null)
            {
                editInterfaceManager.removeSubInterface(command);
            }
        }
    }
}
