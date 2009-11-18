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
    public partial class LeftCondylarDegenrationPanel : BoneManipulatorPanel
    {
        public LeftCondylarDegenrationPanel()
        {
            InitializeComponent();
            this.Text = "Left Condyle Degeneration";
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
