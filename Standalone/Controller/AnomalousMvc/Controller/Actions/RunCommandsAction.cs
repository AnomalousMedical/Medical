﻿using System;
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

        public RunCommandsAction(String name, params ActionCommand[] commands)
            : base(name)
        {
            this.commands.AddRange(commands);
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

        public void clear()
        {
            commands.Clear();
            commandsCleared();
        }

        public IEnumerable<ActionCommand> Commands
        {
            get
            {
                return commands;
            }
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
        private EditInterface editInterface;

        public enum CustomQueries
        {
            ShowCommandBrowser,
        }

        protected sealed override EditInterface createEditInterface()
        {
            editInterface = new EditInterface(Name, null);
            editInterface.IconReferenceTag = "MvcContextEditor/RunCommandsIcon";

            editInterface.addCommand(new EditInterfaceCommand("Add Command", callback =>
            {
                callback.runCustomQuery(CustomQueries.ShowCommandBrowser, delegate(Type resultType, ref string errorMessage)
                {
                    addCommand((ActionCommand)Activator.CreateInstance(resultType));
                    return true;
                });
            }));

            var editInterfaceManager = editInterface.createEditInterfaceManager<ActionCommand>();
            editInterfaceManager.addCommand(new EditInterfaceCommand("Remove", removeCommand));

            foreach (ActionCommand command in commands)
            {
                commandAdded(command);
            }

            return editInterface;
        }

        private void removeCommand(EditUICallback callback)
        {
            removeCommand(editInterface.resolveSourceObject<ActionCommand>(callback.getSelectedEditInterface()));
        }

        void commandAdded(ActionCommand command)
        {
            if (editInterface != null)
            {
                editInterface.addSubInterface(command, command.EditInterface);
            }
        }

        void commandRemoved(ActionCommand command)
        {
            if (editInterface != null)
            {
                editInterface.removeSubInterface(command);
            }
        }

        void commandsCleared()
        {
            if (editInterface != null)
            {
                editInterface.getEditInterfaceManager<ActionCommand>().clearSubInterfaces();
            }
        }

        public static Browser CreateCommandBrowser()
        {
            Browser browser = new Browser("Commands", "Choose Command");
            BrowserNode rootNode = browser.getTopNode();
            //Camera
            BrowserNode cameraCommands = new BrowserNode("Camera", null, iconName: "MvcContextEditor/CameraIcon");
            cameraCommands.addChild(new BrowserNode("Move Camera", typeof(MoveCameraCommand), iconName: "MvcContextEditor/CameraMoveIcon"));
            cameraCommands.addChild(new BrowserNode("Save Camera Position", typeof(SaveCameraPositionCommand), iconName: "MvcContextEditor/CameraSavePositionIcon"));
            cameraCommands.addChild(new BrowserNode("Restore Camera Position", typeof(RestoreCameraPositionCommand), iconName: "MvcContextEditor/CameraRestorePositionIcon"));
            rootNode.addChild(cameraCommands);

            //Layers
            BrowserNode layerCommands = new BrowserNode("Layers", null, iconName: "MvcContextEditor/LayersIcon");
            layerCommands.addChild(new BrowserNode("Change Layers", typeof(ChangeLayersCommand), iconName: "MvcContextEditor/LayersChangeIcon"));
            layerCommands.addChild(new BrowserNode("Save Layers", typeof(SaveLayersCommand), iconName: "MvcContextEditor/LayersSaveIcon"));
            layerCommands.addChild(new BrowserNode("Restore Layers", typeof(RestoreLayersCommand), iconName: "MvcContextEditor/LayersRestoreIcon"));
            rootNode.addChild(layerCommands);

            //Medical State
            BrowserNode medicalStateCommands = new BrowserNode("Medical State", null, iconName: "MvcContextEditor/MedicalStateIcon");
            medicalStateCommands.addChild(new BrowserNode("Change Medical State", typeof(ChangeMedicalStateCommand), iconName: "MvcContextEditor/MedicalStateChangeIcon"));
            medicalStateCommands.addChild(new BrowserNode("Save Medical State", typeof(SaveMedicalStateCommand), iconName: "MvcContextEditor/MedicalStateSaveIcon"));
            medicalStateCommands.addChild(new BrowserNode("Restore Medical State", typeof(RestoreMedicalStateCommand), iconName: "MvcContextEditor/MedicalStateRestoreIcon"));
            medicalStateCommands.addChild(new BrowserNode("Create Medical State", typeof(CreateMedicalStateCommand), iconName: "MvcContextEditor/MedicalStateCreateIcon"));
            rootNode.addChild(medicalStateCommands);

            //Timeline
            BrowserNode timelineCommands = new BrowserNode("Timeline", null, iconName: "MvcContextEditor/TimelineIcon");
            timelineCommands.addChild(new BrowserNode("Play Timeline", typeof(PlayTimelineCommand), iconName: "MvcContextEditor/TimelinePlayIcon"));
            timelineCommands.addChild(new BrowserNode("Stop Timeline", typeof(StopTimelineCommand), iconName: "MvcContextEditor/TimelineStopIcon"));
            rootNode.addChild(timelineCommands);

            //View
            BrowserNode viewCommands = new BrowserNode("Views", null, iconName: "MvcContextEditor/ViewsIcon");
            viewCommands.addChild(new BrowserNode("Show View", typeof(ShowViewCommand), iconName: "MvcContextEditor/ViewShowIcon"));
            viewCommands.addChild(new BrowserNode("Show View If Not Open", typeof(ShowViewIfNotOpenCommand), iconName: "MvcContextEditor/ViewShowIfNotOpen"));
            viewCommands.addChild(new BrowserNode("Close View", typeof(CloseViewCommand), iconName: "MvcContextEditor/ViewCloseIcon"));
            viewCommands.addChild(new BrowserNode("Close View If Open", typeof(CloseViewIfOpen), iconName: "MvcContextEditor/ViewCloseIcon"));
            viewCommands.addChild(new BrowserNode("Close All Views", typeof(CloseAllViewsCommand), iconName: "MvcContextEditor/ViewCloseAllIcon"));
            rootNode.addChild(viewCommands);

            //Main Interface
            BrowserNode mainInterfaceCommands = new BrowserNode("Main Interface", null, iconName: "MvcContextEditor/MainInterfaceIcon");
            mainInterfaceCommands.addChild(new BrowserNode("Hide Main Interface", typeof(HideMainInterfaceCommand), iconName: "MvcContextEditor/MainInterfaceHideIcon"));
            mainInterfaceCommands.addChild(new BrowserNode("Show Main Interface", typeof(ShowMainInterfaceCommand), iconName: "MvcContextEditor/MainInterfaceShowIcon"));
            rootNode.addChild(mainInterfaceCommands);

            //Navigation
            BrowserNode navigationCommands = new BrowserNode("Navigation", null, iconName: "MvcContextEditor/NavigateIcon");
            navigationCommands.addChild(new BrowserNode("Navigate Previous", typeof(NavigatePreviousCommand), iconName: "MvcContextEditor/NavigatePreviousIcon"));
            navigationCommands.addChild(new BrowserNode("Navigate Next", typeof(NavigateNextCommand), iconName: "MvcContextEditor/NavigateNextIcon"));
            navigationCommands.addChild(new BrowserNode("Navigate Reload", typeof(NavigateReloadCurrentCommand), iconName: CommonResources.NoIcon));
            navigationCommands.addChild(new BrowserNode("Navigate To", typeof(NavigateToCommand), iconName: "MvcContextEditor/NavigateToIcon"));
            navigationCommands.addChild(new BrowserNode("Navigate To Indexed", typeof(NavigateToIndexCommand), iconName: "MvcContextEditor/NavigateToIcon"));
            navigationCommands.addChild(new BrowserNode("Navigate To First", typeof(NavigateFirstCommand), iconName: "MvcContextEditor/NavigatePreviousIcon"));
            navigationCommands.addChild(new BrowserNode("Navigate To Last", typeof(NavigateLastCommand), iconName: "MvcContextEditor/NavigateNextIcon"));
            navigationCommands.addChild(new BrowserNode("Open Website", typeof(OpenWebsite), iconName: CommonResources.NoIcon));
            rootNode.addChild(navigationCommands);

            //Back
            BrowserNode backActions = new BrowserNode("Back", null, iconName: CommonResources.NoIcon);
            backActions.addChild(new BrowserNode("Go Back", typeof(GoBackAction), iconName: CommonResources.NoIcon));
            backActions.addChild(new BrowserNode("Record Back", typeof(RecordBackAction), iconName: CommonResources.NoIcon));
            rootNode.addChild(backActions);

            //Mvc Actions
            BrowserNode mvcActions = new BrowserNode("Mvc", null, iconName: "MvcContextEditor/MVCcomIcon");
            mvcActions.addChild(new BrowserNode("Run Action", typeof(RunActionCommand), iconName: "MvcContextEditor/MVCRunACtionIcon"));
            mvcActions.addChild(new BrowserNode("Shutdown", typeof(ShutdownCommand), iconName: "MvcContextEditor/MVCShutdownIcon"));
            rootNode.addChild(mvcActions);

            //Data Collection
            BrowserNode dataCollection = new BrowserNode("Data", null, iconName: CommonResources.NoIcon);
            dataCollection.addChild(new BrowserNode("Collect Form Data", typeof(CollectFormData), iconName: CommonResources.NoIcon));
            dataCollection.addChild(new BrowserNode("Set Data", typeof(SetData), iconName: CommonResources.NoIcon));
            dataCollection.addChild(new BrowserNode("Populate Form Data", typeof(PopulateFormData), iconName: CommonResources.NoIcon));
            dataCollection.addChild(new BrowserNode("Analyze Form Data", typeof(AnalyzeFormData), iconName: CommonResources.NoIcon));
            rootNode.addChild(dataCollection);

            //Muscle Position
            BrowserNode musclePosition = new BrowserNode("Muscle Position", null, iconName: "MvcContextEditor/MusclePositionIcon");
            musclePosition.addChild(new BrowserNode("Change Muscle Position", typeof(SetMusclePositionCommand), iconName: "MvcContextEditor/MusclePositionChangeIcon"));
            musclePosition.addChild(new BrowserNode("Save Muscle Position", typeof(SaveMusclePositionCommand), iconName: "MvcContextEditor/MusclePositionSaveIcon"));
            musclePosition.addChild(new BrowserNode("Restore Muscle Position", typeof(RestoreMusclePositionCommand), iconName: "MvcContextEditor/MusclePositionRestoreIcon"));
            rootNode.addChild(musclePosition);

            //Teeth Highlight
            BrowserNode teethHighlights = new BrowserNode("Teeth Highlights", null, iconName: CommonResources.NoIcon);
            teethHighlights.addChild(new BrowserNode("Highlight Teeth", typeof(ChangeTeethHighlightsCommand), iconName: CommonResources.NoIcon));
            teethHighlights.addChild(new BrowserNode("Save Highlight Teeth", typeof(SaveTeethHighlightsCommand), iconName: CommonResources.NoIcon));
            teethHighlights.addChild(new BrowserNode("Restore Highlight Teeth", typeof(RestoreTeethHighlightsCommand), iconName: CommonResources.NoIcon));
            rootNode.addChild(teethHighlights);

            return browser;
        }
    }
}