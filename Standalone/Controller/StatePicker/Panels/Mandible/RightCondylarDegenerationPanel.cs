﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RightCondylarDegenerationPanel : BoneManipulatorPanel
    {
        private bool allowCameraChange = false;
        private bool showingWear = false;

        private BoneManipulatorSlider wearSlider;
        private BoneManipulatorSlider rightCondyleDegenerationSlider;
        private BoneManipulatorSlider rightLateralPoleSlider;
        private BoneManipulatorSlider rightMedialPoleScaleSlider;

        private Button undoButton;
        private Button makeNormalButton;

        public RightCondylarDegenerationPanel(String panelFile, StateWizardPanelController controller)
            : base(panelFile, controller)
        {
            //gridPropertiesControl1.setGrid(panelController.MeasurementGrid);

            rightCondyleDegenerationSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleDegen/CondyleSlider") as VScroll);
            rightLateralPoleSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleDegen/LateralPoleSlider") as VScroll);
            rightMedialPoleScaleSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleDegen/MedialPoleSlider") as VScroll);
            wearSlider = new BoneManipulatorSlider(mainWidget.findWidget("RightCondyleDegen/WearSlider") as VScroll);

            addBoneManipulator(rightCondyleDegenerationSlider);
            addBoneManipulator(rightLateralPoleSlider);
            addBoneManipulator(rightMedialPoleScaleSlider);
            addBoneManipulator(wearSlider);

            wearSlider.ValueChanged += new EventHandler(wearSlider_ValueChanged);
            rightCondyleDegenerationSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            rightLateralPoleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            rightMedialPoleScaleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);

            undoButton = mainWidget.findWidget("RightCondyleDegen/UndoButton") as Button;
            makeNormalButton = mainWidget.findWidget("RightCondyleDegen/MakeNormalButton") as Button;

            undoButton.MouseButtonClick += new MyGUIEvent(undoButton_MouseButtonClick);
            makeNormalButton.MouseButtonClick += new MyGUIEvent(makeNormalButton_MouseButtonClick);
        }

        void wearSlider_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && !showingWear)
            {
                this.setNavigationState("WizardRightTMJ");
                showingWear = true;
            }
        }

        void otherSliders_ValueChanged(object sender, EventArgs e)
        {
            if (allowCameraChange && showingWear)
            {
                this.setNavigationState(NavigationState);
                showingWear = false;
            }
        }

        void makeNormalButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to reset the condylar degeneration to normal?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                allowCameraChange = false;
                setToDefault();
                allowCameraChange = true;
            }
        }

        void undoButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //if (MessageBox.Show(this, "Are you sure you want to undo the condylar degeneration to before the wizard was opened?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                allowCameraChange = false;
                resetToOpeningState();
                allowCameraChange = true;
            }
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            allowCameraChange = true;
            //gridPropertiesControl1.updateGrid();
            showingWear = false;
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            allowCameraChange = false;
            //panelController.MeasurementGrid.Visible = false;
        }
    }
}
