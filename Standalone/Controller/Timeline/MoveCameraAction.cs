using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;

namespace Medical
{
    class MoveCameraAction : TimelineAction
    {
        public MoveCameraAction(float startTime, String cameraName)
        {
            this.StartTime = startTime;
            this.CameraName = cameraName;
            this.Duration = 1.0f;
        }

        public MoveCameraAction(float startTime, String cameraName, Vector3 translation, Vector3 lookAt)
            :this(startTime, cameraName)
        {
            this.Translation = translation;
            this.LookAt = lookAt;
        }

        public void started(float timelineTime, Clock clock)
        {
            TimelineController.Instance.SceneViewController.findWindow(CameraName).setPosition(Translation, LookAt, Duration);
        }

        public void stopped(float timelineTime, Clock clock)
        {
            
        }

        public void update(float timelineTime, Clock clock)
        {
            
        }

        public float StartTime
        {
            get;
            private set;
        }

        public bool Finished
        {
            get
            {
                return true;
            }
        }

        public Vector3 Translation { get; set; }

        public Vector3 LookAt { get; set; }

        public String CameraName { get; set; }

        public float Duration { get; set; }
    }
}
