using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Platform;

namespace Medical.GUI
{
    class SelectionModeChooser : PopupContainer
    {
        private static readonly int lockSize = ScaleHelper.Scaled(18);

        private static ButtonEvent ChangeSelectionMode;

        static SelectionModeChooser()
        {
            ChangeSelectionMode = new ButtonEvent(EventLayers.Gui);
            ChangeSelectionMode.addButton(KeyboardButtonCode.KC_TAB);
            DefaultEvents.registerDefaultEvent(ChangeSelectionMode);
        }

        private AnatomyController anatomyController;
        private bool allowSelectionModeChanges = true;
        private ButtonGroup pickingModeGroup;
        private Button individualButton;
        private Button noneButton;
        private ImageBox lockedFeatureImage;

        public event Action ShowBuyMessage;

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

            ChangeSelectionMode.FirstFrameUpEvent += changeSelectionMode_FirstFrameUpEvent;

            toggleIndividualSelectionVisible();
        }

        public override void Dispose()
        {
            if(lockedFeatureImage != null)
            {
                Gui.Instance.destroyWidget(lockedFeatureImage);
            }
            ChangeSelectionMode.FirstFrameUpEvent -= changeSelectionMode_FirstFrameUpEvent;
            base.Dispose();
        }

        void pickingModeGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (allowSelectionModeChanges)
            {
                AnatomyPickingMode newMode = (AnatomyPickingMode)pickingModeGroup.SelectedButton.UserObject;
                if (!anatomyController.ShowPremiumAnatomy && newMode == AnatomyPickingMode.Individual)
                {
                    showBuyMessage();
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
                    lockedFeatureImage = (ImageBox)widget.createWidgetT("ImageBox", "ImageBox", individualButton.Left, individualButton.Top, lockSize, lockSize, Align.Left | Align.Top, "LockedFeatureImage");
                    lockedFeatureImage.NeedMouseFocus = false;
                    lockedFeatureImage.setItemResource("LockedFeature");
                }
            }
        }

        private void showBuyMessage()
        {
            if(ShowBuyMessage != null)
            {
                ShowBuyMessage.Invoke();
            }
        }

        void changeSelectionMode_FirstFrameUpEvent(EventLayer eventLayer)
        {
            if (!Gui.Instance.HandledKeyboardButtons)
            {
                switch (anatomyController.PickingMode)
                {
                    case AnatomyPickingMode.Group:
                        if (anatomyController.ShowPremiumAnatomy)
                        {
                            anatomyController.PickingMode = AnatomyPickingMode.Individual;
                        }
                        else
                        {
                            anatomyController.PickingMode = AnatomyPickingMode.None;
                        }
                        break;
                    case AnatomyPickingMode.Individual:
                        anatomyController.PickingMode = AnatomyPickingMode.None;
                        break;
                    case AnatomyPickingMode.None:
                        anatomyController.PickingMode = AnatomyPickingMode.Group;
                        break;
                }
            }
        }
    }
}
