using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Logging;

namespace Medical.GUI
{
    public partial class LeftCondylarGrowthPanel : BoneManipulatorPanel
    {
        public LeftCondylarGrowthPanel(StatePickerPanelController panelController)
            : base(panelController)
        {
            InitializeComponent();
            this.Text = "Left Condyle Growth";
        }

        private void makeNormalButton_Click(object sender, EventArgs e)
        {
            setToDefault();
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            resetToOpeningState();
        }
    }
}
