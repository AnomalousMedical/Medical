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
        private Dictionary<MovementSequenceGroup, MenuCtrl> groupMenuCtrls = new Dictionary<MovementSequenceGroup, MenuCtrl>();

        public SequenceMenu(MovementSequenceController sequenceController)
        {
            this.sequenceController = sequenceController;

            sequenceController.GroupAdded += new MovementSequenceGroupEvent(sequenceController_GroupAdded);
            sequenceController.SequenceAdded += new MovementSequenceInfoEvent(sequenceController_SequenceAdded);

            sequenceMenu = Gui.Instance.createWidgetT("PopupMenu", "LargeIconPopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "SequencesMenu") as PopupMenu;
            sequenceMenu.Visible = false;
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
            if (sequenceMenu != null)
            {
                sequenceMenu.setVisibleSmooth(true);
                LayerManager.Instance.upLayerItem(sequenceMenu);
                sequenceMenu.setPosition(left, top);
            }
        }

        public void hide()
        {
            sequenceMenu.setVisibleSmooth(false);
        }

        void sequenceItem_Click(Widget sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                MovementSequence sequence = sequenceController.loadSequence((MovementSequenceInfo)item.UserObject);
                sequenceController.stopPlayback();
                sequenceController.CurrentSequence = sequence;
                sequenceController.playCurrentSequence();
            }
            sequenceMenu.setVisibleSmooth(false);
        }

        void sequenceController_SequenceAdded(MovementSequenceController controller, MovementSequenceGroup group, MovementSequenceInfo sequenceInfo)
        {
            MenuCtrl groupItemChild;
            groupMenuCtrls.TryGetValue(group, out groupItemChild);
            //Double check that we have the group.
            if (groupItemChild == null)
            {
                sequenceController_GroupAdded(controller, group);
                groupItemChild = groupMenuCtrls[group];
            }

            MenuItem sequenceItem = groupItemChild.addItem(sequenceInfo.Name, MenuItemType.Normal);
            sequenceItem.MouseButtonClick += sequenceItem_Click;
            sequenceItem.UserObject = sequenceInfo;
            sequenceItem.StaticImage.setItemResource("SequenceToolstrip/Sequence");
            sequenceItem.StaticImage.setItemGroup("Icons");
            sequenceItem.StaticImage.setItemName("Icon");
        }

        void sequenceController_GroupAdded(MovementSequenceController controller, MovementSequenceGroup group)
        {
            MenuItem groupItem = sequenceMenu.addItem(group.Name, MenuItemType.Popup);
            groupItem.StaticImage.setItemResource("SequenceToolstrip/Sequence");
            groupItem.StaticImage.setItemGroup("Icons");
            groupItem.StaticImage.setItemName("Icon");
            MenuCtrl groupItemChild = groupItem.createItemChild();
            groupMenuCtrls.Add(group, groupItemChild);
        }
    }
}
