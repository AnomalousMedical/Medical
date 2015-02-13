using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Platform;
using Anomalous.GuiFramework.Cameras;

namespace Medical.GUI
{
    class CameraMovementModeChooser : PopupContainer
    {
        private SceneViewController sceneViewController;
        private bool allowSelectionModeChanges = true;
        private ButtonGroup<CameraMovementMode> selectionOperators;

        public CameraMovementModeChooser(SceneViewController sceneViewController)
            : base("Medical.GUI.CameraMovementModeChooser.CameraMovementModeChooser.layout")
        {
            this.sceneViewController = sceneViewController;
            sceneViewController.CameraMovementModeChanged +=sceneViewController_CameraMovementModeChanged;

            selectionOperators = new ButtonGroup<CameraMovementMode>();
            setupSelectionButton(CameraMovementMode.Rotate, "RotateButton");
            setupSelectionButton(CameraMovementMode.Pan, "PanButton");
            setupSelectionButton(CameraMovementMode.Zoom, "ZoomButton");
            selectionOperators.Selection = sceneViewController.MovementMode;
            selectionOperators.SelectedButtonChanged += pickingModeGroup_SelectedButtonChanged;
        }

        public override void Dispose()
        {
            sceneViewController.CameraMovementModeChanged -= sceneViewController_CameraMovementModeChanged;
            base.Dispose();
        }

        void pickingModeGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            if (allowSelectionModeChanges)
            {
                allowSelectionModeChanges = false;
                sceneViewController.MovementMode = (CameraMovementMode)selectionOperators.Selection;
                hide();
                allowSelectionModeChanges = true;
            }
        }

        void sceneViewController_CameraMovementModeChanged(CameraMovementMode arg)
        {
            if (allowSelectionModeChanges)
            {
                allowSelectionModeChanges = false;
                selectionOperators.Selection = arg;
                allowSelectionModeChanges = true;
            }
        }

        private void setupSelectionButton(CameraMovementMode mode, String name)
        {
            Button selectionButton = (Button)widget.findWidget(name);
            selectionOperators.addButton(mode, selectionButton);
            selectionButton.NeedToolTip = true;
            selectionButton.EventToolTip += selectionButton_EventToolTip;
        }

        void selectionButton_EventToolTip(Widget source, EventArgs e)
        {
            String text;
            switch (selectionOperators[(Button)source])
            {
                case CameraMovementMode.Rotate:
                    text = "Select";
                    break;
                case CameraMovementMode.Pan:
                    text = String.Format("Pan ({0})", CameraInputController.PanKeyDescription);
                    break;
                case CameraMovementMode.Zoom:
                    text = String.Format("Zoom ({0})", CameraInputController.ZoomKeyDescription);
                    break;
                default:
                    text = "Unknown";
                    break;
            }
            TooltipManager.Instance.processTooltip(source, text, (ToolTipEventArgs)e);
        }
    }
}
