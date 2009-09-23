using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Medical.GUI
{
    public class ProgressDialog
    {
        LoadingDialog currentDialog;
        Bitmap backgroundImage;

        public ProgressDialog(Bitmap backgroundImage)
        {
            this.backgroundImage = backgroundImage;
            this.ProgressMaximum = 100;
            this.ProgressMinimum = 0;
        }

        public void fadeIn()
        {
            currentDialog = new LoadingDialog(backgroundImage);
            currentDialog.ProgressMaximum = ProgressMaximum;
            currentDialog.ProgressMinimum = ProgressMinimum;
            currentDialog.TopMost = TopMost;
            Thread loadThread = new Thread(doFadeIn);
            loadThread.Name = "Load Dialog Thread";
            loadThread.Start();
        }

        void doFadeIn()
        {
            //currentDialog.Show();
            //while (currentDialog.Visible)
            //{
            //    Application.DoEvents();
            //    Thread.Sleep(10);
            //}
            currentDialog.ShowDialog();
            System.Console.WriteLine("Load Dialog thread closed");
        }

        public void fadeAway()
        {
            currentDialog.fadeAway();
        }

        public void stepProgress(int amount)
        {
            currentDialog.stepProgress(amount);
        }

        public int ProgressMaximum { get; set; }
        public int ProgressMinimum { get; set; }
        public bool TopMost { get; set; }
    }
}
