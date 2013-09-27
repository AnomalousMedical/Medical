using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class SelectionModeChooser : PopupContainer
    {
        private AnatomyController anatomyController;
        private bool allowSelectionModeChanges = true;
        private ButtonGroup pickingModeGroup;
        private Button individualButton;
        private Button noneButton;

        public SelectionModeChooser(AnatomyController anatomyController)
            :base("Medical.GUI.SelectionModeChooser.SelectionModeChooser.layout")
        {
            this.anatomyController = anatomyController;
            anatomyController.PickingModeChanged += new Engine.EventDelegate<AnatomyController, AnatomyPickingMode>(anatomyController_PickingModeChanged);
            anatomyController.ShowPremiumAnatomyChanged += new Engine.EventDelegate<AnatomyController, bool>(anatomyController_ShowPremiumAnatomyChanged);

            pickingModeGroup = new ButtonGroup();
            Button groupButton = (Button)widget.findWidget("GroupButton");
            groupButton.UserObject = AnatomyPickingMode.Group;
            pickingModeGroup.addButton(groupButton);
            individualButton = (Button)widget.findWidget("IndividualButton");
            individualButton.UserObject = AnatomyPickingMode.Individual;
            pickingModeGroup.addButton(individualButton);
            noneButton = (Button)widget.findWidget("NoneButton");
            noneButton.UserObject = AnatomyPickingMode.None;
            pickingModeGroup.addButton(noneButton);
            pickingModeGroup.SelectedButton = pickingModeGroup.findButtonWithUserData(anatomyController.PickingMode);
            pickingModeGroup.SelectedButtonChanged += new EventHandler(pickingModeGroup_SelectedButtonChanged);

            toggleIndividualSelectionVisible();
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

        void anatomyController_ShowPremiumAnatomyChanged(AnatomyController source, bool arg)
        {
            toggleIndividualSelectionVisible();
        }

        private void toggleIndividualSelectionVisible()
        {
            if (anatomyController.ShowPremiumAnatomy)
            {
                individualButton.Visible = true;
                noneButton.setPosition(ScaleHelper.Scaled(8), ScaleHelper.Scaled(137));
                widget.setSize(widget.Width, ScaleHelper.Scaled(205));
            }
            else
            {
                individualButton.Visible = false;
                noneButton.setPosition(individualButton.Left, individualButton.Top);
                widget.setSize(widget.Width, ScaleHelper.Scaled(137));
            }
        }
    }
}
