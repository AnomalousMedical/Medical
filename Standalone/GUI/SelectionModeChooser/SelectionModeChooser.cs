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
        private ImageBox lockedFeatureImage;

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
            if(lockedFeatureImage != null)
            {
                Gui.Instance.destroyWidget(lockedFeatureImage);
            }
            base.Dispose();
        }

        void pickingModeGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (allowSelectionModeChanges)
            {
                AnatomyPickingMode newMode = (AnatomyPickingMode)pickingModeGroup.SelectedButton.UserObject;
                if (!anatomyController.ShowPremiumAnatomy && newMode == AnatomyPickingMode.Individual)
                {
                    showNagMessage();
                    allowSelectionModeChanges = false;
                    pickingModeGroup.SelectedButton = pickingModeGroup.findButtonWithUserData(AnatomyPickingMode.Group);
                    anatomyController.PickingMode = AnatomyPickingMode.Group;
                    allowSelectionModeChanges = true;
                }
                else
                {
                    anatomyController.PickingMode = newMode;
                    hide();
                }
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
            if(anatomyController.ShowPremiumAnatomy)
            {
                if (lockedFeatureImage != null)
                {
                    Gui.Instance.destroyWidget(lockedFeatureImage);
                    lockedFeatureImage = null;
                }
            }
            else
            {
                if(lockedFeatureImage == null)
                {
                    int lockSize = individualButton.Height / 3;
                    lockedFeatureImage = (ImageBox)widget.createWidgetT("ImageBox", "ImageBox", individualButton.Left, individualButton.Top, lockSize, lockSize, Align.Left | Align.Top, "LockedFeatureImage");
                    lockedFeatureImage.NeedMouseFocus = false;
                    lockedFeatureImage.setItemResource("LockedFeature");
                }
            }
        }

        private static void showNagMessage()
        {
            MessageBox.show("Placeholder for nag message", "Placeholder", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
        }
    }
}
