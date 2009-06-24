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
    /// <summary>
    /// Base class for panel dialogs. Should be abstract, but not so the
    /// designer can still be used.
    /// </summary>
    public partial class StateWizardPanel : UserControl
    {
        public StateWizardPanel()
        {
            InitializeComponent();
        }

        public virtual void applyToState(MedicalState state)
        {

        }

        public virtual void setToDefault()
        {

        }
    }
}
