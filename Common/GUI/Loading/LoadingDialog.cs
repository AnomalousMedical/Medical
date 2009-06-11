using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Medical.GUI
{
    partial class LoadingDialog : Form
    {
        private double opacityIncrement = 0.1;
        private double opacityDecrement = 0.2;
        private const int TIMER_INTERVAL = 50;
        private int currentStep = 0;

        public LoadingDialog(Bitmap backgroundImage)
        {
            InitializeComponent();
            if (backgroundImage != null)
            {
                this.picturePanel.BackgroundImage = backgroundImage;
                Size size = backgroundImage.Size;
                size.Height += progressBar.Size.Height;
                this.Size = size;
            }
            this.Opacity = 0.0;
            opacityTimer.Interval = TIMER_INTERVAL;
            opacityTimer.Tick += new EventHandler(opacityTimer_Tick);
            this.Shown += new EventHandler(LoadingDialog_Shown);
        }

        public void stepProgress()
        {
            currentStep += progressBar.Step;
        }

        public void fadeAway()
        {
            opacityIncrement = -opacityDecrement;
        }

        void LoadingDialog_Shown(object sender, EventArgs e)
        {
            opacityTimer.Start();
        }

        void opacityTimer_Tick(object sender, EventArgs e)
        {
            if (opacityIncrement > 0)
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += opacityIncrement;
                }
            }
            else
            {
                if (this.Opacity > 0)
                {
                    this.Opacity += opacityIncrement;
                }
                else
                {
                    opacityTimer.Stop();
                    this.Close();
                }
            }
            if (progressBar.Value < currentStep)
            {
                progressBar.PerformStep();
            }
        }

        public int ProgressMaximum
        {
            get
            {
                return progressBar.Maximum;
            }
            set
            {
                progressBar.Maximum = value;
            }
        }
        public int ProgressMinimum
        {
            get
            {
                return progressBar.Minimum;
            }
            set
            {
                progressBar.Minimum = value;
            }
        }
        public int ProgressStep
        {
            get
            {
                return progressBar.Step;
            }
            set
            {
                progressBar.Step = value;
            }
        }
    }
}
