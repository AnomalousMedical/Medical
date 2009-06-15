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
    public partial class AnimationGUI : DockContent
    {
        private MedicalController controller;
        private float startTime;

        public AnimationGUI()
        {
            InitializeComponent();
        }

        public void initialize(MedicalController controller)
        {
            this.controller = controller;
        }

        public void sceneLoaded()
        {
            startTime = 10.0f;
        }

        public void sceneUnloading()
        {

        }

        private void playButton_Click(object sender, EventArgs e)
        {
            controller.MedicalPlayback.startPlayback(0.0f);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            controller.MedicalPlayback.stopPlayback();
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            controller.createMedicalPlaybackState(startTime);
            startTime += 10.0f;
        }
    }
}
