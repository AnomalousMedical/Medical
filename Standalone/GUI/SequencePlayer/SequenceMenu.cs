using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using MyGUIPlugin;
using Medical.Muscles;

namespace Medical
{
    class SequenceMenu : IDisposable
    {
        private MovementSequenceController sequenceController;
        private PopupMenu sequenceMenu;

        public SequenceMenu(MovementSequenceController sequenceController)
        {
            this.sequenceController = sequenceController;

            sequenceController.CurrentSequenceSetChanged += new MovementSequenceEvent(sequenceController_CurrentSequenceSetChanged);
        }

        public void Dispose()
        {
            if (sequenceMenu != null)
            {
                Gui.Instance.destroyWidget(sequenceMenu);
            }
        }

        public void show(int left, int top)
        {
            sequenceMenu.setVisibleSmooth(true);
            LayerManager.Instance.upLayerItem(sequenceMenu);
            sequenceMenu.setPosition(left, top);
        }

        public void hide()
        {
            sequenceMenu.setVisibleSmooth(false);
        }

        void sequenceController_CurrentSequenceSetChanged(MovementSequenceController controller)
        {
            if (sequenceMenu != null)
            {
                Gui.Instance.destroyWidget(sequenceMenu);
            }
            MovementSequenceSet currentSet = controller.SequenceSet;

            sequenceMenu = Gui.Instance.createWidgetT("PopupMenu", "LargeIconPopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "SequencesMenu") as PopupMenu;
            sequenceMenu.Visible = false;

            foreach (MovementSequenceGroup sequenceGroup in currentSet.Groups)
            {
                MenuItem groupItem = sequenceMenu.addItem(sequenceGroup.Name, MenuItemType.Popup);
                groupItem.StaticImage.setItemResource("SequenceToolstrip/Sequence");
                groupItem.StaticImage.setItemGroup("Icons");
                groupItem.StaticImage.setItemName("Icon");
                //groupItem.Image = Resources.SequenceIconLarge;
                MenuCtrl groupItemChild = groupItem.createItemChild();
                foreach (MovementSequenceInfo sequenceInfo in sequenceGroup.Sequences)
                {
                    MenuItem sequenceItem = groupItemChild.addItem(sequenceInfo.Name, MenuItemType.Normal);
                    sequenceItem.MouseButtonClick += sequenceItem_Click;
                    sequenceItem.UserObject = sequenceInfo.FileName;
                    sequenceItem.StaticImage.setItemResource("SequenceToolstrip/Sequence");
                    sequenceItem.StaticImage.setItemGroup("Icons");
                    sequenceItem.StaticImage.setItemName("Icon");
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
    }
}
