using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Navigator;
using System.Drawing;
using ComponentFactory.Krypton.Docking;
using System.Windows.Forms;

namespace Medical.GUI
{
    public class KryptonPictureWindow
    {
        private PictureWindow pictureWindow = new PictureWindow();
        private KryptonPage page = new KryptonPage();

        public KryptonPictureWindow()
        {
            pictureWindow.Dock = DockStyle.Fill;
            page.Controls.Add(pictureWindow);
            pictureWindow.TitleTextChanged += new EventHandler(pictureWindow_TitleTextChanged);
        }

        public void initialize(Bitmap bitmap)
        {
            pictureWindow.initialize(bitmap);
        }

        public void show(KryptonDockingManager dockingManager)
        {
            dockingManager.AddFloatingWindow("Floating", new KryptonPage[] { page });
        }

        public String Text
        {
            get
            {
                return pictureWindow.Text;
            }
            set
            {
                pictureWindow.Text = value;
                page.Text = value;
                page.TextTitle = value;
            }
        }

        void pictureWindow_TitleTextChanged(object sender, EventArgs e)
        {
            page.Text = pictureWindow.Text;
            page.TextTitle = pictureWindow.Text;
        }
    }
}
