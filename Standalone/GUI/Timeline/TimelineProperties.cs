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

        private Button playButton;

        private const int START_COLUMN_WIDTH = 100;

        public TimelineProperties(TimelineController timelineController)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.timelineController = timelineController;
            timelineController.PlaybackStarted += new EventHandler(timelineController_PlaybackStarted);
            timelineController.PlaybackStopped += new EventHandler(timelineController_PlaybackStopped);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            //Menu
            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;
            MenuItem fileMenu = menuBar.addItem("File", MenuItemType.Popup);
            fileMenuCtrl = fileMenu.createItemChild();
            MenuItem openTimeline = fileMenuCtrl.addItem("Open");
            openTimeline.MouseButtonClick += new MyGUIEvent(openTimeline_MouseButtonClick);
            MenuItem saveTimeline = fileMenuCtrl.addItem("Save");
            saveTimeline.MouseButtonClick += new MyGUIEvent(saveTimeline_MouseButtonClick);
            MenuItem saveTimelineAs = fileMenuCtrl.addItem("Save As");
            saveTimelineAs.MouseButtonClick += new MyGUIEvent(saveTimelineAs_MouseButtonClick);

            //Add action combo box.
            addActionCombo = window.findWidget("AddActionCombo") as ComboBox;
            foreach (String actionName in TimelineActionFactory.ActionNames)
            {
                addActionCombo.addItem(actionName);
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

            //Action filter
            ScrollView actionFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            actionFilter = new ActionFilter(actionFilterScrollView);
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
            if (currentTimeline != null)
            {
                currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                currentTimeline.ActionStartTimeChanged -= currentTimeline_ActionStartTimeChanged;
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
            actionFilter.removeAllItems();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                actionFilter.actionAdded(action);
            }
            currentTimeline.ActionAdded += currentTimeline_ActionAdded;
            currentTimeline.ActionStartTimeChanged += currentTimeline_ActionStartTimeChanged;
            currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
        }

        void saveTimelineAs_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void saveTimeline_MouseButtonClick(Widget source, EventArgs e)
        {
            if (currentTimelineFile != null)
            {

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
                if (openDialog.ShowModal() == wx.ShowModalResult.OK)
                {
                    setCurrentTimeline(timelineController.openTimeline(openDialog.Path), openDialog.Path);
                }
            }
            fileMenuCtrl.Visible = false;
        }

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void addActionButton_MouseButtonClick(Widget source, EventArgs e)
        {

        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback();
            }
            else if(currentTimeline != null)
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

        #region CurrentTimeline callbacks

        void currentTimeline_ActionStartTimeChanged(object sender, TimelineActionEventArgs e)
        {
            
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            
        }

        #endregion
    }
}
