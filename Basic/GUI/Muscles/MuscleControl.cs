﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Medical.Properties;
using Engine.ObjectManagement;
using Engine.Saving.XMLSaver;
using Medical.Muscles;
using Engine.Resources;
using System.Xml;
using Engine.Platform;

namespace Medical.GUI
{
    public partial class MuscleControl : GUIElement
    {
        private XmlSaver xmlSaver = new XmlSaver();
        private MovementSequence currentSequence;
        private float time;

        public MuscleControl()
        {
            InitializeComponent();
            muscleSequenceView.SequenceActivated += new MuscleSequenceActivated(muscleSequenceView_SequenceActivated);
            playbackTrackBar.TimeChanged += new TimeChanged(playbackTrackBar_TimeChanged);
        }

        public void stopPlayback()
        {
            time = 0.0f;
            playbackTrackBar.CurrentTime = 0.0f;
            unsubscribeFromUpdates();
        }

        void playbackTrackBar_TimeChanged(TimeTrackBar trackBar, double currentTime)
        {
            if (currentSequence != null)
            {
                currentSequence.setPosition((float)currentTime);
            }
        }

        void muscleSequenceView_SequenceActivated(string sequenceText, string sequenceFile)
        {
            using (Archive archive = FileSystem.OpenArchive(sequenceFile))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(archive.openStream(sequenceFile, FileMode.Open, FileAccess.Read)))
                {
                    currentSequence = xmlSaver.restoreObject(xmlReader) as MovementSequence;
                }
            }
            playbackPanel.Enabled = currentSequence != null;
            if(playbackPanel.Enabled)
            {
                playbackTrackBar.MaximumTime = currentSequence.Duration;
                playbackTrackBar.CurrentTime = 0;
                time = 0.0f;
            }
        }

        protected override void sceneLoaded(SimScene scene)
        {
            SimSubScene defaultScene = scene.getDefaultSubScene();
            if (defaultScene != null)
            {
                SimulationScene medicalScene = defaultScene.getSimElementManager<SimulationScene>();
                muscleSequenceView.initializeSequences(medicalScene, MedicalController.CurrentSceneDirectory);
            }
        }

        protected override void sceneUnloading()
        {
            muscleSequenceView.clearSequences();
        }

        protected override void fixedLoopUpdate(Clock time)
        {
            base.fixedLoopUpdate(time);
            this.time += (float)time.Seconds;
            playbackTrackBar.CurrentTime = this.time % playbackTrackBar.MaximumTime;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            subscribeToUpdates();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopPlayback();
        }
    }
}
