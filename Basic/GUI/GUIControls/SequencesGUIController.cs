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
        private KryptonRibbonTab sequenceTab;
        private MovementSequenceController sequenceController;
        private KryptonRibbonGroupButton previousClickedButton;
        private KryptonContextMenu sequenceMenu;
        private KryptonContextMenuItems sequenceMenuItems;
        private KryptonCommand playCommand;
        private KryptonCommand stopCommand;
        private KryptonRibbonGroupLabel nowPlayingLabel;

        public SequencesGUIController(BasicForm form, BasicController basicController)
        {
            this.sequenceTab = form.sequencesTab;
            this.sequenceController = basicController.MovementSequenceController;
            this.sequenceMenu = form.sequenceMenu;
            this.playCommand = form.playCommand;
            this.stopCommand = form.stopCommand;
            this.nowPlayingLabel = form.nowPlayingLabel;
            sequenceMenuItems = new KryptonContextMenuItems();
            sequenceMenu.Items.Add(sequenceMenuItems);

            sequenceController.CurrentSequenceSetChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceSetChanged);
            playCommand.Execute += new EventHandler(playCommand_Execute);
            stopCommand.Execute += new EventHandler(stopCommand_Execute);
        }

        void sequenceController_CurrentSequenceSetChanged(MovementSequenceController controller)
        {
            //sequenceTab.Groups.Clear();
            MovementSequenceSet currentSet = controller.SequenceSet;
            foreach (MovementSequenceGroup sequenceGroup in currentSet.Groups)
            {
                KryptonRibbonGroup group = new KryptonRibbonGroup();
                group.TextLine1 = sequenceGroup.Name;
                sequenceTab.Groups.Add(group);
                KryptonRibbonGroupTriple triple = null;
                foreach (MovementSequenceInfo sequenceInfo in sequenceGroup.Sequences)
                {
                    if (triple == null || triple.Items.Count > 2)
                    {
                        triple = new KryptonRibbonGroupTriple();
                        group.Items.Add(triple);
                    }
                    KryptonRibbonGroupButton button = new KryptonRibbonGroupButton();
                    button.TextLine1 = sequenceInfo.Name;
                    button.Tag = sequenceInfo.FileName;
                    if (sequenceInfo.Thumbnail == null)
                    {
                        button.ImageLarge = Resources.SequenceIconLarge;
                        button.ImageSmall = Resources.SequenceIconSmall;
                    }
                    else
                    {
                        button.ImageLarge = sequenceInfo.Thumbnail;
                        button.ImageSmall = sequenceInfo.Thumbnail;
                    }
                    button.ButtonType = GroupButtonType.Check;
                    button.Click += new EventHandler(button_Click);
                    triple.Items.Add(button);

                    //Menu
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
                playCommand.Enabled = true;
                stopCommand.Enabled = false;
                nowPlayingLabel.TextLine2 = item.Text;
                nowPlayingLabel.ImageLarge = item.Image;
            }
        }

        void stopCommand_Execute(object sender, EventArgs e)
        {
            sequenceController.stopPlayback();
            playCommand.Enabled = true;
            stopCommand.Enabled = false;
        }

        void playCommand_Execute(object sender, EventArgs e)
        {
            sequenceController.playCurrentSequence();
            playCommand.Enabled = false;
            stopCommand.Enabled = true;
        }

        void button_Click(object sender, EventArgs e)
        {
            KryptonRibbonGroupButton currentButton = sender as KryptonRibbonGroupButton;
            if (currentButton != null)
            {
                if (currentButton != previousClickedButton)
                {
                    if (previousClickedButton != null)
                    {
                        previousClickedButton.Checked = false;
                    }
                    MovementSequence sequence = sequenceController.loadSequence(currentButton.Tag.ToString());
                    sequenceController.CurrentSequence = sequence;
                    sequenceController.playCurrentSequence();
                    previousClickedButton = currentButton;
                }
                else
                {
                    if (!previousClickedButton.Checked)
                    {
                        sequenceController.stopPlayback();
                    }
                    else
                    {
                        sequenceController.playCurrentSequence();
                    }
                }
            }
        }
    }
}
