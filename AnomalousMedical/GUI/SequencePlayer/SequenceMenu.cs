using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using MyGUIPlugin;
using Medical.Muscles;

namespace Medical
{
    class SequenceMenu : PopupContainer
    {
        private MovementSequenceController sequenceController;
        private ButtonGrid buttonGrid;

        public SequenceMenu(MovementSequenceController sequenceController)
            :base("Medical.GUI.SequencePlayer.SequenceMenu.layout")
        {
            this.sequenceController = sequenceController;

            sequenceController.SequenceAdded += sequenceController_SequenceAdded;

            buttonGrid = new ButtonGrid(widget as ScrollView, new SingleSelectionStrategy(), new ButtonGridListLayout());
        }

        public override void Dispose()
        {
            buttonGrid.Dispose();
            base.Dispose();
        }

        void sequenceController_SequenceAdded(MovementSequenceController controller, MovementSequenceGroup group, MovementSequenceInfo sequenceInfo)
        {
            ButtonGridItem item = buttonGrid.addItem(group.Name, sequenceInfo.Name);
            item.ItemClicked += (s, e) =>
                {
                    MovementSequence sequence = sequenceController.loadSequence(sequenceInfo);
                    sequenceController.stopPlayback();
                    sequenceController.CurrentSequence = sequence;
                    sequenceController.playCurrentSequence();
                    this.hide();
                };
        }
    }
}
