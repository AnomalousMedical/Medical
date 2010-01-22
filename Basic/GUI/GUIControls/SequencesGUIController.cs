using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Engine.ObjectManagement;
using Engine.Resources;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using Medical.Properties;
using Medical.Muscles;

namespace Medical.GUI
{
    class SequencesGUIController
    {
        private MovementSequenceController sequenceController;
        private KryptonContextMenu sequenceMenu;
        private KryptonContextMenuItems sequenceMenuItems;
        private KryptonCommand playCommand;
        private KryptonCommand stopCommand;
        private KryptonRibbonGroupLabel nowPlayingLabel;

        public SequencesGUIController(BasicForm form, BasicController basicController)
        {
            this.sequenceController = basicController.MovementSequenceController;
            this.sequenceMenu = form.sequenceMenu;
            this.playCommand = form.playCommand;
            this.stopCommand = form.stopCommand;
            this.nowPlayingLabel = form.nowPlayingLabel;
            sequenceMenuItems = new KryptonContextMenuItems();
            sequenceMenu.Items.Add(sequenceMenuItems);

            sequenceController.CurrentSequenceSetChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceSetChanged);
            sequenceController.PlaybackStarted += new MovementSequenceEvent(sequenceController_PlaybackStarted);
            sequenceController.PlaybackStopped += new MovementSequenceEvent(sequenceController_PlaybackStopped);
            sequenceController.CurrentSequenceChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceChanged);
            playCommand.Execute += new EventHandler(playCommand_Execute);
            stopCommand.Execute += new EventHandler(stopCommand_Execute);
        }

        void sequenceController_CurrentSequenceSetChanged(MovementSequenceController controller)
        {
            MovementSequenceSet currentSet = controller.SequenceSet;
            sequenceMenuItems.Items.Clear();
            foreach (MovementSequenceGroup sequenceGroup in currentSet.Groups)
            {
                KryptonContextMenuHeading heading = new KryptonContextMenuHeading(sequenceGroup.Name);
                sequenceMenuItems.Items.Add(heading);
                foreach (MovementSequenceInfo sequenceInfo in sequenceGroup.Sequences)
                {
                    KryptonContextMenuItem sequenceItem = new KryptonContextMenuItem(sequenceInfo.Name);
                    sequenceItem.Click += sequenceItem_Click;
                    sequenceItem.Tag = sequenceInfo.FileName;
                    if (sequenceInfo.Thumbnail == null)
                    {
                        sequenceItem.Image = Resources.SequenceIconLarge;
                    }
                    else
                    {
                        sequenceItem.Image = sequenceInfo.Thumbnail;
                    }
                    sequenceMenuItems.Items.Add(sequenceItem);
                }
            }
        }

        void sequenceItem_Click(object sender, EventArgs e)
        {
            KryptonContextMenuItem item = sender as KryptonContextMenuItem;
            if (item != null)
            {
                MovementSequence sequence = sequenceController.loadSequence(item.Tag.ToString());
                sequenceController.stopPlayback();
                sequenceController.CurrentSequence = sequence;
                //nowPlayingLabel.TextLine2 = item.Text;
                //nowPlayingLabel.ImageLarge = item.Image;
            }
        }

        void stopCommand_Execute(object sender, EventArgs e)
        {
            sequenceController.stopPlayback();
        }

        void playCommand_Execute(object sender, EventArgs e)
        {
            sequenceController.playCurrentSequence();
        }

        void sequenceController_PlaybackStopped(MovementSequenceController controller)
        {
            playCommand.Enabled = true;
            stopCommand.Enabled = false;
        }

        void sequenceController_PlaybackStarted(MovementSequenceController controller)
        {
            playCommand.Enabled = false;
            stopCommand.Enabled = true;
        }

        void sequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            if (controller.CurrentSequence == null)
            {
                nowPlayingLabel.TextLine2 = "None";
                playCommand.Enabled = false;
                stopCommand.Enabled = false;
            }
            else
            {
                nowPlayingLabel.TextLine2 = controller.CurrentSequence.Name;
                playCommand.Enabled = true;
                stopCommand.Enabled = false;
            }
        }
    }
}
