using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Platform;
using Engine;
using Logging;
using System.IO;
using System.Xml;

namespace Medical.GUI
{
    public class TimelineEditor : MDIDialog
    {
        public const String TIMELINE_WILDCARD = "Timelines (*.tl)|*.tl";

        public event EventDelegate<TimelineEditor, float> MarkerMoved;

        private String windowTitle;
        private const String windowTitleFormat = "{0} - {1}";

        private TimelineController timelineController;
        private TrackFilter actionFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Dictionary<TimelineAction, TimelineActionData> actionDataBindings = new Dictionary<TimelineAction, TimelineActionData>();
        private EditorController editorController;
        private SaveableClipboard clipboard;
        private TimelineActionFactory actionFactory;
        private TimelineDataProperties dataProperties;
        private TimelineData editingStoppedLastData;
        private Timeline currentTimeline;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();

        private Button playButton;
        private Button rewindButton;
        private Button fastForwardButton;

        private String currentFile;

        public TimelineEditor(TimelineController timelineController, EditorController editorController, SaveableClipboard clipboard, EditorPlugin editorPlugin)
            :base("Medical.GUI.TimelineEditor.TimelineEditor.layout")
        {
            windowTitle = window.Caption;

            this.clipboard = clipboard;
            this.editorController = editorController;

            this.timelineController = timelineController;
            timelineController.TimelinePlaybackStarted += new EventHandler(timelineController_TimelinePlaybackStarted);
            timelineController.TimelinePlaybackStopped += new EventHandler(timelineController_TimelinePlaybackStopped);
            timelineController.TimeTicked += new TimeTickEvent(timelineController_TimeTicked);

            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);
            window.RootMouseChangeFocus += new MyGUIEvent(window_RootMouseChangeFocus);

            //Remove action button
            Button removeActionButton = window.findWidget("RemoveAction") as Button;
            removeActionButton.MouseButtonClick += new MyGUIEvent(removeActionButton_MouseButtonClick);

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            fastForwardButton = window.findWidget("FastForward") as Button;
            fastForwardButton.MouseButtonClick += new MyGUIEvent(fastForwardButton_MouseButtonClick);

            rewindButton = window.findWidget("Rewind") as Button;
            rewindButton.MouseButtonClick += new MyGUIEvent(rewindButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);
            timelineView.ActiveDataChanging += new EventHandler<CancelEventArgs>(timelineView_ActiveDataChanging);
            timelineView.MarkerMoved += new EventDelegate<TimelineView, float>(timelineView_MarkerMoved);

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
            actionFactory = new TimelineActionFactory(propertiesScrollView, editorPlugin);
            foreach (TimelineActionFactoryData actionProp in actionFactory.ActionProperties)
            {
                timelineView.addTrack(actionProp.TypeName, actionProp.Color);

                if (actionProp.Panel != null)
                {
                    dataProperties.addPanel(actionProp.TypeName, actionProp.Panel);
                }
            }

            extensionActions.Add(new ExtensionAction("Save Timeline", "File", saveTimeline));
            extensionActions.Add(new ExtensionAction("Save Timeline As", "File", saveTimelineAs));
            extensionActions.Add(new ExtensionAction("Cut", "Edit", cut));
            extensionActions.Add(new ExtensionAction("Copy", "Edit", copy));
            extensionActions.Add(new ExtensionAction("Paste", "Edit", paste));
            extensionActions.Add(new ExtensionAction("Select All", "Edit", selectAll));
        }

        public void loadTimeline(String filename)
        {
            CurrentTimeline = timelineController.openTimeline(filename);
            currentFile = filename;
            currentFileChanged();
            timelineView.removeAllData();
            foreach (TimelineAction action in currentTimeline.Actions)
            {
                addActionToTimeline(action);
            }
        }

        public void saveTimeline()
        {
            if (currentFile != null)
            {
                saveTimeline(currentTimeline, currentFile);
            }
            else
            {
                saveTimelineAs();
            }
        }

        public void saveTimelineAs()
        {
            using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save Timeline", "", "", TIMELINE_WILDCARD))
            {
                if (saveDialog.showModal() == NativeDialogResult.OK)
                {
                    saveTimeline(currentTimeline, saveDialog.Path);
                }
            }
        }

        public void paste()
        {
            TimelineActionClipboardContainer clipContainer = clipboard.createCopy<TimelineActionClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addActionsToTimeline(currentTimeline, timelineView.MarkerTime);
            }
        }

        public void copy()
        {
            TimelineActionClipboardContainer clipContainer = new TimelineActionClipboardContainer();
            clipContainer.addActions(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        public void cut()
        {
            copy();
            deleteSelectedActions();
        }

        public void selectAll()
        {
            timelineView.selectAll();
        }

        public void activateExtensionActions()
        {
            editorController.ExtensionActions = extensionActions;
        }

        public Timeline CurrentTimeline
        {
            get
            {
                return currentTimeline;
            }
            set
            {
                if (currentTimeline != value)
                {
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded -= currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved -= currentTimeline_ActionRemoved;
                    }
                    currentTimeline = value;
                    timelineController.setAsTimelineController(currentTimeline);
                    if (currentTimeline != null)
                    {
                        currentTimeline.ActionAdded += currentTimeline_ActionAdded;
                        currentTimeline.ActionRemoved += currentTimeline_ActionRemoved;
                    }
                }
            }
        }

        void window_RootMouseChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                activateExtensionActions();
            }
        }

        void removeActionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            TimelineActionData data = (TimelineActionData)timelineView.CurrentData;
            if (data != null)
            {
                stopTimelineIfPlaying();
                currentTimeline.removeAction(data.Action);
            }
        }

        void actionFilter_AddTrackItem(string name)
        {
            TimelineAction action = actionFactory.createAction(name);
            action.StartTime = timelineView.MarkerTime;
            currentTimeline.addAction(action);
            action.capture();
            timelineView.CurrentData = actionDataBindings[action];
        }

        void timelineController_TimelinePlaybackStopped(object sender, EventArgs e)
        {
            playButton.Caption = "Play";
            playButton.ImageBox.setItemResource("Timeline/PlayIcon");
            rewindButton.Enabled = true;
            fastForwardButton.Enabled = true;
            if (currentTimeline != null)
            {
                timelineController.setAsTimelineController(currentTimeline);
            }
            timelineView.CurrentData = editingStoppedLastData;
        }

        void timelineController_TimelinePlaybackStarted(object sender, EventArgs e)
        {
            playButton.Caption = "Stop";
            playButton.ImageBox.setItemResource("Timeline/StopIcon");
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

        private void deleteSelectedActions()
        {
            stopTimelineIfPlaying();
            foreach (TimelineActionData data in timelineView.SelectedData)
            {
                currentTimeline.removeAction(data.Action);
            }
        }

        void window_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            processFormKeys(ke);
        }

        void timelineView_KeyReleased(object sender, KeyEventArgs e)
        {
            processFormKeys(e);
        }

        void processFormKeys(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case KeyboardButtonCode.KC_DELETE:
                    deleteSelectedActions();
                    break;
                case KeyboardButtonCode.KC_SPACE:
                    togglePlayPreview(timelineView.MarkerTime);
                    break;
            }
        }

        public void togglePlayPreview(float startTime)
        {
            if (timelineController.Playing)
            {
                timelineController.stopPlayback(false);
            }
            else if (currentTimeline != null)
            {
                editingStoppedLastData = timelineView.CurrentData;
                timelineView.CurrentData = null;
                timelineController.startPlayback(currentTimeline, startTime, false);
            }
        }

        void timelineView_ActiveDataChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = timelineController.Playing;
        }

        void timelineView_MarkerMoved(TimelineView source, float arg)
        {
            if (MarkerMoved != null)
            {
                MarkerMoved.Invoke(this, arg);
            }
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
            togglePlayPreview(timelineView.MarkerTime);
        }

        private void currentFileChanged()
        {
            if (currentFile == null)
            {
                window.Caption = windowTitle;
            }
            else
            {
                window.Caption = String.Format(windowTitleFormat, windowTitle, currentFile);
            }
        }

        void currentTimeline_ActionRemoved(object sender, TimelineActionEventArgs e)
        {
            removeActionFromTimeline(e.Action);
        }

        void currentTimeline_ActionAdded(object sender, TimelineActionEventArgs e)
        {
            addActionToTimeline(e.Action);
        }

        void removeActionFromTimeline(TimelineAction action)
        {
            timelineView.removeData(actionDataBindings[action]);
        }

        void addActionToTimeline(TimelineAction action)
        {
            TimelineActionData data = new TimelineActionData(action);
            actionDataBindings.Add(action, data);
            timelineView.addData(data);
        }

        private void saveTimeline(Timeline timeline, String filename)
        {
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Create, FileAccess.Write))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default))
                    {
                        writer.Formatting = Formatting.Indented;
                        EditorController.XmlSaver.saveObject(timeline, writer);
                    }
                }
                timeline.SourceFile = filename;
                currentFile = filename;
                currentFileChanged();
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your timeline to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save timeline. {0}", ex.Message);
            }
        }
    }
}
