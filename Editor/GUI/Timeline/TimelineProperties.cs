using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine;
using Logging;
using Engine.Saving;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical.GUI
{
    class TimelineProperties : MDIDialog
    {
        public const String PROJECT_EXTENSION = ".tlp";
        public const String PROJECT_WILDCARD = "All Timeline Types (*.tlp, *.tix, *.tl)|*.tlp;*.tix;*.tl|Timeline Projects (*.tlp)|*.tlp|Timeline Indexes (*.tix)|*.tix|Timelines(*.tl)|*.tl";

        private TimelineController timelineController;
        private TimelineDataProperties dataProperties;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private TimelineAction copySourceAction;
        private CopySaver copySaver = new CopySaver();
        private TimelineActionFactory actionFactory;
        private TimelinePropertiesController timelinePropertiesController;

        //Menus
        private ShowMenuButton editMenuButton;
        private PopupMenu editMenu;
        private ShowMenuButton otherActionsMenuButton;
        private PopupMenu otherActionsMenu;
        private ShowMenuButton analyzeMenuButton;
        private PopupMenu analyzeMenu;

        //Dialogs
        private StartActionEditor startActionEditor;
        private FinishActionEditor finishActionEditor;
        private TimelineIndexEditor timelineIndexEditor;

        private Button playButton;
        private Button playFullButton;
        private Button rewindButton;
        private Button fastForwardButton;

        private const int START_COLUMN_WIDTH = 100;

        public TimelineProperties(TimelineController timelineController, EditorPlugin editorPlugin, GUIManager guiManager, TimelinePropertiesController timelinePropertiesController, TimelineFileBrowserDialog fileBrowserDialog)
            :base("Medical.GUI.Timeline.TimelineProperties.layout")
        {
            this.timelinePropertiesController = timelinePropertiesController;

            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += new EventHandler(timelineController_TimelinePlaybackStarted);
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);
            timelineController.ResourceLocationChanged += new EventHandler(timelineController_ResourceLocationChanged);

            MenuBar menuBar = window.findWidget("MenuBar") as MenuBar;

            //Edit button
            MenuItem editMenuItem = menuBar.addItem("Edit");
            editMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            editMenu.Visible = false;
            MenuItem copy = editMenu.addItem("Copy");
            copy.MouseButtonClick += new MyGUIEvent(copy_MouseButtonClick);
            MenuItem paste = editMenu.addItem("Paste");
            paste.MouseButtonClick += new MyGUIEvent(paste_MouseButtonClick);
            editMenuButton = new ShowMenuButton(editMenuItem, editMenu);

            //Other Actions Menu
            MenuItem actionsMenuItem = menuBar.addItem("Actions");
            otherActionsMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            otherActionsMenu.Visible = false;
            MenuItem editTimelineIndex = otherActionsMenu.addItem("Edit Timeline Index");
            editTimelineIndex.MouseButtonClick += new MyGUIEvent(editTimelineIndex_MouseButtonClick);
            MenuItem startAction = otherActionsMenu.addItem("Start Action");
            startAction.MouseButtonClick += new MyGUIEvent(startAction_MouseButtonClick);
            MenuItem finishAction = otherActionsMenu.addItem("Finish Action");
            finishAction.MouseButtonClick += new MyGUIEvent(finishAction_MouseButtonClick);
            MenuItem reverseSidesAction = otherActionsMenu.addItem("Reverse Sides");
            reverseSidesAction.MouseButtonClick += new MyGUIEvent(reverseSidesAction_MouseButtonClick);
            otherActionsMenuButton = new ShowMenuButton(actionsMenuItem, otherActionsMenu);

            //Analyze Menu
            MenuItem analyzeMenuItem = menuBar.addItem("Analyze");
            analyzeMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            analyzeMenu.Visible = false;
            MenuItem dumpPostActions = analyzeMenu.addItem("Dump PostActions to Log");
            dumpPostActions.MouseButtonClick += new MyGUIEvent(dumpPostActions_MouseButtonClick);
            analyzeMenuButton = new ShowMenuButton(analyzeMenuItem, analyzeMenu);
           
            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            playFullButton = window.findWidget("PlayFull") as Button;
            playFullButton.MouseButtonClick += new MyGUIEvent(playFullButton_MouseButtonClick);

            fastForwardButton = window.findWidget("FastForward") as Button;
            fastForwardButton.MouseButtonClick += new MyGUIEvent(fastForwardButton_MouseButtonClick);

            rewindButton = window.findWidget("Rewind") as Button;
            rewindButton.MouseButtonClick += new MyGUIEvent(rewindButton_MouseButtonClick);

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
            startActionEditor = new StartActionEditor(fileBrowserDialog, timelineController);

            finishActionEditor = new FinishActionEditor(timelineController, fileBrowserDialog, guiManager);

            timelineIndexEditor = new TimelineIndexEditor(fileBrowserDialog);
            timelineIndexEditor.SaveIndexData += new EventHandler(timelineIndexEditor_SaveIndexData);

            //Add tracks to timeline.
            actionFactory = new TimelineActionFactory(propertiesScrollView, editorPlugin);
            foreach (TimelineActionFactoryData actionProp in actionFactory.ActionProperties)
            {
                timelineView.addTrack(actionProp.TypeName, actionProp.Color);

                if (actionProp.Panel != null)
                {
                    dataProperties.addPanel(actionProp.TypeName, actionProp.Panel);
                }
            }

            setEnabled(false);
        }

        public override void Dispose()
        {
            actionFactory.Dispose();
            finishActionEditor.Dispose();
            timelineController.FileBrowser = null;
            startActionEditor.Dispose();
            actionFilter.Dispose();
            timelineView.Dispose();
            Gui.Instance.destroyWidget(editMenu);
            Gui.Instance.destroyWidget(otherActionsMenu);
            Gui.Instance.destroyWidget(analyzeMenu);
            base.Dispose();
        }

        public void setCurrentTimeline(Timeline timeline)
        {
            timelineView.removeAllData();
            foreach (TimelineAction action in timeline.Actions)
            {
                addActionToTimeline(action);
            }
            startActionEditor.CurrentTimeline = timeline;
            finishActionEditor.CurrentTimeline = timeline;
            updateWindowCaption();
        }

        public void updateWindowCaption()
        {
            if (timelineController.ResourceProvider != null && timelinePropertiesController.CurrentTimelineFile != null)
            {
                window.Caption = String.Format("Timeline - {0}", Path.GetFileName(timelinePropertiesController.CurrentTimelineFile));
            }
            else
            {
                window.Caption = "Timeline";
            }
        }

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelinePropertiesController.CurrentTimeline.removeAction(((TimelineActionData)timelineView.CurrentData).Action);
        }

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = actionFactory.createAction(name);
            action.StartTime = timelineView.MarkerTime;
            timelinePropertiesController.CurrentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataBindings[action];
        }

        public void addActionToTimeline(TimelineAction action)
        {
            TimelineActionData data = new TimelineActionData(action);
            actionDataBindings.Add(action, data);
            timelineView.addData(data);
        }

        public void removeActionFromTimeline(TimelineAction action)
        {
            timelineView.removeData(actionDataBindings[action]);
        }

        void timelineController_ResourceLocationChanged(object sender, EventArgs e)
        {
            setEnabled(timelineController.ResourceProvider != null);
            updateWindowCaption();
        }

        private void setEnabled(bool enabled)
        {
            actionFilter.Enabled = enabled;
            otherActionsMenuButton.Enabled = enabled;
            editMenuButton.Enabled = enabled;
            playButton.Enabled = enabled;
            playFullButton.Enabled = enabled;
            timelineView.Enabled = enabled;
            fastForwardButton.Enabled = enabled;
            rewindButton.Enabled = enabled;
            analyzeMenuButton.Enabled = enabled;
        }

        void rewindButton_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelineView.MarkerTime = 0.0f;
        }

        void fastForwardButton_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelineView.MarkerTime += 10.0f;
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelinePropertiesController.playPreview(timelineView.MarkerTime);
        }

        void playFullButton_MouseButtonClick(Widget source, EventArgs e)
        {
            timelinePropertiesController.playFull();
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            playButton.Caption = "Play";
            playButton.StaticImage.setItemResource("Timeline/PlayIcon");
            rewindButton.Enabled = true;
            fastForwardButton.Enabled = true;
        }

        void timelineController_TimelinePlaybackStarted(object sender, EventArgs e)
        {
            playButton.Caption = "Stop";
            playButton.StaticImage.setItemResource("Timeline/StopIcon");
            rewindButton.Enabled = false;
            fastForwardButton.Enabled = false;
            timelineView.CurrentData = null;
        }

        void timelineController_TimeTicked(float currentTime)
        {
            timelineView.MarkerTime = currentTime;
        }

        private void stopTimelineIfPlaying()
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
        }

        #region More stuff to remove

        void paste_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            if (copySourceAction != null)
            {
                TimelineAction copiedAction = copySaver.copy<TimelineAction>(copySourceAction);
                copiedAction.StartTime = timelineView.MarkerTime;
                timelinePropertiesController.CurrentTimeline.addAction(copiedAction);
                timelineView.CurrentData = actionDataBindings[copiedAction];
            }
            editMenu.setVisibleSmooth(false);
        }

        void copy_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            TimelineActionData currentData = timelineView.CurrentData as TimelineActionData;
            if (currentData != null)
            {
                copySourceAction = copySaver.copy<TimelineAction>(currentData.Action);
            }
            editMenu.setVisibleSmooth(false);
        }

        void editTimelineIndex_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            timelineIndexEditor.setData(timelineController.CurrentTimelineIndex);
            timelineIndexEditor.open(true);
            timelineIndexEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            timelineIndexEditor.ensureVisible();
        }

        void timelineIndexEditor_SaveIndexData(object sender, EventArgs e)
        {
            TimelineIndex index = timelineIndexEditor.createIndex();
            timelineController.saveIndex(index);
        }

        void startAction_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            startActionEditor.open(false);
            startActionEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            startActionEditor.ensureVisible();
        }

        void reverseSidesAction_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            MessageBox.show("Reversing sides will attempt to help you make a timeline that works on the opposite side.\nIt can only reverse things on the x-axis meaning it will reverse stuff left to right.\n\nThe only things that can be reversed are:\n* Camera translation and look at.\n* Prop translation (rotations need to be fixed manually).\n* Movement sequence keyframes.\nContinue?", "Reverse", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, delegate(MessageBoxStyle result)
            {
                if (result == MessageBoxStyle.Yes)
                {
                    timelineController.EditingTimeline.reverseSides();
                }
            });
            
        }

        void finishAction_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            finishActionEditor.open(true);
            finishActionEditor.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            finishActionEditor.ensureVisible();
        }

        void dumpPostActions_MouseButtonClick(Widget source, EventArgs e)
        {
            stopTimelineIfPlaying();
            Log.Debug("");
            Log.Debug("");
            Log.Debug("Dumping Timeline Post Actions");
            String[] files = timelineController.listResourceFiles("*.tl");
            foreach(String file in files)
            {
                Log.Debug("-----------------------------------------------------------");
                Timeline tl = timelineController.openTimeline(file);
                Log.Debug("Dumping post actions for timeline \"{0}\".", tl.SourceFile);
                tl.dumpPostActionsToLog();
                analyzeMenu.setVisibleSmooth(false);
                Log.Debug("-----------------------------------------------------------");
                Log.Debug("");
            }
            Log.Debug("");
            Log.Debug("");
        }

        #endregion
    }
}
