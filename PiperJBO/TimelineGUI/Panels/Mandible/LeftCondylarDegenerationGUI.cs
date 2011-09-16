using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class LeftCondylarDegenerationGUI : BoneManipulatorGUI
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

        public LeftCondylarDegenerationGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Mandible.LeftCondylarDegenerationGUI.layout", wizard)
        {
            leftCondyleDegenerationSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/CondyleSlider") as VScroll);
            leftLateralPoleSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/LateralPoleSlider") as VScroll);
            leftMedialPoleScaleSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/MedialPoleSlider") as VScroll);
            wearSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/WearSlider") as VScroll);

            addBoneManipulator(leftCondyleDegenerationSlider);
            addBoneManipulator(leftLateralPoleSlider);
            addBoneManipulator(leftMedialPoleScaleSlider);
            addBoneManipulator(wearSlider);

            wearSlider.ValueChanged += new EventHandler(wearSlider_ValueChanged);
            leftCondyleDegenerationSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            leftLateralPoleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            leftMedialPoleScaleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);

            undoButton = widget.findWidget("LeftCondyleDegen/UndoButton") as Button;
            makeNormalButton = widget.findWidget("LeftCondyleDegen/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(wizard.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 5;
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            base.opening(medicalController, simScene);
            allowCameraChange = true;
            gridPropertiesControl.updateGrid();
        }

        protected override void closing()
        {
            base.closing();
            timelineWizard.MeasurementGrid.Visible = false;
        }

        void wearSlider_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && !showingWear)
            {
                CondylarDegenerationGUIData guiData = (CondylarDegenerationGUIData)PanelData;
                applyCameraPosition(guiData.ShowOsteophyteCamera);
                showingWear = true;
            }
        }

        void otherSliders_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && showingWear)
            {
                CondylarDegenerationGUIData guiData = (CondylarDegenerationGUIData)PanelData;
                applyCameraPosition(guiData.NormalCamera);
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
    }
}
