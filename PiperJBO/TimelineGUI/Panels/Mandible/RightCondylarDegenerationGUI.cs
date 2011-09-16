using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class RightCondylarDegenerationGUI : BoneManipulatorGUI
    {
        private bool allowCameraChange = false;
        private bool showingWear = false;

        private BoneManipulatorSlider wearSlider;
        private BoneManipulatorSlider rightCondyleDegenerationSlider;
        private BoneManipulatorSlider rightLateralPoleSlider;
        private BoneManipulatorSlider rightMedialPoleScaleSlider;

        private Button undoButton;
        private Button makeNormalButton;

        GridPropertiesControl gridPropertiesControl;

        public RightCondylarDegenerationGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Mandible.RightCondylarDegenerationGUI.layout", wizard)
        {
            rightCondyleDegenerationSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/CondyleSlider") as VScroll);
            rightLateralPoleSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/LateralPoleSlider") as VScroll);
            rightMedialPoleScaleSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/MedialPoleSlider") as VScroll);
            wearSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/WearSlider") as VScroll);

            addBoneManipulator(rightCondyleDegenerationSlider);
            addBoneManipulator(rightLateralPoleSlider);
            addBoneManipulator(rightMedialPoleScaleSlider);
            addBoneManipulator(wearSlider);

            wearSlider.ValueChanged += new EventHandler(wearSlider_ValueChanged);
            rightCondyleDegenerationSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            rightLateralPoleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            rightMedialPoleScaleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);

            undoButton = widget.findWidget("RightCondyleDegen/UndoButton") as Button;
            makeNormalButton = widget.findWidget("RightCondyleDegen/MakeNormalButton") as Button;

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
