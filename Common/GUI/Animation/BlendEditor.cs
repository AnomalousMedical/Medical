using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class BlendEditor : DockContent
    {
        private MedicalController controller;

        public BlendEditor()
        {
            InitializeComponent();
            
        }

        public void initialize(MedicalController controller)
        {
            this.controller = controller;
        }

        public void sceneLoaded()
        {
        }

        public void sceneUnloading()
        {
            keyFrameTrackBar.clearKeyFrames();
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            controller.createMedicalPlaybackState(keyFrameTrackBar.CurrentTickPosition);
            keyFrameTrackBar.addKeyFrame(keyFrameTrackBar.CurrentTickPosition);
        }
    }
}
