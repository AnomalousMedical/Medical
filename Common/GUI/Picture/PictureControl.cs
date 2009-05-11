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
    public partial class PictureControl : UserControl
    {
        public PictureControl()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowser.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                directoryText.Text = folderBrowser.SelectedPath;
            }
        }
    }
}
