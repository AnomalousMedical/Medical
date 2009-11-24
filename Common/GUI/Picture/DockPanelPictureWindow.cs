using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class DockPanelPictureWindow : DockContent
    {
        public DockPanelPictureWindow()
        {
            InitializeComponent();
        }

        public void initialize(Bitmap bitmap)
        {
            pictureWindow.initialize(bitmap);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //Prevent the main window from going into the background.
            Form topLevel = DockPanel.TopLevelControl as Form;
            if (topLevel != null)
            {
                topLevel.Activate();
            }
        }
    }
}
