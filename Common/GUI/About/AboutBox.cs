using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class AboutBox : Form
    {
        public AboutBox(Bitmap backgroundImage)
        {
            InitializeComponent();
            if (backgroundImage != null)
            {
                this.picturePanel.BackgroundImage = backgroundImage;
                Size imageSize = backgroundImage.Size;
                picturePanel.Size = imageSize;

                Size aboutSize = imageSize;
                aboutSize.Height += contentPanel.Size.Height;
                this.Size = aboutSize;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
