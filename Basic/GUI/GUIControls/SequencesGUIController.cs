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

        public SequencesGUIController(BasicForm form, BasicController basicController)
        {
            this.sequenceTab = form.sequencesTab;
            this.sequenceController = basicController.MovementSequenceController;

            sequenceController.CurrentSequenceChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceChanged);
            sequenceController.CurrentSequenceSetChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceSetChanged);
        }

        void sequenceController_CurrentSequenceSetChanged(MovementSequenceController controller)
        {
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
                }
            }
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

        void sequenceController_CurrentSequenceChanged(MovementSequenceController controller)
        {
            
        }
    }
}
