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
        private ComboBox addActionCombo;
        private Timeline currentTimeline;
        private TimelineController timelineController;
        private MenuCtrl fileMenuCtrl;
        private String currentTimelineFile;
        private ActionProperties actionProperties;
        private ActionFilter actionFilter;
        private ActionView actionView;
        private NumberLine numberLine;

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
            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem fileMenu = menuBar.addItem("File", MenuItemType.Popup);
            fileMenuCtrl = fileMenu.createItemChild();
            MenuItem newTimeline = fileMenuCtrl.addItem("New");
            newTimeline.MouseButtonClick += new MyGUIEvent(newTimeline_MouseButtonClick);
            MenuItem openTimeline = fileMenuCtrl.addItem("Open");
            openTimeline.MouseButtonClick += new MyGUIEvent(openTimeline_MouseButtonClick);
            MenuItem saveTimeline = fileMenuCtrl.addItem("Save");
            saveTimeline.MouseButtonClick += new MyGUIEvent(saveTimeline_MouseButtonClick);
            MenuItem saveTimelineAs = fileMenuCtrl.addItem("Save As");
            saveTimelineAs.MouseButtonClick += new MyGUIEvent(saveTimelineAs_MouseButtonClick);

            //Add action combo box.
            addActionCombo = window.findWidget("AddActionCombo") as ComboBox;
            foreach (TimelineActionProperties actionProp in TimelineActionFactory.ActionProperties)
            {
                addActionCombo.addItem(actionProp.TypeName);
                addActionCombo.setItemDataAt(addActionCombo.getItemCount() - 1, actionProp);
            }
            addActionCombo.SelectedIndex = 0;

            //Add action button
            Button addActionButton = window.findWidget("AddAction") as Button;
            addActionButton.MouseButtonClick += new MyGUIEvent(addActionButton_MouseButtonClick);
            
            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            //Action Properties
            ScrollView actionPropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new ActionProperties(actionPropertiesScrollView);
            actionProperties.Visible = false;

            //Action view
            ScrollView actionViewScrollView = window.findWidget("ActionView") as ScrollView;
            actionView = new ActionView(actionViewScrollView);
            actionView.ActiveActionChanged += new EventHandler(actionView_ActiveActionChanged);

            //Action filter
            ScrollView actionFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            actionFilter = new ActionFilter(actionFilterScrollView, actionView);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, actionView);

            createNewTimeline();
        }

        public override void Dispose()
        {
            actionFilter.Dispose();
            actionView.Dispose();
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
            actionView.removeAllActions();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                actionView.addAction(action);
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
            currentTimeline.removeAction(actionView.CurrentAction.Action);
        }

        void addActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            TimelineAction action = TimelineActionFactory.createAction((TimelineActionProperties)addActionCombo.getItemDataAt(addActionCombo.SelectedIndex));
            action.StartTime = actionView.MarkerTime;
            currentTimeline.addAction(action);
            action.capture();
            actionView.setCurrentAction(action);
        }

        void actionView_ActiveActionChanged(object sender, EventArgs e)
        {
            if (actionView.CurrentAction != null)
            {
                actionProperties.CurrentAction = actionView.CurrentAction;
                actionProperties.Visible = true;
            }
            else
            {
                actionProperties.CurrentAction = null;
                actionProperties.Visible = false;
            }
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            ActionViewButton button = actionView.addAction(e.Action);
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            actionView.removeAction(e.Action);
        }

        #endregion

        #region File Menu

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
            fileMenuCtrl.Visible = false;
        }

        void newTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            createNewTimeline();
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
            actionView.MarkerTime = currentTime;
        }

        #endregion
    }
}
