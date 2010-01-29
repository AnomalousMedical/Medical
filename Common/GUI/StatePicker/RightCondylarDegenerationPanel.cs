using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class RightCondylarDegenerationPanel : BoneManipulatorPanel
    {
        private bool open = false;
        private bool showingWear = false;

        public RightCondylarDegenerationPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Right Condyle Degeneration";

            wearSlider.ValueChanged += new EventHandler(wearSlider_ValueChanged);
            rightCondyleDegenerationSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            rightLateralPoleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
            rightMedialPoleScaleSlider.ValueChanged += new EventHandler(otherSliders_ValueChanged);
        }

        void wearSlider_ValueChanged(object sender, EventArgs e)
        {
            if (open && !showingWear)
            {
                this.setNavigationState("Right TMJ");
                showingWear = true;
            }
        }

        void otherSliders_ValueChanged(object sender, EventArgs e)
        {
            if (open && showingWear)
            {
                this.setNavigationState(NavigationState);
                showingWear = false;
            }
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            setToDefault();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            resetToOpeningState();
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            open = true;
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            open = false;
        }
    }
}
