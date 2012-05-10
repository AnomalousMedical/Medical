using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class LeftCondylarDegenerationGUI : BoneManipulatorGUI<LeftCondylarDegenerationView>
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

        public LeftCondylarDegenerationGUI(LeftCondylarDegenerationView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.DistortionWizard.Mandible.LeftCondylarDegenerationGUI.layout", view, context, viewHost)
        {
            leftCondyleDegenerationSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/CondyleSlider") as ScrollBar);
            leftLateralPoleSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/LateralPoleSlider") as ScrollBar);
            leftMedialPoleScaleSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/MedialPoleSlider") as ScrollBar);
            wearSlider = new BoneManipulatorSlider(widget.findWidget("LeftCondyleDegen/WearSlider") as ScrollBar);

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

            gridPropertiesControl = new GridPropertiesControl(context.MeasurementGrid, widget);
            gridPropertiesControl.GridSpacing = 5;
        }

        public override void opening()
        {
            base.opening();
            allowCameraChange = true;
            gridPropertiesControl.updateGrid();
        }

        public override void closing()
        {
            base.closing();
            context.MeasurementGrid.Visible = false;
        }

        void wearSlider_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && !showingWear)
            {
                if (wizardView.ShowOsteophyteAction != null)
                {
                    context.runAction(wizardView.ShowOsteophyteAction, ViewHost);
                }
                showingWear = true;
            }
        }

        void otherSliders_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && showingWear)
            {
                if (wizardView.NormalAction != null)
                {
                    context.runAction(wizardView.NormalAction, ViewHost);
                }
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
