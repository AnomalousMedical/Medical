using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Platform;
using Medical.Muscles;

namespace Medical.GUI
{
    public partial class MovementStateControl : GUIElement
    {
        private MovementSequence movementSequence = new MovementSequence("Test");
        private float time = 0.0f;

        public MovementStateControl()
        {
            InitializeComponent();
            movementSequence.Duration = 5.0f;
            timeUpDown.ValueChanged += new EventHandler(timeUpDown_ValueChanged);
            timeTrackBar.MoveMarks = true;
            timeTrackBar.MarkMoved += new MarkMoved(timeTrackBar_MarkMoved);
            timeTrackBar.TickMenu = tickMenu;
            timeTrackBar.BarMenu = barMenu;
            timeTrackBar.TimeChanged += new TimeChanged(timeTrackBar_TimeChanged);
        }

        void timeTrackBar_TimeChanged(TimeTrackBar trackBar, double currentTime)
        {
            movementSequence.setPosition((float)currentTime);
        }

        protected override void fixedLoopUpdate(Clock time)
        {
            base.fixedLoopUpdate(time);
            this.time += (float)time.Seconds;
            timeTrackBar.CurrentTime = this.time % timeTrackBar.MaximumTime;
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            time = 0.0f;
            subscribeToUpdates();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            unsubscribeFromUpdates();
        }

        void timeUpDown_ValueChanged(object sender, EventArgs e)
        {
            movementSequence.Duration = (float)timeUpDown.Value;
            timeTrackBar.MaximumTime = movementSequence.Duration;
        }

        void timeTrackBar_MarkMoved(TrackBarMark mark)
        {
            movementSequence.sortStates();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MovementStateTick tick = timeTrackBar.MenuTargetMark as MovementStateTick;
            if (tick != null)
            {
                timeTrackBar.removeMark(tick);
                movementSequence.deleteState(tick.State);
            }
        }

        private void addKeyStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MovementSequenceState state = new MovementSequenceState();
            state.StartTime = timeTrackBar.BarMenuTime;
            state.captureState();
            movementSequence.addState(state);
            timeTrackBar.addMark(new MovementStateTick(state));
            timeTrackBar.MaximumTime = movementSequence.Duration;
        }

        private void lockButtonCheckChanged(object sender, EventArgs e)
        {
            timeTrackBar.MoveMarks = !lockButton.Checked;
        }
    }
}
