using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class LeftCondylarGrowthGUI : BoneManipulatorGUI<LeftCondylarGrowthView>
    {
        private BoneManipulatorSlider leftRamusHeightSlider;
        private BoneManipulatorSlider leftCondyleHeightSlider;
        private BoneManipulatorSlider leftCondyleRotationSlider;
        private BoneManipulatorSlider leftMandibularNotchSlider;
        private BoneManipulatorSlider leftAntegonialNotchSlider;

        private Button undoButton;
        private Button makeNormalButton;

        private GridPropertiesControl gridPropertiesControl;

        public LeftCondylarGrowthGUI(LeftCondylarGrowthView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Mandible.LeftCondylarGrowthGUI.layout", view, context, viewHost)
        {
            leftRamusHeightSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/RamusHeightSlider") as ScrollBar);
            leftCondyleHeightSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/CondyleHeightSlider") as ScrollBar);
            leftCondyleRotationSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/CondyleRotationSlider") as ScrollBar);
            leftMandibularNotchSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/MandibularNotchSlider") as ScrollBar);
            leftAntegonialNotchSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleGrowth/AntegonialNotchSlider") as ScrollBar);

            addBoneManipulator(leftRamusHeightSlider);
            addBoneManipulator(leftCondyleHeightSlider);
            addBoneManipulator(leftCondyleRotationSlider);
            addBoneManipulator(leftMandibularNotchSlider);
            addBoneManipulator(leftAntegonialNotchSlider);

            undoButton = widget.findWidget("LeftCondyleGrowth/UndoButton") as Button;
            makeNormalButton = widget.findWidget("LeftCondyleGrowth/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);

            gridPropertiesControl = new GridPropertiesControl(context.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 5;
        }

        public override void opening()
        {
            gridPropertiesControl.updateGrid();
            base.opening();
        }

        public override void closing()
        {
            context.MeasurementGrid.Visible = false;
            base.closing();
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
