using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI.Layers
{
    public partial class LayerEntry : UserControl
    {
        private TransparencyInterface transparencyInterface;

        public LayerEntry()
        {
            InitializeComponent();
            this.transparency.ValueChanged += new EventHandler(transparency_ValueChanged);
        }

        public void initialize(TransparencyInterface transparency)
        {
            this.transparencyInterface = transparency;
            this.entryCheckBox.Text = transparencyInterface.ObjectName;
            this.transparency.Value = (decimal)transparency.CurrentAlpha;
        }

        public void setAlpha(decimal alpha)
        {
            transparency.Value = alpha;
        }

        void transparency_ValueChanged(object sender, EventArgs e)
        {
            transparencyInterface.setAlpha((float)transparency.Value);
        }

        public bool Selected
        {
            get
            {
                return entryCheckBox.Checked;
            }
        }

    }
}
