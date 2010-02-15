using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using Medical.Properties;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public partial class BottomTeethRemovalPanel : StatePickerPanel
    {
        private Dictionary<KryptonCheckButton, bool> openCheckStatus = new Dictionary<KryptonCheckButton, bool>();
        private bool allowUpdates = true;

        public BottomTeethRemovalPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                    openCheckStatus.Add(checkBox, false);
                }
            }
            this.Text = "Remove Bottom Teeth";
        }

        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                showChanges(true);
            }
        }

        public override void applyToState(MedicalState state)
        {
            TeethState teethState = state.Teeth;
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    teethState.addPosition(new ToothState(checkBox.Tag.ToString(), checkBox.Checked, Vector3.Zero, Quaternion.Identity));
                }
            }
        }

        public override void setToDefault()
        {
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    checkBox.Checked = false;
                }
            }
        }

        protected override void onPanelOpening()
        {
            allowUpdates = false;
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    checkBox.Checked = TeethController.getTooth(checkBox.Tag.ToString()).Extracted;
                }
            }
            allowUpdates = true;
        }

        public override void recordOpeningState()
        {
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    openCheckStatus[checkBox] = TeethController.getTooth(checkBox.Tag.ToString()).Extracted;
                }
            }
        }

        public override void resetToOpeningState()
        {
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    checkBox.Checked = openCheckStatus[checkBox];
                }
            }
        }
    }
}
