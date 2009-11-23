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

        public BottomTeethRemovalPanel()
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

        public override void recordOpeningState()
        {
            foreach (Control control in this.Controls)
            {
                KryptonCheckButton checkBox = control as KryptonCheckButton;
                if (checkBox != null)
                {
                    openCheckStatus[checkBox] = checkBox.Checked;
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

        protected override void statePickerSet(StatePickerWizard controller)
        {
            parentPicker.ImageList.Images.Add(NavigationImageKey, Resources.BottomTeethRemove);
        }

        protected override void onPanelClosing()
        {
            base.onPanelClosing();
            TeethController.showBottomTeethTools(false);
        }

        protected override void onPanelOpening()
        {
            base.onPanelOpening();
            TeethController.showBottomTeethTools(true);
        }

        internal override String NavigationImageKey
        {
            get
            {
                return "__BottomToothRemovalKey";
            }
        }
    }
}
