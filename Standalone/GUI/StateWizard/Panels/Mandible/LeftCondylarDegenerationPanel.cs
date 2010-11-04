using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class LeftCondylarDegenerationPanel : BoneManipulatorPanel
    {
        private bool allowCameraChange = false;
        private bool showingWear = false;

        private BoneManipulatorSlider wearSlider;
        private BoneManipulatorSlider leftCondyleDegenerationSlider;
        private BoneManipulatorSlider leftLateralPoleSlider;
        private BoneManipulatorSlider leftMedialPoleScaleSlider;

        private Button undoButton;
        private Button makeNormalButton;

        private GridPropertiesControl gridPropertiesControl;

        public LeftCondylarDegenerationPanel(StateWizardPanelController controller)
            : base("Medical.GUI.StateWizard.Panels.Mandible.LeftCondylarDegenerationPanel.layout", controller)
        {
            leftCondyleDegenerationSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleDegen/CondyleSlider") as VScroll);
            leftLateralPoleSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleDegen/LateralPoleSlider") as VScroll);
            leftMedialPoleScaleSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleDegen/MedialPoleSlider") as VScroll);
            wearSlider = new BoneManipulatorSlider(mainWidget.findWidget("LeftCondyleDegen/WearSlider") as VScroll);

            addBoneManipulator(leftCondyleDegenerationSlider);
            addBoneManipulator(leftLateralPoleSlider);
            addBoneManipulator(leftMedialPoleScaleSlider);
            addBoneManipulator(wearSlider);

            wearSlider.ValueChanged += new EventHandler(wearSlider_ValueChanged);
            leftCondyleDegenerationSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            leftLateralPoleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            leftMedialPoleScaleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);

            undoButton = mainWidget.findWidget("LeftCondyleDegen/UndoButton") as Button;
            makeNormalButton = mainWidget.findWidget("LeftCondyleDegen/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(controller.MeasurementGrid, mainWidget);
            gridPropertiesControl.GridSpacing = 5;
        }

        void wearSlider_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && !showingWear)
            {
                this.setNavigationState("WizardLeftTMJ");
                showingWear = true;
            }
        }

        void otherSliders_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && showingWear)
            {
                this.setNavigationState(NavigationState);
                showingWear = false;
            }
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the condylar degeneration to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                allowCameraChange = false;
                setToDefault();
                allowCameraChange = true;
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to undo the condylar degeneration to before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
        }

        private void doUndoButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                allowCameraChange = false;
                resetToOpeningState();
                allowCameraChange = true;
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            allowCameraChange = true;
            gridPropertiesControl.updateGrid();
            showingWear = false;
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            allowCameraChange = false;
            controller.MeasurementGrid.Visible = false;
        }
    }
}
