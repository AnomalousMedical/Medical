using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;
using Medical.Muscles;
using Engine.Saving.XMLSaver;
using System.Xml;
using Engine.Platform;

namespace Medical.GUI
{
    public class MovementSequenceEditor : MDIDialog
    {
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Button playButton;
        private NumericEdit durationEdit;
        private bool allowSynchronization = true;
        private String currentSequenceFile = null;
        private bool loadingSequenceFromFile = false;
        private XmlSaver xmlSaver = new XmlSaver();
        private ShowMenuButton showMenuButton;
        private SaveableClipboard clipboard;
        private EditorController editorController;

        private MovementSequenceController movementSequenceController;

        public MovementSequenceEditor(MovementSequenceController movementSequenceController, SaveableClipboard clipboard, EditorController editorController)
            : base("Medical.GUI.MovementSequence.MovementSequenceEditor.layout")
        {
            this.clipboard = clipboard;
            this.editorController = editorController;

            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);
            window.RootMouseChangeFocus += new MyGUIEvent(window_RootMouseChangeFocus);

            this.movementSequenceController = movementSequenceController;
            movementSequenceController.CurrentSequenceChanged += new MovementSequenceEvent(movementSequenceController_CurrentSequenceChanged);
            movementSequenceController.PlaybackStarted += new MovementSequenceEvent(movementSequenceController_PlaybackStarted);
            movementSequenceController.PlaybackStopped += new MovementSequenceEvent(movementSequenceController_PlaybackStopped);
            movementSequenceController.PlaybackUpdate += new MovementSequenceEvent(movementSequenceController_PlaybackUpdate);

            //Menu
            extensionActions.Add(new ExtensionAction("Save Movement Sequence", "File", saveSequence));
            extensionActions.Add(new ExtensionAction("Save Movement Sequence As", "File", saveSequenceAs));
            extensionActions.Add(new ExtensionAction("Cut", "Edit", cut));
            extensionActions.Add(new ExtensionAction("Copy", "Edit", copy));
            extensionActions.Add(new ExtensionAction("Paste", "Edit", paste));
            extensionActions.Add(new ExtensionAction("Select All", "Edit", selectAll));
            extensionActions.Add(new ExtensionAction("Reverse Sides", "Sequence", reverseSides));

            //Remove button
            Button removeButton = window.findWidget("RemoveAction") as Button;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);
            
            //Duration Edit
            durationEdit = new NumericEdit(window.findWidget("SequenceDuration") as EditBox);
            durationEdit.AllowFloat = true;
            durationEdit.ValueChanged += new MyGUIEvent(durationEdit_ValueChanged);
            durationEdit.MinValue = 0.0f;
            durationEdit.MaxValue = 600;

            //Play Button
            playButton = window.findWidget("PlayButton") as Button;
            playButton.MouseButtonClick += new MyGUIEvent(playButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = window.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.DurationChanged += new EventHandler(timelineView_DurationChanged);
            timelineView.Duration = 5.0f;
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionProperties.addPanel("Muscle Position", new MovementKeyframeProperties(timelinePropertiesScrollView));

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            timelineView.addTrack("Muscle Position", Color.Red);
        }

        public void activateExtensionActions()
        {
            editorController.ExtensionActions = extensionActions;
        }

        void window_RootMouseChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfae = (RootFocusEventArgs)e;
            if (rfae.Focus)
            {
                activateExtensionActions();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void openSequence(String filename)
        {
            try
            {
                using (XmlReader xmlReader = new XmlTextReader(filename))
                {
                    loadingSequenceFromFile = true;
                    CurrentSequenceFile = filename;
                    MovementSequence movementSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                    movementSequenceController.CurrentSequence = movementSequence;
                    loadingSequenceFromFile = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error opening movement sequence {0}.\nReason: {1}", filename, ex.Message), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        internal void addStateToTimeline(MovementSequenceState state)
        {
            timelineView.addData(new MovementKeyframeData(state, movementSequenceController.CurrentSequence));
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (movementSequenceController.Playing)
            {
                movementSequenceController.stopPlayback();
            }
            else
            {
                movementSequenceController.playCurrentSequence();
            }
        }

        void createNewSequence()
        {
            MovementSequence movementSequence = new MovementSequence();
            movementSequence.Duration = 5.0f;
            movementSequenceController.CurrentSequence = movementSequence;
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MovementKeyframeData data = timelineView.CurrentData as MovementKeyframeData;
            timelineView.removeData(data);
            movementSequenceController.CurrentSequence.deleteState(data.KeyFrame);
        }

        private void deleteSelectedActions()
        {
            foreach (MovementKeyframeData data in timelineView.SelectedData)
            {
                timelineView.removeData(data);
                movementSequenceController.CurrentSequence.deleteState(data.KeyFrame);
            }
        }

        private String CurrentSequenceFile
        {
            get
            {
                return currentSequenceFile;
            }
            set
            {
                currentSequenceFile = value;
                if (currentSequenceFile != null)
                {
                    window.Caption = String.Format("Movement Sequence - {0}", currentSequenceFile);
                }
                else
                {
                    window.Caption = "Movement Sequence";
                }
            }
        }

        void window_KeyButtonReleased(Widget source, EventArgs e)
        {
            processKeys((KeyEventArgs)e);
        }

        void timelineView_KeyReleased(object sender, KeyEventArgs e)
        {
            processKeys(e);
        }

        private void processKeys(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case KeyboardButtonCode.KC_DELETE:
                    deleteSelectedActions();
                    break;
            }
        }

        void movementSequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            playButton.Caption = "Play";
            playButton.ImageBox.setItemResource("Timeline/PlayIcon");
        }

        void movementSequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            playButton.Caption = "Stop";
            playButton.ImageBox.setItemResource("Timeline/StopIcon");
        }

        void movementSequenceController_PlaybackUpdate(MovementSequenceController controller)
        {
            timelineView.MarkerTime = controller.CurrentTime;
        }

        void movementSequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            if (!loadingSequenceFromFile)
            {
                CurrentSequenceFile = null;
            }
            timelineView.removeAllData();
            MovementSequence movementSequence = controller.CurrentSequence;
            if (movementSequence != null)
            {
                timelineView.Duration = movementSequence.Duration;
                foreach (MovementSequenceState state in movementSequence.States)
                {
                    timelineView.addData(new MovementKeyframeData(state, movementSequence));
                }
            }
            else
            {
                timelineView.Duration = 5.0f;
            }
        }

        void trackFilter_AddTrackItem(string name)
        {
            if (movementSequenceController.CurrentSequence == null)
            {
                createNewSequence();
            }
            MovementSequenceState state = new MovementSequenceState();
            state.captureState();
            state.StartTime = timelineView.MarkerTime;
            movementSequenceController.CurrentSequence.addState(state);
            addStateToTimeline(state);
        }

        private void synchronizeDuration(object sender, float duration)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (sender != durationEdit)
                {
                    durationEdit.FloatValue = duration;
                }
                if (sender != timelineView)
                {
                    timelineView.Duration = duration;
                }
                if (sender != movementSequenceController.CurrentSequence && movementSequenceController.CurrentSequence != null)
                {
                    movementSequenceController.CurrentSequence.Duration = duration;
                }
                allowSynchronization = true;
            }
        }

        void timelineView_DurationChanged(object sender, EventArgs e)
        {
            synchronizeDuration(sender, timelineView.Duration);
        }

        void durationEdit_ValueChanged(Widget source, EventArgs e)
        {
            synchronizeDuration(durationEdit, durationEdit.FloatValue);
        }

        void saveSequenceAs()
        {
            using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Save a sequence."))
            {
                saveDialog.Wildcard = "Sequence files (*.seq)|*.seq";
                if (saveDialog.showModal() == NativeDialogResult.OK)
                {
                    using(XmlTextWriter textWriter = new XmlTextWriter(saveDialog.Path, Encoding.Default))
                    {
                        textWriter.Formatting = Formatting.Indented;
                        xmlSaver.saveObject(movementSequenceController.CurrentSequence, textWriter);
                        CurrentSequenceFile = saveDialog.Path;
                    }
                }
            }
        }

        void saveSequence()
        {
            if (CurrentSequenceFile != null)
            {
                using (XmlTextWriter textWriter = new XmlTextWriter(CurrentSequenceFile, Encoding.Default))
                {
                    textWriter.Formatting = Formatting.Indented;
                    xmlSaver.saveObject(movementSequenceController.CurrentSequence, textWriter);
                }
            }
            else
            {
                saveSequenceAs();
            }
        }

        void openSequence()
        {
            using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Open a sequence."))
            {
                openDialog.Wildcard = "Sequence files (*.seq)|*.seq";
                if (openDialog.showModal() == NativeDialogResult.OK)
                {
                    openSequence(openDialog.Path);
                }
            }
        }

        void reverseSides()
        {
            if (movementSequenceController.CurrentSequence != null)
            {
                movementSequenceController.CurrentSequence.reverseSides();
            }
        }

        void cut()
        {
            MovementSequenceClipboardContainer clipContainer = new MovementSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            deleteSelectedActions();
        }

        void copy()
        {
            MovementSequenceClipboardContainer clipContainer = new MovementSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        void paste()
        {
            MovementSequenceClipboardContainer clipContainer = clipboard.createCopy<MovementSequenceClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addKeyFramesToSequence(movementSequenceController.CurrentSequence, this, timelineView.MarkerTime, timelineView.Duration);
            }
        }

        void selectAll()
        {
            timelineView.selectAll();
        }
    }
}
