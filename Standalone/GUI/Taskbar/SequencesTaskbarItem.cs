using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Medical.Muscles;

namespace Medical.GUI
{
    public class SequencesTaskbarItem : TaskbarItem
    {
        private MovementSequenceController sequenceController;
        private PopupMenu sequenceMenu;

        public SequencesTaskbarItem(MovementSequenceController sequenceController)
            :base("Sequences", "SequenceIconLarge")
        {
            this.sequenceController = sequenceController;

            sequenceController.CurrentSequenceSetChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceSetChanged);
            sequenceController.PlaybackStarted += new MovementSequenceEvent(sequenceController_PlaybackStarted);
            sequenceController.PlaybackStopped += new MovementSequenceEvent(sequenceController_PlaybackStopped);
            sequenceController.CurrentSequenceChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceChanged);
        }

        public override void Dispose()
        {
            if (sequenceMenu != null)
            {
                Gui.Instance.destroyWidget(sequenceMenu);
            }
            base.Dispose();
        }

        void sequenceController_CurrentSequenceSetChanged(MovementSequenceController controller)
        {
            if (sequenceMenu != null)
            {
                Gui.Instance.destroyWidget(sequenceMenu);
            }
            MovementSequenceSet currentSet = controller.SequenceSet;

            sequenceMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "SequencesMenu") as PopupMenu;
            sequenceMenu.Visible = false;

            foreach (MovementSequenceGroup sequenceGroup in currentSet.Groups)
            {
                MenuItem groupItem = sequenceMenu.addItem(sequenceGroup.Name, MenuItemType.Popup);
                //groupItem.Image = Resources.SequenceIconLarge;
                MenuCtrl groupItemChild = groupItem.createItemChild();
                foreach (MovementSequenceInfo sequenceInfo in sequenceGroup.Sequences)
                {
                    MenuItem sequenceItem = groupItemChild.addItem(sequenceInfo.Name, MenuItemType.Normal);
                    sequenceItem.MouseButtonClick += sequenceItem_Click;
                    sequenceItem.UserObject = sequenceInfo.FileName;
                    //if (sequenceInfo.Thumbnail == null)
                    //{
                    //    sequenceItem.Image = Resources.SequenceIconLarge;
                    //}
                    //else
                    //{
                    //    sequenceItem.Image = sequenceInfo.Thumbnail;
                    //}
                }
            }
        }

        void sequenceItem_Click(Widget sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                MovementSequence sequence = sequenceController.loadSequence(item.UserObject.ToString());
                sequenceController.stopPlayback();
                sequenceController.CurrentSequence = sequence;
                sequenceController.playCurrentSequence();
            }
            sequenceMenu.setVisibleSmooth(false);
        }

        void sequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            taskbarButton.StateCheck = false;
        }

        void sequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            taskbarButton.StateCheck = true;
        }

        void sequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            //if (controller.CurrentSequence == null)
            //{
            //    nowPlayingLabel.Caption = "None";
            //    playButton.Enabled = false;
            //    stopButton.Enabled = false;
            //}
            //else
            //{
            //    nowPlayingLabel.Caption = controller.CurrentSequence.Name;
            //    playButton.Enabled = true;
            //    stopButton.Enabled = false;
            //}
        }

        #region ITaskbarItem Members

        public override void clicked(Widget source, EventArgs e)
        {
            if (sequenceController.Playing)
            {
                sequenceController.stopPlayback();
            }
            else
            {
                sequenceMenu.setVisibleSmooth(true);
                LayerManager.Instance.upLayerItem(sequenceMenu);
                sequenceMenu.setPosition(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
            }
        }

        #endregion
    }
}
