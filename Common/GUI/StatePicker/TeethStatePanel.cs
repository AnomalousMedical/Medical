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
    public partial class TeethStatePanel : StatePickerPanel
    {
        public TeethStatePanel()
        {
            InitializeComponent();
            this.Text = "Teeth";
        }

        public override void applyToState(MedicalState state)
        {
            TeethState teethState = state.Teeth;
            foreach (CheckBox checkBox in this.Controls)
            {
                teethState.addPosition(checkBox.Tag.ToString(), new ToothState(checkBox.Checked));
            }
        }

        public override void setToDefault()
        {

        }
    }
}
