using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Standalone;
using Medical.Controller;

namespace Medical
{
    class TimelineController : UpdateListener
    {
        private Timeline activeTimeline;
        private UpdateTimer mainTimer;
        private StandaloneController standaloneController;
        private bool updating = false;

        public TimelineController(StandaloneController standaloneController)
        {
            this.mainTimer = standaloneController.MedicalController.MainTimer;
            this.standaloneController = standaloneController;
        }

        public Timeline ActiveTimeline
        {
            get
            {
                return activeTimeline;
            }
        }

        public void startPlayback(Timeline timeline)
        {
            if (!updating)
            {
                activeTimeline = timeline;
                activeTimeline.TimelineController = this;
                activeTimeline.start();
                mainTimer.addFixedUpdateListener(this);
                updating = true;
            }
        }

        public void stopPlayback()
        {
            if (updating)
            {
                activeTimeline.stop();
                mainTimer.removeFixedUpdateListener(this);
                activeTimeline.TimelineController = null;
                activeTimeline = null;
                updating = false;
            }
        }

        #region UpdateListener Members

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            activeTimeline.update(clock);
            if (activeTimeline.Finished)
            {
                stopPlayback();
            }
        }

        #endregion

        public SceneViewController SceneViewController
        {
            get
            {
                return standaloneController.SceneViewController;
            }
        }

        public MedicalStateController MedicalStateController
        {
            get
            {
                return standaloneController.MedicalStateController;
            }
        }

        public MovementSequenceController MovementSequenceController
        {
            get
            {
                return standaloneController.MovementSequenceController;
            }
        }
    }
}
