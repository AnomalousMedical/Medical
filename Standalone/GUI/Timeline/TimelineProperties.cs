using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class TimelineProperties : Dialog
    {
        private Timeline currentTimeline;
        private TimelineController timelineController;
        private PopupMenu fileMenu;
        private String currentTimelineFile;
        private TimelineDataProperties dataProperties;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private Dictionary<String, TimelineActionProperties> properties = new Dictionary<string, TimelineActionProperties>();

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

            //Menu
            Button fileButton = window.findWidget("FileButton") as Button;
            fileButton.MouseButtonClick += new MyGUIEvent(fileButton_MouseButtonClick);
            fileMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
            fileMenu.Visible = false;
            MenuItem newTimeline = fileMenu.addItem("New");
            newTimeline.MouseButtonClick += new MyGUIEvent(newTimeline_MouseButtonClick);
            MenuItem openTimeline = fileMenu.addItem("Open");
            openTimeline.MouseButtonClick += new MyGUIEvent(openTimeline_MouseButtonClick);
            MenuItem saveTimeline = fileMenu.addItem("Save");
            saveTimeline.MouseButtonClick += new MyGUIEvent(saveTimeline_MouseButtonClick);
            MenuItem saveTimelineAs = fileMenu.addItem("Save As");
            saveTimelineAs.MouseButtonClick += new MyGUIEvent(saveTimelineAs_MouseButtonClick);
           
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

            createNewTimeline();
        }

        public override void Dispose()
        {
            actionFilter.Dispose();
            timelineView.Dispose();
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
            if (currentTimelineFile != null)
            {
                window.Caption = String.Format("Timeline - {0}", currentTimelineFile);
            }
            else
            {
                window.Caption = "Timeline";
            }
            currentTimeline = timeline;
            timelineView.removeAllData();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                timelineView.addData(new TimelineActionData(action));
            }
            currentTimeline.ActionAdded += currentTimeline_ActionAdded;
            currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
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

        void fileButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fileMenu.setPosition(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
            LayerManager.Instance.upLayerItem(fileMenu);
            fileMenu.setVisibleSmooth(true);
        }

        void saveTimelineAs_MouseButtonClick(Widget source, EventArgs e)
        {
            using (wx.FileDialog saveDialog = new wx.FileDialog(MainWindow.Instance, "Save a timeline"))
            {
                saveDialog.StyleFlags = wx.WindowStyles.FD_SAVE;
                saveDialog.Wildcard = "Timeline files (*.tl)|*.tl";
                if (saveDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    timelineController.saveTimeline(currentTimeline, saveDialog.Path);
                }
            }
            fileMenu.setVisibleSmooth(false);
        }

        void saveTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentTimelineFile != null)
            {
                timelineController.saveTimeline(currentTimeline, currentTimelineFile);
            }
            else
            {
                saveTimelineAs_MouseButtonClick(source, e);
            }
            fileMenu.setVisibleSmooth(false);
        }

        void openTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            using (wx.FileDialog openDialog = new wx.FileDialog(MainWindow.Instance, "Open a timeline"))
            {
                openDialog.StyleFlags = wx.WindowStyles.FD_OPEN;
                openDialog.Wildcard = "Timeline files (*.tl)|*.tl";
                if (openDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    setCurrentTimeline(timelineController.openTimeline(openDialog.Path), openDialog.Path);
                }
            }
            fileMenu.setVisibleSmooth(false);
        }

        void newTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            createNewTimeline();
            fileMenu.setVisibleSmooth(false);
        }

        #endregion

        #region Playback

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback();
            }
            else if (currentTimeline != null)
            {
                timelineController.startPlayback(currentTimeline);
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
