using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RightCondylarGrowthPanel : BoneManipulatorPanel
    {
        private BoneManipulatorSlider rightRamusHeightSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        public RightCondylarGrowthPanel(String panelFile, StateWizardPanelController controller)
            : base(panelFile, controller)
        {
            //gridPropertiesControl1.setGrid(panelController.MeasurementGrid);

            rightRamusHeightSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleGrowth/RamusHeightSlider") as VScroll);
            rightCondyleHeightSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleGrowth/CondyleHeightSlider") as VScroll);
            rightCondyleRotationSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleGrowth/CondyleRotationSlider") as VScroll);
            rightMandibularNotchSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleGrowth/MandibularNotchSlider") as VScroll);
            rightAntegonialNotchSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleGrowth/AntegonialNotchSlider") as VScroll);

            addBoneManipulator(rightRamusHeightSlider);
            addBoneManipulator(rightCondyleHeightSlider);
            addBoneManipulator(rightCondyleRotationSlider);
            addBoneManipulator(rightMandibularNotchSlider);
            addBoneManipulator(rightAntegonialNotchSlider);

            undoButton = mainWidget.findWidget("RightCondyleGrowth/UndoButton") as Button;
            makeNormalButton = mainWidget.findWidget("RightCondyleGrowth/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to reset the condylar growth to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                setToDefault();
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to undo the condylar growth to before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                resetToOpeningState();
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            //gridPropertiesControl1.updateGrid();
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            //panelController.MeasurementGrid.Visible = false;
        }
    }
}
