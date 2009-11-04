using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class FossaControl : GUIElement
    {
        Fossa leftFossa;
        Fossa rightFossa;
        bool allowUpdates = true;

        public FossaControl()
        {
            InitializeComponent();
            leftEminanceSlider.ValueChanged += new EventHandler(leftEminanceSlider_ValueChanged);
            rightEminanceSlider.ValueChanged += new EventHandler(rightEminanceSlider_ValueChanged);
        }

        protected override void sceneLoaded(SimScene scene)
        {
            leftFossa = FossaController.get("LeftFossa");
            rightFossa = FossaController.get("RightFossa");
            this.Enabled = leftFossa != null && rightFossa != null;
            if (this.Enabled)
            {
                allowUpdates = false;
                leftEminanceSlider.Value = (int)(leftFossa.getEminanceDistortion() * leftEminanceSlider.Maximum);
                rightEminanceSlider.Value = (int)(rightFossa.getEminanceDistortion() * rightEminanceSlider.Maximum);
                allowUpdates = true;
            }
        }

        protected override void sceneUnloading()
        {
            base.sceneUnloading();
            leftFossa = null;
            rightFossa = null;
        }

        void leftEminanceSlider_ValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                leftFossa.setEminanceDistortion((float)leftEminanceSlider.Value / leftEminanceSlider.Maximum);
            }
        }

        void rightEminanceSlider_ValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                rightFossa.setEminanceDistortion((float)rightEminanceSlider.Value / rightEminanceSlider.Maximum);
            }
        }
    }
}
