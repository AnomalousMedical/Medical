using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class RightCondylarGrowthPanel : BoneManipulatorPanel
    {
        private BoneManipulatorSlider rightRamusHeightSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        GridPropertiesControl gridPropertiesControl;

        public RightCondylarGrowthPanel(StateWizardPanelController controller)
            : base("Medical.GUI.StateWizard.Panels.Mandible.RightCondylarGrowthPanel.layout", controller)
        {

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

            gridPropertiesControl = new GridPropertiesControl(controller.MeasurementGrid, mainWidget);
            gridPropertiesControl.GridSpacing = 5;
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

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            gridPropertiesControl.updateGrid();
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            controller.MeasurementGrid.Visible = false;
        }
    }
}
