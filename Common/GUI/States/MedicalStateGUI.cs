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
using Medical.Properties;

namespace Medical.GUI
{
    public delegate MedicalState CreateStateCallback(int stateIndex);

    public partial class MedicalStateGUI : GUIElement
    {
        private MedicalStateController stateController;
        //private MedicalController medicalController;
        private bool playing = false;
        private double targetTime = 0.0f;
        private double playbackSpeed = 1.0f;
        private Dictionary<MedicalState, MedicalStateTrackMark> trackMarks = new Dictionary<MedicalState, MedicalStateTrackMark>();
        private CreateStateCallback createStateCallback = null;

        public MedicalStateGUI()
        {
            InitializeComponent();
            stateImageList.Images.AddStrip(Resources.StateButtons);
            stateTrackBar.TimeChanged += new TimeChanged(stateTrackBar_TimeChanged);
        }

        public void initialize(MedicalStateController stateController)
        {
            this.stateController = stateController;
            stateController.StateAdded += new MedicalStateAdded(MedicalStates_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(MedicalStates_StateRemoved);
            stateController.StatesCleared += new MedicalStateEvent(MedicalStates_StatesCleared);
        }

        public void next()
        {
            int blend = (int)stateTrackBar.CurrentTime;
            int numStates = stateController.getNumStates();
            if (blend < numStates && numStates > 1)
            {
                startPlayback(blend + 1, 1.0);
            }
        }

        public void previous()
        {
            int blend = (int)stateTrackBar.CurrentTime;
            if (blend != stateTrackBar.CurrentTime)
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
            startPlayback(stateController.getNumStates() - 1, 1.0);
        }

        public void setToEnd()
        {
            CurrentBlend = stateController.getNumStates() - 1;
        }

        public void pause()
        {
            if (playing)
            {
                playing = false;
                unsubscribeFromUpdates();
            }
        }

        void stateTrackBar_TimeChanged(TimeTrackBar trackBar, double currentTime)
        {
            stateController.blend((float)currentTime);
        }

        void MedicalStates_StatesCleared(MedicalStateController controller)
        {
            stateTrackBar.clearMarks();
        }

        void MedicalStates_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            MedicalStateTrackMark mark;
            trackMarks.TryGetValue(state, out mark);
            if (mark != null)
            {
                stateTrackBar.removeMark(mark);
                foreach (MedicalStateTrackMark reindex in trackMarks.Values)
                {
                    if (reindex.Location > index)
                    {
                        reindex.Location--;
                    }
                }
                if (stateTrackBar.CurrentTime > index)
                {
                    stateTrackBar.CurrentTime -= 1;
                }
                stateTrackBar.MaximumTime = controller.getNumStates() - 1;
            }
        }

        void MedicalStates_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            //stateTrackBar.Enabled = controller.getNumStates() > 1;
            stateTrackBar.MaximumTime = controller.getNumStates() - 1;
            foreach (MedicalStateTrackMark reindex in trackMarks.Values)
            {
                if (reindex.Location >= index)
                {
                    reindex.Location++;
                }
            }
            MedicalStateTrackMark mark = new MedicalStateTrackMark(state);
            mark.Location = index;
            stateTrackBar.addMark(mark);
            trackMarks.Add(state, mark);
        }

        protected override void fixedLoopUpdate(Clock time)
        {
            double nextTime = stateTrackBar.CurrentTime + time.Seconds * playbackSpeed;
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
            stateTrackBar.CurrentTime = (float)nextTime;
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
                subscribeToUpdates();
            }
        }

        public double CurrentBlend
        {
            get
            {
                return stateTrackBar.CurrentTime;
            }
            set
            {
                stateTrackBar.CurrentTime = (float)value;
            }
        }

        public CreateStateCallback CreateStateCallback
        {
            get
            {
                return createStateCallback;
            }
            set
            {
                createStateCallback = value;
                if (createStateCallback != null)
                {
                    stateTrackBar.BarMenu = barMenu;
                }
                else
                {
                    stateTrackBar.BarMenu = null;
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MedicalStateTrackMark mark = stateTrackBar.MenuTargetMark as MedicalStateTrackMark;
            stateController.destroyState(mark.State);
        }

        private void appendStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createStateCallback.Invoke(stateController.getNumStates());
        }

        private void insertStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createStateCallback.Invoke((int)stateTrackBar.BarMenuTime + 1);
        }

        private void insertStateAtStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createStateCallback.Invoke(0);
        }
    }
}
