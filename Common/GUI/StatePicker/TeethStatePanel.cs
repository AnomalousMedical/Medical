using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;

namespace Medical.GUI
{
    public partial class TeethStatePanel : StatePickerPanel
    {
        public TeethStatePanel()
        {
            InitializeComponent();
            this.Text = "Teeth";
            foreach (Control control in this.Controls)
            {
                CheckBox checkBox = control as CheckBox;
                if (checkBox != null)
                {
                    checkBox.CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                }
            }
        }

        void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            showChanges();
        }

        public override void applyToState(MedicalState state)
        {
            TeethState teethState = state.Teeth;
            foreach (Control control in this.Controls)
            {
                CheckBox checkBox = control as CheckBox;
                if (checkBox != null)
                {
                    teethState.addPosition(new ToothState(checkBox.Tag.ToString(), checkBox.Checked, Vector3.Zero, Quaternion.Identity));
                }
            }
        }

        public override void setToDefault()
        {

        }
    }
}
