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

namespace Medical.GUI
{
    class MovementSequenceEditor : MDIDialog
    {
        private PopupMenu fileMenu;
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

        private MovementSequenceController movementSequenceController;

        public MovementSequenceEditor(MovementSequenceController movementSequenceController)
            : base("Medical.GUI.MovementSequence.MovementSequenceEditor.layout")
        {
            this.movementSequenceController = movementSequenceController;
            movementSequenceController.CurrentSequenceChanged += new MovementSequenceEvent(movementSequenceController_CurrentSequenceChanged);
            movementSequenceController.PlaybackStarted += new MovementSequenceEvent(movementSequenceController_PlaybackStarted);
            movementSequenceController.PlaybackStopped += new MovementSequenceEvent(movementSequenceController_PlaybackStopped);
            movementSequenceController.PlaybackUpdate += new MovementSequenceEvent(movementSequenceController_PlaybackUpdate);

            //Menu
            Button fileButton = window.findWidget("FileButton") as Button;
            fileMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
            fileMenu.Visible = false;
            MenuItem newSequence = fileMenu.addItem("New");
            newSequence.MouseButtonClick += new MyGUIEvent(newSequence_MouseButtonClick);
            MenuItem openSequence = fileMenu.addItem("Open");
            openSequence.MouseButtonClick += new MyGUIEvent(openSequence_MouseButtonClick);
            MenuItem saveSequence = fileMenu.addItem("Save");
            saveSequence.MouseButtonClick += new MyGUIEvent(saveSequence_MouseButtonClick);
            MenuItem saveSequenceAs = fileMenu.addItem("Save As");
            saveSequenceAs.MouseButtonClick += new MyGUIEvent(saveSequenceAs_MouseButtonClick);
            showMenuButton = new ShowMenuButton(fileButton, fileMenu);
            fileMenu.addItem("Sep", MenuItemType.Separator);
            MenuItem reverseSides = fileMenu.addItem("Reverse Sides");
            reverseSides.MouseButtonClick += new MyGUIEvent(reverseSides_MouseButtonClick);

            //Remove button
            Button removeButton = window.findWidget("RemoveAction") as Button;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);
            
            //Duration Edit
            durationEdit = new NumericEdit(window.findWidget("SequenceDuration") as Edit);
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

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(fileMenu);
            base.Dispose();
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

        #region MovementSequenceController Callbacks

        void movementSequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            playButton.Caption = "Play";
            playButton.StaticImage.setItemResource("Timeline/PlayIcon");
        }

        void movementSequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            playButton.Caption = "Stop";
            playButton.StaticImage.setItemResource("Timeline/StopIcon");
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

        #endregion

        #region Timeline Callbacks

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
            timelineView.addData(new MovementKeyframeData(state, movementSequenceController.CurrentSequence));
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

        #endregion

        #region File Menu

        void newSequence_MouseButtonClick(Widget source, EventArgs e)
        {
            createNewSequence();
            fileMenu.setVisibleSmooth(false);
        }

        void saveSequenceAs_MouseButtonClick(Widget source, EventArgs e)
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
            fileMenu.setVisibleSmooth(false);
        }

        void saveSequence_MouseButtonClick(Widget source, EventArgs e)
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
                saveSequenceAs_MouseButtonClick(source, e);
            }
            fileMenu.setVisibleSmooth(false);
        }

        void openSequence_MouseButtonClick(Widget source, EventArgs e)
        {
            using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Open a sequence."))
            {
                openDialog.Wildcard = "Sequence files (*.seq)|*.seq";
                if (openDialog.showModal() == NativeDialogResult.OK)
                {
                    using (XmlReader xmlReader = new XmlTextReader(openDialog.Path))
                    {
                        loadingSequenceFromFile = true;
                        CurrentSequenceFile = openDialog.Path;
                        MovementSequence movementSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                        movementSequenceController.CurrentSequence = movementSequence;
                        loadingSequenceFromFile = false;
                    }
                }
            }
            fileMenu.setVisibleSmooth(false);
        }

        void reverseSides_MouseButtonClick(Widget source, EventArgs e)
        {
            if (movementSequenceController.CurrentSequence != null)
            {
                movementSequenceController.CurrentSequence.reverseSides();
            }
            fileMenu.setVisibleSmooth(false);
        }

        #endregion
    }
}
