using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;
using Medical.Muscles;
using Engine.Platform;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    public class OffsetSequenceEditor : LayoutComponent, EditMenuProvider
    {
        public const float Duration = 10.0f;
        private const String DefaultTargetLabelText = "No Target Selected";

        private TimelineDataProperties actionProperties;
        private TrackFilter trackFilter;
        private TimelineView timelineView;
        private NumberLine numberLine;
        private SaveableClipboard clipboard;
        private MedicalController medicalController;
        private SimObjectMover simObjectMover;
        private KeyframeMovableObject keyframeMover;

        private OffsetModifierSequence offsetSequence;
        private OffsetModifierPlayer player;

        private TextBox targetLabel;

        public OffsetSequenceEditor(SaveableClipboard clipboard, MyGUIViewHost viewHost, OffsetSequenceEditorView view, MedicalController medicalController)
            : base("Medical.GUI.OffsetSequence.OffsetSequenceEditor.layout", viewHost)
        {
            this.clipboard = clipboard;
            this.medicalController = medicalController;
            this.simObjectMover = view.SimObjectMover;

            widget.KeyButtonReleased += new MyGUIEvent(window_KeyButtonReleased);
            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

            //Remove button
            Button removeButton = widget.findWidget("RemoveAction") as Button;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);

            //Timeline view
            ScrollView timelineViewScrollView = widget.findWidget("ActionView") as ScrollView;
            timelineView = new TimelineView(timelineViewScrollView);
            timelineView.Duration = Duration;
            timelineView.KeyReleased += new EventHandler<KeyEventArgs>(timelineView_KeyReleased);
            timelineView.ActiveDataChanged += timelineView_ActiveDataChanged;

            //Properties
            ScrollView timelinePropertiesScrollView = widget.findWidget("ActionPropertiesScrollView") as ScrollView;
            actionProperties = new TimelineDataProperties(timelinePropertiesScrollView, timelineView);
            actionProperties.Visible = false;
            actionProperties.addPanel("Offset Position", new OffsetKeyframeProperties(timelinePropertiesScrollView, this, view.UICallback));

            keyframeMover = new KeyframeMovableObject();
            simObjectMover.addMovableObject("KeyframeMover", keyframeMover);

            //Timeline filter
            ScrollView timelineFilterScrollView = widget.findWidget("ActionFilter") as ScrollView;
            trackFilter = new TrackFilter(timelineFilterScrollView, timelineView);
            trackFilter.AddTrackItem += new AddTrackItemCallback(trackFilter_AddTrackItem);

            numberLine = new NumberLine(widget.findWidget("NumberLine") as ScrollView, timelineView);

            //Add tracks to timeline.
            timelineView.addTrack("Offset Position");

            ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);

            Button chooseTargetButton = (Button)widget.findWidget("ChooseTargetButton");
            chooseTargetButton.MouseButtonClick += chooseTargetButton_MouseButtonClick;

            targetLabel = (TextBox)widget.findWidget("TargetLabel");
            targetLabel.Caption = DefaultTargetLabelText;

            CurrentSequence = view.Sequence;
        }

        public override void Dispose()
        {
            simObjectMover.removeMovableObject(keyframeMover);
            simObjectMover.Visible = false;
            Player = null;//Reset the player, this does a lot of cleanup so don't remove it
            actionProperties.Dispose();
            base.Dispose();
        }

        public void reverseSides()
        {
            if (offsetSequence != null)
            {
                //offsetSequence.reverseSides();
            }
        }

        public void cut()
        {
            OffsetSequenceClipboardContainer clipContainer = new OffsetSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
            deleteSelectedActions();
        }

        public void copy()
        {
            OffsetSequenceClipboardContainer clipContainer = new OffsetSequenceClipboardContainer();
            clipContainer.addKeyFrames(timelineView.SelectedData);
            clipboard.copyToSourceObject(clipContainer);
        }

        public void paste()
        {
            OffsetSequenceClipboardContainer clipContainer = clipboard.createCopy<OffsetSequenceClipboardContainer>();
            if (clipContainer != null)
            {
                clipContainer.addKeyFramesToSequence(offsetSequence, this, timelineView.MarkerTime / Duration, 1.0f);
            }
        }

        public void selectAll()
        {
            timelineView.selectAll();
        }

        public OffsetModifierSequence CurrentSequence
        {
            get
            {
                return offsetSequence;
            }
            set
            {
                offsetSequence = value;
                timelineView.removeAllData();
                if (offsetSequence != null)
                {
                    timelineView.Duration = Duration;
                    foreach (var state in offsetSequence.Keyframes)
                    {
                        addToTimeline(state);
                    }

                    if (offsetSequence.SimObjectHint != null && offsetSequence.PlayerNameHint != null)
                    {
                        var simObject = medicalController.getSimObject(offsetSequence.SimObjectHint);
                        if (simObject != null)
                        {
                            Player = simObject.getElement(offsetSequence.PlayerNameHint) as OffsetModifierPlayer;
                        }
                        else
                        {
                            Player = null;
                        }
                    }
                }
                else
                {
                    timelineView.Duration = Duration;
                }
            }
        }

        internal void addToTimeline(OffsetModifierKeyframe state)
        {
            timelineView.addData(new OffsetKeyframeData(state, offsetSequence, this));
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (offsetSequence != null)
            {
                OffsetKeyframeData data = timelineView.CurrentData as OffsetKeyframeData;
                timelineView.removeData(data);
                offsetSequence.removeFrame(data.KeyFrame);
            }
        }

        private void deleteSelectedActions()
        {
            foreach (OffsetKeyframeData data in timelineView.SelectedData)
            {
                timelineView.removeData(data);
                offsetSequence.removeFrame(data.KeyFrame);
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

        void trackFilter_AddTrackItem(string name, Object trackUserObject)
        {
            if (offsetSequence != null)
            {
                OffsetModifierKeyframe keyframe = new OffsetModifierKeyframe();
                if(Player != null)
                {
                    keyframe.deriveOffsetFromFollower(Player.Follower);
                }
                keyframe.BlendAmount = timelineView.MarkerTime / Duration;
                offsetSequence.addKeyframe(keyframe);
                offsetSequence.sort();
                addToTimeline(keyframe);
            }
        }

        void widget_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
            }
        }

        void chooseTargetButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Browser browser = new Browser("Offset Modifier Players", "Choose a player");
            foreach(var player in OffsetModifierPlayer.EditablePlayers)
            {
                browser.addNode(player.Owner.Name, new BrowserNode(player.Name, player));
            }
            BrowserWindow<OffsetModifierPlayer>.GetInput(browser, true, getBrowseResult);
        }

        bool getBrowseResult(OffsetModifierPlayer result, ref String errorMessage)
        {
            Player = result;
            return true;
        }

        public OffsetModifierPlayer Player
        {
            get
            {
                return player;
            }
            set
            {
                if (player != null)
                {
                    player.BlendDriver.BlendAmountChanged -= BlendDriver_BlendAmountChanged;
                    player.restoreDefaultSequence();
                }
                player = value;
                if(player != null)
                {
                    player.BlendDriver.BlendAmountChanged += BlendDriver_BlendAmountChanged;
                    if(CurrentSequence != null)
                    {
                        player.Sequence = CurrentSequence;
                        CurrentSequence.SimObjectHint = player.Owner.Name;
                        CurrentSequence.PlayerNameHint = player.Name;
                    }
                    timelineView.MarkerTime = player.BlendDriver.BlendAmount;
                    targetLabel.Caption = String.Format("{0} - {1}", player.Owner.Name, player.Name);
                    keyframeMover.Follower = player.Follower;
                    simObjectMover.Visible = true;
                }
                else
                {
                    targetLabel.Caption = DefaultTargetLabelText;
                    keyframeMover.Follower = null;
                    simObjectMover.Visible = false;
                }
            }
        }

        void BlendDriver_BlendAmountChanged(BlendDriver obj)
        {
            timelineView.MarkerTime = obj.BlendAmount * Duration;
        }

        void timelineView_ActiveDataChanged(object sender, EventArgs e)
        {
            OffsetKeyframeData data = (OffsetKeyframeData)timelineView.CurrentData;
            if(data != null)
            {
                keyframeMover.Keyframe = data.KeyFrame;
            }
            else
            {
                keyframeMover.Keyframe = null;
            }
        }
    }
}
