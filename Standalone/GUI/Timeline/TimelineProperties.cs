using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine;

namespace Medical.GUI
{
    class TimelineProperties : Dialog
    {
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
        private OpenTimelineDialog openTimelineDialog;
        private SaveTimelineDialog saveTimelineDialog;

        private Button playButton;

        private const int START_COLUMN_WIDTH = 100;

        public TimelineProperties(TimelineController timelineController)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.timelineController = timelineController;
            timelineController.PlaybackStarted += new EventHandler(timelineController_PlaybackStarted);
            timelineController.PlaybackStopped += new EventHandler(timelineController_PlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);

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
            MenuItem newTimeline = fileMenu.addItem("New Timeline");
            newTimeline.MouseButtonClick += new MyGUIEvent(newTimeline_MouseButtonClick);
            MenuItem openTimeline = fileMenu.addItem("Open Timeline");
            openTimeline.MouseButtonClick += new MyGUIEvent(openTimeline_MouseButtonClick);
            MenuItem saveTimeline = fileMenu.addItem("Save Timeline");
            saveTimeline.MouseButtonClick += new MyGUIEvent(saveTimeline_MouseButtonClick);
            MenuItem saveTimelineAs = fileMenu.addItem("Save Timeline As");
            saveTimelineAs.MouseButtonClick += new MyGUIEvent(saveTimelineAs_MouseButtonClick);
            fileMenuButton = new ShowMenuButton(fileButton, fileMenu);

            //Other Actions Menu
            Button otherActionsButton = window.findWidget("OtherActionsButton") as Button;
            otherActionsMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            otherActionsMenu.Visible = false;
            MenuItem changeScene = otherActionsMenu.addItem("Change Scene");
            changeScene.MouseButtonClick += new MyGUIEvent(changeScene_MouseButtonClick);
            otherActionsMenu.addItem("", MenuItemType.Separator);
            testActions = otherActionsMenu.addItem("Enable Other Actions");
            testActions.StateCheck = true;
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

            //Dialogs
            changeSceneEditor = new ChangeSceneEditor();
            newProjectDialog = new NewProjectDialog();
            newProjectDialog.ProjectCreated += new EventHandler(newProjectDialog_ProjectCreated);
            openTimelineDialog = new OpenTimelineDialog(timelineController);
            openTimelineDialog.OpenFile += new EventHandler(openTimelineDialog_OpenFile);
            saveTimelineDialog = new SaveTimelineDialog();
            saveTimelineDialog.SaveFile += new EventHandler(saveTimelineDialog_SaveFile);

            createNewTimeline();
        }

        public override void Dispose()
        {
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
            currentTimelineFile = filename;
            updateWindowCaption();
            currentTimeline = timeline;
            timelineView.removeAllData();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                timelineView.addData(new TimelineActionData(action));
            }
            currentTimeline.ActionAdded += currentTimeline_ActionAdded;
            currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
            changeSceneEditor.CurrentTimeline = currentTimeline;
        }

        private void updateWindowCaption()
        {
            if (timelineController.ResourceDirectory != null)
            {
                if (currentTimelineFile != null)
                {
                    window.Caption = String.Format("Timeline - {0} - {1}", Path.GetFileName(currentTimelineFile), timelineController.ResourceDirectory);
                }
                else
                {
                    window.Caption = String.Format("Timeline - {0}", timelineController.ResourceDirectory);
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

        void newProject_MouseButtonClick(Widget source, EventArgs e)
        {
            newProjectDialog.open(true);
            newProjectDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
        }

        void openProject_MouseButtonClick(Widget source, EventArgs e)
        {
            using (wx.DirDialog dirDialog = new wx.DirDialog(MainWindow.Instance, "Choose the path to load files from.", newProjectDialog.ProjectLocation))
            {
                if (dirDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    timelineController.ResourceDirectory = dirDialog.Path;
                    updateWindowCaption();
                }
            }
        }

        void newProjectDialog_ProjectCreated(object sender, EventArgs e)
        {
            timelineController.ResourceDirectory = newProjectDialog.FullProjectName;
            updateWindowCaption();
        }

        void saveTimelineAs_MouseButtonClick(Widget source, EventArgs e)
        {
            saveTimelineDialog.open(true);
            saveTimelineDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileMenu.setVisibleSmooth(false);
        }

        void saveTimelineDialog_SaveFile(object sender, EventArgs e)
        {
            timelineController.saveTimeline(currentTimeline, saveTimelineDialog.Filename);
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
            openTimelineDialog.open(true);
            openTimelineDialog.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileMenu.setVisibleSmooth(false);
        }

        void openTimelineDialog_OpenFile(object sender, EventArgs e)
        {
            setCurrentTimeline(timelineController.openTimeline(openTimelineDialog.SelectedFile), openTimelineDialog.SelectedFile);
        }

        void newTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            createNewTimeline();
            fileMenu.setVisibleSmooth(false);
        }

        #endregion

        #region Pre/Post Actions Menu

        void testActions_MouseButtonClick(Widget source, EventArgs e)
        {
            testActions.StateCheck = !testActions.StateCheck;
        }

        void changeScene_MouseButtonClick(Widget source, EventArgs e)
        {
            changeSceneEditor.open(true);
            changeSceneEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
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
        }

        void timelineController_TimeTicked(float currentTime)
        {
            timelineView.MarkerTime = currentTime;
        }

        #endregion
    }
}
