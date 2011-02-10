using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.Saving;
using Medical.Controller;

namespace Medical
{
    [TimelineActionProperties("Move Camera")]
    public class MoveCameraAction : TimelineAction
    {
        private float lastTime;

        public MoveCameraAction()
            :this(0.0f, null)
        {

        }

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

        public override void skipTo(float timelineTime)
        {
            MDISceneViewWindow window = TimelineController.SceneViewController.findWindow(CameraName);
            if (timelineTime <= EndTime)
            {
                Vector3 translation = window.Translation;
                Vector3 lookAt = window.LookAt;
                Vector3 finalTrans = Translation;
                Vector3 finalLookAt = LookAt;
                float percent = 1.0f;
                float currentTime = timelineTime - StartTime;
                if (Duration != 0.0f)
                {
                    percent = currentTime / Duration;
                }
                window.immediatlySetPosition(translation.lerp(ref finalTrans, ref percent), lookAt.lerp(ref finalLookAt, ref percent));
                window.setPosition(Translation, LookAt, Duration - currentTime);
            }
            else
            {
                window.immediatlySetPosition(Translation, LookAt);
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            lastTime = timelineTime;
        }

        public override bool Finished
        {
            get
            {
                return lastTime > StartTime + Duration;
            }
        }

        public override void capture()
        {
            SceneViewWindow currentWindow = TimelineController.SceneViewController.ActiveWindow;
            Translation = currentWindow.Translation;
            LookAt = currentWindow.LookAt;
            CameraName = currentWindow.Name;
        }

        public override void editing()
        {
            TimelineController.SceneViewController.ActiveWindow.setPosition(Translation, LookAt);
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
