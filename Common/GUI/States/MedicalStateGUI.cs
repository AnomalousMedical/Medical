using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine.Platform;

namespace Medical.GUI
{
    public partial class MedicalStateGUI : DockContent
    {
        private MedicalController medicalController;
        private bool playing = false;
        private double targetTime = 0.0f;
        private double playbackSpeed = 1.0f;

        public MedicalStateGUI()
        {
            InitializeComponent();
            medicalStateTrackBar.CurrentBlendChanged += new CurrentBlendChanged(medicalStateTrackBar_CurrentBlendChanged);
        }

        public void initialize(MedicalController medicalController)
        {
            this.medicalController = medicalController;
            medicalController.MedicalStates.StateAdded += new MedicalStateAdded(MedicalStates_StateAdded);
            medicalController.MedicalStates.StateRemoved += new MedicalStateRemoved(MedicalStates_StateRemoved);
            medicalController.MedicalStates.StatesCleared += new MedicalStatesCleared(MedicalStates_StatesCleared);
        }

        public void sceneLoaded()
        {

        }

        public void sceneUnloading()
        {

        }

        void medicalStateTrackBar_CurrentBlendChanged(MedicalStateTrackBar trackBar, double currentTime)
        {
            medicalController.MedicalStates.blend((float)currentTime);
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            medicalController.createMedicalState("Test");
        }

        void MedicalStates_StatesCleared(MedicalStateController controller)
        {
            medicalStateTrackBar.clearStates();
        }

        void MedicalStates_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            medicalStateTrackBar.removeState(state);
        }

        void MedicalStates_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            medicalStateTrackBar.Enabled = controller.getNumStates() > 1;
            if (medicalStateTrackBar.Enabled)
            {
                medicalStateTrackBar.MaxBlend = controller.getNumStates() - 1;
            }
            medicalStateTrackBar.addState(state, index);
        }

        void loopUpdate(Clock time)
        {
            double nextTime = medicalStateTrackBar.CurrentBlend + time.Seconds * playbackSpeed;
            if (playbackSpeed > 0)
            {
                if (nextTime > targetTime)
                {
                    nextTime = targetTime;
                    pausePlayback();
                }
            }
            else if (playbackSpeed < 0)
            {
                if (nextTime < targetTime)
                {
                    nextTime = targetTime;
                    pausePlayback();
                }
            }
            medicalStateTrackBar.CurrentBlend = nextTime;
        }

        private void playAllButton_Click(object sender, EventArgs e)
        {
            startPlayback(medicalController.MedicalStates.getNumStates(), 1.0);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            int blend = (int)medicalStateTrackBar.CurrentBlend;
            if (blend != medicalStateTrackBar.CurrentBlend)
            {
                startPlayback(blend, -1.0);
            }
            else if (blend > 0)
            {
                startPlayback(blend - 1, -1.0);
            }
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            pausePlayback();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            int blend = (int)medicalStateTrackBar.CurrentBlend;
            if (blend < medicalController.MedicalStates.getNumStates())
            {
                startPlayback(blend + 1, 1.0);
            }
        }

        private void startPlayback(double goalTime, double playbackDirection)
        {
            targetTime = goalTime;
            switch (speedTrackBar.Value)
            {
                case(0):
                    playbackSpeed = playbackDirection * 0.2;
                    break;
                case (1):
                    playbackSpeed = playbackDirection * 0.5;
                    break;
                case (2):
                    playbackSpeed = playbackDirection * 1.0;
                    break;
                case (3):
                    playbackSpeed = playbackDirection * 3.0;
                    break;
                case (4):
                    playbackSpeed = playbackDirection * 10.0;
                    break;
            }
            if (!playing)
            {
                playing = true;
                medicalController.LoopUpdate += loopUpdate;
            }
        }

        private void pausePlayback()
        {
            if (playing)
            {
                playing = false;
                medicalController.LoopUpdate -= loopUpdate;
            }
        }
    }
}
