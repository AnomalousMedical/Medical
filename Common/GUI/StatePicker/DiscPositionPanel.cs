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
    public partial class DiscPositionPanel : StatePickerPanel
    {
        public DiscPositionPanel()
        {
            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (clockFaceList != null && clockFaceList.Columns.Count > 0)
            {
                clockFaceList.Columns[0].Width = -2;
            }
        }
    }
}
