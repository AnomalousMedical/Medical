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
        private MenuItem fileMenu;
        private String currentTimelineFile;
        private MultiList actionList;

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
            fileMenu = menuBar.addItem("File", MenuItemType.Popup);
            MenuCtrl fileMenuCtrl = fileMenu.createItemChild();
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

            //Action list
            actionList = window.findWidget("ActionList") as MultiList;
            actionList.addColumn("Start", START_COLUMN_WIDTH);
            actionList.addColumn("Action", actionList.Width - START_COLUMN_WIDTH);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            actionList.setColumnWidthAt(1, actionList.Width - START_COLUMN_WIDTH);
        }

        public void setCurrentTimeline(Timeline timeline)
        {
            setCurrentTimeline(timeline, null);
        }

        public void setCurrentTimeline(Timeline timeline, String filename)
        {
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
            actionList.removeAllItems();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                actionList.addItem(action.TypeName);
                uint newIndex = actionList.getItemCount() - 1;
                actionList.setSubItemNameAt(0, newIndex, action.StartTime.ToString());
                actionList.setSubItemNameAt(1, newIndex, action.TypeName);
            }
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
            fileMenu.Visible = false;
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
            else
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
    }
}
