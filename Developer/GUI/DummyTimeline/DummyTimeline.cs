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
using Medical.GUI;

namespace Developer
{
    public class DummyTimeline : MDIDialog
    {
        private PopupMenu fileMenu;
        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private Button playButton;
        private NumericEdit durationEdit;
        private ShowMenuButton showMenuButton;

        public DummyTimeline()
            : base("Developer.GUI.DummyTimeline.DummyTimeline.layout")
        {
            window.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);

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
            fileMenu.addItem("Sep2", MenuItemType.Separator);
            MenuItem cut = fileMenu.addItem("Cut");
            cut.MouseButtonClick += new MyGUIEvent(cut_MouseButtonClick);
            MenuItem copy = fileMenu.addItem("Copy");
            copy.MouseButtonClick += new MyGUIEvent(copy_MouseButtonClick);
            MenuItem paste = fileMenu.addItem("Paste");
            paste.MouseButtonClick += new MyGUIEvent(paste_MouseButtonClick);

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
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);

            //Properties
            ScrollView timelinePropertiesScrollView = window.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            //actionProperties.addPanel("Muscle Position", new MovementKeyframeProperties(timelinePropertiesScrollView));

            //Timeline filter
            ScrollView timelineFilterScrollView = window.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(window.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            timelineView.addTrack("Dummy", Color.Red);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(fileMenu);
            base.Dispose();
        }

        void playButton_MouseButtonClick(Widget source, EventArgs e)
        {
            int numButtons = 100;
            for (int i = 0; i < numButtons; ++i)
            {
                Logging.Log.Debug("{0} of {1}", i, numButtons);
                timelineView.addData(new DummyTimelineData(i % 5, 1));
            }
        }

        void createNewSequence()
        {
            
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            DummyTimelineData data = timelineView.CurrentData as DummyTimelineData;
            timelineView.removeData(data);
        }

        private void deleteSelectedActions()
        {
            foreach (DummyTimelineData data in timelineView.SelectedData)
            {
                timelineView.removeData(data);
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

        #region Timeline Callbacks

        void trackFilter_AddTrackItem(string name)
        {
            timelineView.addData(new DummyTimelineData());
        }

        void timelineView_DurationChanged(object sender, EventArgs e)
        {
            
        }

        void durationEdit_ValueChanged(Widget source, EventArgs e)
        {
            
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
            
        }

        void saveSequence_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void openSequence_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void reverseSides_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void cut_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void copy_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        void paste_MouseButtonClick(Widget source, EventArgs e)
        {
            
        }

        #endregion
    }
}
