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
    /// <summary>
    /// A panel for the doppler in a state wizard.
    /// </summary>
    public partial class DopplerPanel : StatePickerPanel
    {
        public DopplerPanel()
        {
            InitializeComponent();
            dopplerControl1.CurrentStageChanged += new EventHandler(dopplerControl1_CurrentStageChanged);
        }

        void dopplerControl1_CurrentStageChanged(object sender, EventArgs e)
        {
            
        }

        public override void setToDefault()
        {
            dopplerControl1.setToDefault();
        }
    }
}
