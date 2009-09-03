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
            timeTrackBar.addMark(new TrackBarMark(0.0f));
            timeTrackBar.addMark(new TrackBarMark(0.3f));
            timeTrackBar.addMark(new TrackBarMark(1.0f));
            timeTrackBar.MaximumTime = 1.0f;
        }

        protected override void fixedLoopUpdate(Clock time)
        {
            base.fixedLoopUpdate(time);
            this.time += (float)time.Seconds;
            movementSequence.setPosition(this.time);
        }

        private void addStateButton_Click(object sender, EventArgs e)
        {
            MovementSequenceState state = new MovementSequenceState();
            state.captureState();
            movementSequence.addState(state);
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
    }
}
