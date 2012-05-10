using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class RightCondylarGrowthGUI : BoneManipulatorGUI<RightCondylarGrowthView>
    {
        private BoneManipulatorSlider rightRamusHeightSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        GridPropertiesControl gridPropertiesControl;

        public RightCondylarGrowthGUI(RightCondylarGrowthView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Mandible.RightCondylarGrowthGUI.layout", view, context, viewHost)
        {

            rightRamusHeightSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/RamusHeightSlider") as ScrollBar);
            rightCondyleHeightSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/CondyleHeightSlider") as ScrollBar);
            rightCondyleRotationSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/CondyleRotationSlider") as ScrollBar);
            rightMandibularNotchSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/MandibularNotchSlider") as ScrollBar);
            rightAntegonialNotchSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleGrowth/AntegonialNotchSlider") as ScrollBar);

            addBoneManipulator(rightRamusHeightSlider);
            addBoneManipulator(rightCondyleHeightSlider);
            addBoneManipulator(rightCondyleRotationSlider);
            addBoneManipulator(rightMandibularNotchSlider);
            addBoneManipulator(rightAntegonialNotchSlider);

            undoButton = widget.findWidget("RightCondyleGrowth/UndoButton") as Button;
            makeNormalButton = widget.findWidget("RightCondyleGrowth/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(context.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 5;
        }

        public override void opening()
        {
            base.opening();
            gridPropertiesControl.updateGrid();
        }

        public override void closing()
        {
            base.closing();
            context.MeasurementGrid.Visible = false;
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
