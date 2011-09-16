using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class LeftCondylarGrowthGUI : BoneManipulatorGUI
    {
        private BoneManipulatorSlider leftRamusHeightSlider;
        private BoneManipulatorSlider leftCondyleHeightSlider;
        private BoneManipulatorSlider leftCondyleRotationSlider;
        private BoneManipulatorSlider leftMandibularNotchSlider;
        private BoneManipulatorSlider leftAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        private GridPropertiesControl gridPropertiesControl;

        public LeftCondylarGrowthGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Mandible.LeftCondylarGrowthGUI.layout", wizard)
        {
            leftRamusHeightSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/RamusHeightSlider") as VScroll);
            leftCondyleHeightSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/CondyleHeightSlider") as VScroll);
            leftCondyleRotationSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/CondyleRotationSlider") as VScroll);
            leftMandibularNotchSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/MandibularNotchSlider") as VScroll);
            leftAntegonialNotchSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/AntegonialNotchSlider") as VScroll);

            addBoneManipulator(leftRamusHeightSlider);
            addBoneManipulator(leftCondyleHeightSlider);
            addBoneManipulator(leftCondyleRotationSlider);
            addBoneManipulator(leftMandibularNotchSlider);
            addBoneManipulator(leftAntegonialNotchSlider);

            undoButton = widget.findWidget("LeftCondyleGrowth/UndoButton") as Button;
            makeNormalButton = widget.findWidget("LeftCondyleGrowth/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(wizard.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 5;
        }

        public override void opening(MedicalController medicalController, SimulationScene simScene)
        {
            base.opening(medicalController, simScene);
            gridPropertiesControl.updateGrid();
        }

        protected override void closing()
        {
            base.closing();
            timelineWizard.MeasurementGrid.Visible = false;
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to reset the condylar growth to normal?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doMakeNormalButtonClick);
        }

        private void doMakeNormalButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                setToDefault();
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            MessageBox.show("Are you sure you want to undo the condylar growth to before the wizard was opened?", "Confirm", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, doUndoButtonClick);
        }

        private void doUndoButtonClick(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                resetToOpeningState();
            }
        }
    }
}
