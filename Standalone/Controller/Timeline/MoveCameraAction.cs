using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.Saving;

namespace Medical
{
    class MoveCameraAction : TimelineAction
    {
        public static readonly String Name = "Move Camera";

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

        public override void started(float timelineTime, Clock clock)
        {
            TimelineController.SceneViewController.findWindow(CameraName).setPosition(Translation, LookAt, Duration);
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            
        }

        public override bool Finished
        {
            get
            {
                return true;
            }
        }

        public override String TypeName
        {
            get { return Name; }
        }

        public Vector3 Translation { get; set; }

        public Vector3 LookAt { get; set; }

        public String CameraName { get; set; }

        #region Saveable

        private static readonly String TRANSLATION = "Translation";
        private static readonly String LOOKAT = "LookAt";
        private static readonly String CAMERA_NAME = "CameraName";

        protected MoveCameraAction(LoadInfo info)
            : base(info)
        {
            Translation = info.GetVector3(TRANSLATION);
            LookAt = info.GetVector3(LOOKAT);
            CameraName = info.GetString(CAMERA_NAME);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(TRANSLATION, Translation);
            info.AddValue(LOOKAT, LookAt);
            info.AddValue(CAMERA_NAME, CameraName);
            base.getInfo(info);
        }

        #endregion
    }
}
