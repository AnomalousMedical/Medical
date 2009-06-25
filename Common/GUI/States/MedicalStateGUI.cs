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
    public partial class MedicalStateGUI : GUIElement
    {
        private MedicalStateController stateController;
        private MedicalController medicalController;
        private bool playing = false;
        private double targetTime = 0.0f;
        private double playbackSpeed = 1.0f;

        public MedicalStateGUI()
        {
            InitializeComponent();
            medicalStateTrackBar.CurrentBlendChanged += new CurrentBlendChanged(medicalStateTrackBar_CurrentBlendChanged);
        }

        public void initialize(MedicalStateController stateController, MedicalController medicalController)
        {
            this.stateController = stateController;
            this.medicalController = medicalController;
            stateController.StateAdded += new MedicalStateAdded(MedicalStates_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(MedicalStates_StateRemoved);
            stateController.StatesCleared += new MedicalStatesCleared(MedicalStates_StatesCleared);
        }

        public void next()
        {
            int blend = (int)medicalStateTrackBar.CurrentBlend;
            int numStates = stateController.getNumStates();
            if (blend < numStates && numStates > 1)
            {
                startPlayback(blend + 1, 1.0);
            }
        }

        public void previous()
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

        public void playAll()
        {
            startPlayback(stateController.getNumStates(), 1.0);
        }

        public void pause()
        {
            if (playing)
            {
                playing = false;
                medicalController.FullSpeedLoopUpdate -= loopUpdate;
            }
        }

        void medicalStateTrackBar_CurrentBlendChanged(MedicalStateTrackBar trackBar, double currentTime)
        {
            stateController.blend((float)currentTime);
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
                    pause();
                }
            }
            else if (playbackSpeed < 0)
            {
                if (nextTime < targetTime)
                {
                    nextTime = targetTime;
                    pause();
                }
            }
            medicalStateTrackBar.CurrentBlend = nextTime;
        }

        private void playAllButton_Click(object sender, EventArgs e)
        {
            playAll();
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            previous();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            pause();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            next();
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
                medicalController.FullSpeedLoopUpdate += loopUpdate;
            }
        }

        public double CurrentBlend
        {
            get
            {
                return medicalStateTrackBar.CurrentBlend;
            }
            set
            {
                medicalStateTrackBar.CurrentBlend = value;
            }
        }
    }
}
