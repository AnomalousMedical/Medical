using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class RightCondylarDegenerationGUI : BoneManipulatorGUI<RightCondylarDegenerationView>
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

        public RightCondylarDegenerationGUI(RightCondylarDegenerationView wizardView, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.DistortionWizards.Mandible.RightCondylarDegenerationGUI.layout", wizardView, context, viewHost)
        {
            rightCondyleDegenerationSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/CondyleSlider") as ScrollBar);
            rightLateralPoleSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/LateralPoleSlider") as ScrollBar);
            rightMedialPoleScaleSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/MedialPoleSlider") as ScrollBar);
            wearSlider = new BoneManipulatorSlider(widget.findWidget("RightCondyleDegen/WearSlider") as ScrollBar);

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
                    context.runAction(wizardView.ShowOsteophyteAction);
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
                    context.runAction(wizardView.NormalAction);
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
