using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class SelectionModeChooser : PopupContainer
    {
        private AnatomyController anatomyController;
        private bool allowSelectionModeChanges = true;
        private ButtonGroup pickingModeGroup;

        public SelectionModeChooser(AnatomyController anatomyController)
            :base("Medical.GUI.SelectionModeChooser.SelectionModeChooser.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.PickingModeChanged += new Engine.EventDelegate<AnatomyController, AnatomyPickingMode>(anatomyController_PickingModeChanged);

            pickingModeGroup = new ButtonGroup();
            Button groupButton = (Button)widget.findWidget("GroupButton");
            groupButton.UserObject = AnatomyPickingMode.Group;
            pickingModeGroup.addButton(groupButton);
            Button individualButton = (Button)widget.findWidget("IndividualButton");
            individualButton.UserObject = AnatomyPickingMode.Individual;
            pickingModeGroup.addButton(individualButton);
            Button noneButton = (Button)widget.findWidget("NoneButton");
            noneButton.UserObject = AnatomyPickingMode.None;
            pickingModeGroup.addButton(noneButton);
            pickingModeGroup.SelectedButton = pickingModeGroup.findButtonWithUserData(anatomyController.PickingMode);
            pickingModeGroup.SelectedButtonChanged += new EventHandler(pickingModeGroup_SelectedButtonChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        void pickingModeGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (allowSelectionModeChanges)
            {
                anatomyController.PickingMode = (AnatomyPickingMode)pickingModeGroup.SelectedButton.UserObject;
                hide();
            }
        }

        void anatomyController_PickingModeChanged(AnatomyController source, AnatomyPickingMode arg)
        {
            allowSelectionModeChanges = false;
            pickingModeGroup.SelectedButton = pickingModeGroup.findButtonWithUserData(anatomyController.PickingMode);
            allowSelectionModeChanges = true;
        }
    }
}
