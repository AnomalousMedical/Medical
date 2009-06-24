using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.StateWizard
{
    public partial class TeethPanel : StateWizardPanel
    {
        public TeethPanel()
        {
            InitializeComponent();
        }

        public override void applyToState(MedicalState state)
        {
            TeethState teethState = new TeethState();
            foreach (CheckBox checkBox in this.Controls)
            {
                teethState.addPosition(checkBox.Tag.ToString(), new ToothState(checkBox.Checked));
            }
            state.Teeth = teethState;
        }

        public override void setToDefault()
        {
            
        }
    }
}
