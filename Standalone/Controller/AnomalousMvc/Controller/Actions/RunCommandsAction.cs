using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using System.Reflection;

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

        public enum CustomQueries
        {
            ShowCommandBrowser,
        }

        protected override EditInterface createEditInterface()
        {
            EditInterface editInterface = new EditInterface(Name, null);

            editInterface.addCommand(new EditInterfaceCommand("Add Command", delegate(EditUICallback callback, EditInterfaceCommand command)
            {
                callback.runCustomQuery(CustomQueries.ShowCommandBrowser, delegate(Object result, ref string errorMessage)
                {
                    Type resultType = result as Type;
                    if (resultType != null)
                    {
                        addCommand((ActionCommand)Activator.CreateInstance(resultType));
                    }
                    return true;
                });
            }));

            editInterfaceManager = new EditInterfaceManager<ActionCommand>(editInterface);
            editInterfaceManager.addCommand(new EditInterfaceCommand("Remove", removeCommand));

            foreach (ActionCommand command in commands)
            {
                commandAdded(command);
            }

            return editInterface;
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

        public static Browser CreateCommandBrowser()
        {
            Browser browser = new Browser("Commands");
            BrowserNode rootNode = browser.getTopNode();
            //Camera
            BrowserNode cameraCommands = new BrowserNode("Camera", null);
            cameraCommands.addChild(new BrowserNode("Move Camera", typeof(MoveCameraCommand)));
            cameraCommands.addChild(new BrowserNode("Save Camera Position", typeof(SaveCameraPositionCommand)));
            cameraCommands.addChild(new BrowserNode("Restore Camera Position", typeof(RestoreCameraPositionCommand)));
            rootNode.addChild(cameraCommands);

            //Layers
            BrowserNode layerCommands = new BrowserNode("Layers", null);
            layerCommands.addChild(new BrowserNode("Change Layers", typeof(ChangeLayersCommand)));
            layerCommands.addChild(new BrowserNode("Save Layers", typeof(SaveLayersCommand)));
            layerCommands.addChild(new BrowserNode("Restore Layers", typeof(RestoreLayersCommand)));
            rootNode.addChild(layerCommands);

            //Medical State
            BrowserNode medicalStateCommands = new BrowserNode("Medical State", null);
            medicalStateCommands.addChild(new BrowserNode("Change Medical State", typeof(ChangeMedicalStateCommand)));
            medicalStateCommands.addChild(new BrowserNode("Save Medical State", typeof(SaveMedicalStateCommand)));
            medicalStateCommands.addChild(new BrowserNode("Restore Medical State", typeof(RestoreMedicalStateCommand)));
            medicalStateCommands.addChild(new BrowserNode("Create Medical State", typeof(CreateMedicalStateCommand)));
            rootNode.addChild(medicalStateCommands);

            //Timeline
            BrowserNode timelineCommands = new BrowserNode("Timeline", null);
            timelineCommands.addChild(new BrowserNode("Play Timeline", typeof(PlayTimelineCommand)));
            timelineCommands.addChild(new BrowserNode("Stop Timeline", typeof(StopTimelineCommand)));
            rootNode.addChild(timelineCommands);

            //View
            BrowserNode viewCommands = new BrowserNode("Views", null);
            viewCommands.addChild(new BrowserNode("Show View", typeof(ShowViewCommand)));
            viewCommands.addChild(new BrowserNode("Close View", typeof(CloseViewCommand)));
            viewCommands.addChild(new BrowserNode("Close All Views", typeof(CloseAllViewsCommand)));
            rootNode.addChild(viewCommands);

            //Main Interface
            BrowserNode mainInterfaceCommands = new BrowserNode("Main Interface", null);
            mainInterfaceCommands.addChild(new BrowserNode("Hide Main Interface", typeof(HideMainInterfaceCommand)));
            mainInterfaceCommands.addChild(new BrowserNode("Show Main Interface", typeof(ShowMainInterfaceCommand)));
            rootNode.addChild(mainInterfaceCommands);

            //Navigation
            BrowserNode navigationCommands = new BrowserNode("Navigation", null);
            navigationCommands.addChild(new BrowserNode("Navigate Previous", typeof(NavigatePreviousCommand)));
            navigationCommands.addChild(new BrowserNode("Navigate Next", typeof(NavigateNextCommand)));
            rootNode.addChild(navigationCommands);

            //Mvc Actions
            BrowserNode mvcActions = new BrowserNode("Mvc", null);
            mvcActions.addChild(new BrowserNode("Run Action", typeof(RunActionCommand)));
            mvcActions.addChild(new BrowserNode("Shutdown", typeof(ShutdownCommand)));
            rootNode.addChild(mvcActions);

            //Muscle Position
            BrowserNode musclePosition = new BrowserNode("Muscle Position", null);
            musclePosition.addChild(new BrowserNode("Change Muscle Position", typeof(SetMusclePositionCommand)));
            musclePosition.addChild(new BrowserNode("Save Muscle Position", typeof(SaveMusclePositionCommand)));
            musclePosition.addChild(new BrowserNode("Restore Muscle Position", typeof(RestoreMusclePositionCommand)));
            rootNode.addChild(musclePosition);

            return browser;
        }
    }
}