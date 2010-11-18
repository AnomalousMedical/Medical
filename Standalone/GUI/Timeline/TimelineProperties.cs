using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine;
using Logging;

namespace Medical.GUI
{
    class TimelineProperties : Dialog
    {
        private const String PROJECT_EXTENSION = ".tlp";
        private const String PROJECT_WILDCARD = "Timeline Projects (*.tlp)|*.tlp";

        private Timeline currentTimeline;
        private TimelineController timelineController;
        private String currentTimelineFile;
        private TimelineDataProperties dataProperties;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private Dictionary<String, TimelineActionProperties> properties = new Dictionary<string, TimelineActionProperties>();
        private ShowMenuButton fileMenuButton;
        private PopupMenu fileMenu;
        private ShowMenuButton prePostActionsMenuButton;
        private PopupMenu otherActionsMenu;
        private MenuItem testActions;

        //Dialogs
        private ChangeSceneEditor changeSceneEditor;
        private NewProjectDialog newProjectDialog;
        private TimelineFileBrowserDialog fileBrowserDialog;
        private SaveTimelineDialog saveTimelineDialog;
        private FinishActionEditor finishActionEditor;

        private Button otherActionsButton;
        private Button playButton;

        private const int START_COLUMN_WIDTH = 100;

        //File Menu
        MenuItem newTimelineItem;
        MenuItem openTimelineItem;
        MenuItem saveTimelineItem;
        MenuItem saveTimelineAsItem;

        public TimelineProperties(TimelineController timelineController)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.timelineController = timelineController;
            timelineController.PlaybackStarted += new EventHandler(timelineController_PlaybackStarted);
            timelineController.PlaybackStopped += new EventHandler(timelineController_PlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);
            timelineController.ResourceLocationChanged += new EventHandler(timelineController_ResourceLocationChanged);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            //File Menu
            Button fileButton = window.findWidget("FileButton") as Button;
            fileMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            fileMenu.Visible = false;
            MenuItem newProject = fileMenu.addItem("New Project");
            newProject.MouseButtonClick += new MyGUIEvent(newProject_MouseButtonClick);
            MenuItem openProject = fileMenu.addItem("Open Project");
            openProject.MouseButtonClick += new MyGUIEvent(openProject_MouseButtonClick);
            fileMenu.addItem("", MenuItemType.Separator);
            newTimelineItem = fileMenu.addItem("New Timeline");
            newTimelineItem.MouseButtonClick += new MyGUIEvent(newTimeline_MouseButtonClick);
            openTimelineItem = fileMenu.addItem("Open Timeline");
            openTimelineItem.MouseButtonClick += new MyGUIEvent(openTimeline_MouseButtonClick);
            saveTimelineItem = fileMenu.addItem("Save Timeline");
            saveTimelineItem.MouseButtonClick += new MyGUIEvent(saveTimeline_MouseButtonClick);
            saveTimelineAsItem = fileMenu.addItem("Save Timeline As");
            saveTimelineAsItem.MouseButtonClick += new MyGUIEvent(saveTimelineAs_MouseButtonClick);
            fileMenuButton = new ShowMenuButton(fileButton, fileMenu);

            //Other Actions Menu
            otherActionsButton = window.findWidget("OtherActionsButton") as Button;
            otherActionsMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            otherActionsMenu.Visible = false;
            MenuItem changeScene = otherActionsMenu.addItem("Change Scene");
            changeScene.MouseButtonClick += new MyGUIEvent(changeScene_MouseButtonClick);
            MenuItem finishAction = otherActionsMenu.addItem("Finish Action");
            finishAction.MouseButtonClick += new MyGUIEvent(finishAction_MouseButtonClick);
            otherActionsMenu.addItem("", MenuItemType.Separator);
            testActions = otherActionsMenu.addItem("Enable Other Actions");
            testActions.StateCheck = false;
            testActions.MouseButtonClick += new MyGUIEvent(testActions_MouseButtonClick);
            prePostActionsMenuButton = new ShowMenuButton(otherActionsButton, otherActionsMenu);
           
            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);

            //Properties
            ScrollView propertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            dataProperties = new TimelineDataProperties(propertiesScrollView, timelineView);
            dataProperties.Visible = false;

            //Track filter
            ScrollView trackFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            actionFilter = new TrackFilter(trackFilterScrollView, timelineView);
            actionFilter.AddTrackItem += new AddTrackItemCallback(actionFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Dialogs
            changeSceneEditor = new ChangeSceneEditor();
            newProjectDialog = new NewProjectDialog(PROJECT_EXTENSION);
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);
            fileBrowserDialog = new TimelineFileBrowserDialog(timelineController);
            timelineController.FileBrowser = fileBrowserDialog;
            saveTimelineDialog = new SaveTimelineDialog();
            saveTimelineDialog.SaveFile += new EventHandler(saveTimelineDialog_SaveFile);
            finishActionEditor = new FinishActionEditor(timelineController, fileBrowserDialog);

            //Add tracks to timeline.
            object[] args = { propertiesScrollView };
            foreach (TimelineActionProperties actionProp in TimelineActionFactory.ActionProperties)
            {
                timelineView.addTrack(actionProp.TypeName, actionProp.Color);
                properties.Add(actionProp.TypeName, actionProp);

                if (actionProp.GUIType != null)
                {
                    try
                    {
                        TimelineDataPanel propPanel = (TimelineDataPanel)Activator.CreateInstance(actionProp.GUIType, args);
                        dataProperties.addPanel(actionProp.TypeName, propPanel);
                    }
                    catch (Exception)
                    {
                        throw new Exception(String.Format("Could not create the GUI for {0}. Make sure it extends ActionPropertiesPanel and has a constructor that takes a Widget."));
                    }
                }
            }

            setEnabled(false);

            createNewTimeline();
        }

        public override void Dispose()
        {
            finishActionEditor.Dispose();
            newProjectDialog.Dispose();
            timelineController.FileBrowser = null;
            fileBrowserDialog.Dispose();
            saveTimelineDialog.Dispose();
            changeSceneEditor.Dispose();
            actionFilter.Dispose();
            timelineView.Dispose();
            Gui.Instance.destroyWidget(fileMenu);
            Gui.Instance.destroyWidget(otherActionsMenu);
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            
        }

        public void setCurrentTimeline(Timeline timeline)
        {
            setCurrentTimeline(timeline, null);
        }

        public void setCurrentTimeline(Timeline timeline, String filename)
        {
            timelineController.EditingTimeline = timeline;
            if (currentTimeline != null)
            {
                currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                currentTimeline.ActionRemoved -= currentTimeline_ActionRemoved;
            }
            changeTimelineFile(filename);
            currentTimeline = timeline;
            timelineView.removeAllData();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                timelineView.addData(new TimelineActionData(action));
            }
            currentTimeline.ActionAdded += currentTimeline_ActionAdded;
            currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
            changeSceneEditor.CurrentTimeline = currentTimeline;
            finishActionEditor.CurrentTimeline = currentTimeline;
        }

        private void changeTimelineFile(String filename)
        {
            currentTimelineFile = filename;
            updateWindowCaption();
        }

        private void updateWindowCaption()
        {
            if (timelineController.ResourceLocation != null)
            {
                if (currentTimelineFile != null)
                {
                    window.Caption = String.Format("Timeline - {0} - {1}", Path.GetFileName(currentTimelineFile), timelineController.ResourceLocation);
                }
                else
                {
                    window.Caption = String.Format("Timeline - {0}", timelineController.ResourceLocation);
                }
            }
            else
            {
                window.Caption = "Timeline";
            }
        }

        public void createNewTimeline()
        {
            Timeline timeline = new Timeline();
            setCurrentTimeline(timeline);
        }

        #region Action Management

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            currentTimeline.removeAction(((TimelineActionData)timelineView.CurrentData).Action);
        }

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = TimelineActionFactory.createAction(properties[name]);
            action.StartTime = timelineView.MarkerTime;
            currentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataBindings[action];
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            TimelineActionData data = new TimelineActionData(e.Action);
            actionDataBindings.Add(e.Action, data);
            timelineView.addData(data);
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            timelineView.removeData(actionDataBindings[e.Action]);
        }

        #endregion

        #region File Menu

        /// <summary>
        /// Create a new project. You can optionally delete the old project.
        /// </summary>
        /// <param name="filename">The file name of the new project.</param>
        /// <param name="deleteOld">True to delete any existing project first.</param>
        void createNewProject(String filename, bool deleteOld)
        {
            try
            {
                if (deleteOld)
                {
                    File.Delete(filename);
                }
                timelineController.createProject(filename);
                updateWindowCaption();
            }
            catch (Exception ex)
            {
                String errorMessage = String.Format("Error creating new project {0}.", ex.Message);
                Log.Error(errorMessage);
                MessageBox.show(errorMessage, "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        void newProject_MouseButtonClick(Widget source, EventArgs e)
        {
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            newProjectDialog.ensureVisible();
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            String newProjectName = newProjectDialog.FullProjectName;
            if (File.Exists(newProjectName))
            {
                MessageBox.show(String.Format("A project named {0} already exists. Would you like to overwrite it?", newProjectName), "Overwrite?", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, overwriteNewProject);
            }
            else
            {
                createNewProject(newProjectName, false);
            }
        }

        void overwriteNewProject(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                createNewProject(newProjectDialog.FullProjectName, true);
            }
        }

        void openProject_MouseButtonClick(Widget source, EventArgs e)
        {
            using (wx.FileDialog fileDialog = new wx.FileDialog(MainWindow.Instance, "Open a timeline.", newProjectDialog.ProjectLocation, "", PROJECT_WILDCARD))
            {
                if (fileDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    timelineController.ResourceLocation = fileDialog.Path;
                    updateWindowCaption();
                }
            }
        }

        void saveTimelineAs_MouseButtonClick(Widget source, EventArgs e)
        {
            saveTimelineDialog.open(true);
            saveTimelineDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            saveTimelineDialog.ensureVisible();
            fileMenu.setVisibleSmooth(false);
        }

        void saveTimeline(Timeline timeline, String filename)
        {
            timelineController.saveTimeline(timeline, filename);
            changeTimelineFile(filename);
        }

        void saveTimelineDialog_SaveFile(object sender, EventArgs e)
        {
            String filename = saveTimelineDialog.Filename;
            if (timelineController.resourceExists(filename))
            {
                MessageBox.show(String.Format("The file {0} already exists. Would you like to overwrite it?", filename), "Overwrite?", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, overwriteTimelineResult);
            }
            else
            {
                saveTimeline(currentTimeline, filename);
            }
        }

        void overwriteTimelineResult(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                saveTimeline(currentTimeline, saveTimelineDialog.Filename);
            }
        }

        void saveTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentTimelineFile != null)
            {
                timelineController.saveTimeline(currentTimeline, currentTimelineFile);
                fileMenu.setVisibleSmooth(false);
            }
            else
            {
                saveTimelineAs_MouseButtonClick(source, e);
            }
        }

        void openTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowserDialog.promptForFile("*.tl", openTimelineDialog_OpenFile);
            fileBrowserDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileBrowserDialog.ensureVisible();
            fileMenu.setVisibleSmooth(false);
        }

        void openTimelineDialog_OpenFile(String filename)
        {
            setCurrentTimeline(timelineController.openTimeline(filename), filename);
        }

        void newTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            createNewTimeline();
            fileMenu.setVisibleSmooth(false);
        }

        void timelineController_ResourceLocationChanged(object sender, EventArgs e)
        {
            setEnabled(timelineController.ResourceLocation != null);
        }

        private void setEnabled(bool enabled)
        {
            newTimelineItem.Enabled = enabled;
            openTimelineItem.Enabled = enabled;
            saveTimelineItem.Enabled = enabled;
            saveTimelineAsItem.Enabled = enabled;
            actionFilter.Enabled = enabled;
            otherActionsButton.Enabled = enabled;
            playButton.Enabled = enabled;
            timelineView.Enabled = enabled;
        }

        #endregion

        #region Other Actions Menu

        void testActions_MouseButtonClick(Widget source, EventArgs e)
        {
            testActions.StateCheck = !testActions.StateCheck;
        }

        void changeScene_MouseButtonClick(Widget source, EventArgs e)
        {
            changeSceneEditor.open(true);
            changeSceneEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            changeSceneEditor.ensureVisible();
        }

        void finishAction_MouseButtonClick(Widget source, EventArgs e)
        {
            finishActionEditor.open(true);
            finishActionEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            finishActionEditor.ensureVisible();
        }

        #endregion

        #region Playback

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
            else if (currentTimeline != null)
            {
                timelineController.startPlayback(currentTimeline, testActions.StateCheck);
            }
        }

        void timelineController_PlaybackStopped(object sender, EventArgs e)
        {
            playButton.Caption = "Play";
        }

        void timelineController_PlaybackStarted(object sender, EventArgs e)
        {
            playButton.Caption = "Stop";
            if (timelineController.ActiveTimeline != currentTimeline)
            {
                setCurrentTimeline(timelineController.ActiveTimeline);
            }
        }

        void timelineController_TimeTicked(float currentTime)
        {
            timelineView.MarkerTime = currentTime;
        }

        #endregion
    }
}
