using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class RightCondylarGrowthGUI : BoneManipulatorGUI
    {
        private BoneManipulatorSlider rightRamusHeightSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        GridPropertiesControl gridPropertiesControl;

        public RightCondylarGrowthGUI(TimelineWizard wizard)
            : base("Medical.TimelineGUI.Panels.Mandible.RightCondylarGrowthGUI.layout", wizard)
        {

            rightRamusHeightSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/RamusHeightSlider") as VScroll);
            rightCondyleHeightSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/CondyleHeightSlider") as VScroll);
            rightCondyleRotationSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/CondyleRotationSlider") as VScroll);
            rightMandibularNotchSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/MandibularNotchSlider") as VScroll);
            rightAntegonialNotchSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/AntegonialNotchSlider") as VScroll);

            addBoneManipulator(rightRamusHeightSlider);
            addBoneManipulator(rightCondyleHeightSlider);
            addBoneManipulator(rightCondyleRotationSlider);
            addBoneManipulator(rightMandibularNotchSlider);
            addBoneManipulator(rightAntegonialNotchSlider);

            undoButton = widget.findWidget("RightCondyleGrowth/UndoButton") as Button;
            makeNormalButton = widget.findWidget("RightCondyleGrowth/MakeNormalButton") as Button;

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
