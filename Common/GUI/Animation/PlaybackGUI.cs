using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Medical.GUI;

namespace Medical.GUI
{
    public partial class PlaybackGUI : DockContent
    {
        private MedicalController controller;
        private float startTime;
        private bool allowUpdate = true;
        private bool dispatchingUpdate = false;

        public PlaybackGUI()
        {
            InitializeComponent();
            playbackTrackBar1.CurrentTimeChanged += new CurrentTimeChanged(playbackTrackBar1_CurrentTimeChanged);
        }

        public void initialize(MedicalController controller)
        {
            this.controller = controller;
            controller.MedicalPlayback.PlaybackTimeChanged += new PlaybackTimeChanged(MedicalPlayback_PlaybackTimeChanged);
        }

        void MedicalPlayback_PlaybackTimeChanged(float time)
        {
            if (!dispatchingUpdate)
            {
                allowUpdate = false;
                playbackTrackBar1.CurrentTime = time;
                allowUpdate = true;
            }
        }

        public void sceneLoaded()
        {
            playbackTrackBar1.CurrentTime = 0.0;
        }

        public void sceneUnloading()
        {

        }

        private void playButton_Click(object sender, EventArgs e)
        {
            controller.MedicalPlayback.startPlayback();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            controller.MedicalPlayback.stopPlayback();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            controller.MedicalPlayback.pausePlayback();
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            controller.createMedicalPlaybackState((float)playbackTrackBar1.CurrentTime);
        }

        void playbackTrackBar1_CurrentTimeChanged(PlaybackTrackBar trackBar, double currentTime)
        {
            int minute = (int)(currentTime / 60.0);
            int second = (int)(currentTime % 60.0);
            String minString = minute.ToString();
            if (minute < 10)
            {
                minString = "0" + minString;
            }
            String secString = second.ToString();
            if (second < 10)
            {
                secString = "0" + secString;
            }
            timeLabel.Text = String.Format("{0}:{1}", minString, secString);
            if (allowUpdate)
            {
                dispatchingUpdate = true;
                controller.MedicalPlayback.setTime((float)currentTime);
                dispatchingUpdate = false;
            }
        }
    }
}
