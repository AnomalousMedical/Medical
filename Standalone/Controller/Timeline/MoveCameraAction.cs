using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.Saving;
using Medical.Controller;
using Logging;

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
            MDISceneViewWindow window = TimelineController.SceneViewController.findWindow(CameraName);
            if (window != null)
            {
                window.setPosition(Translation, LookAt, Duration);
            }
            else
            {
                Log.Warning("Window {0} not found. Could not do MoveCameraAction.", CameraName);
            }
        }

        public override void skipTo(float timelineTime)
        {
            MDISceneViewWindow window = TimelineController.SceneViewController.findWindow(CameraName);
            if (window != null)
            {
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
                    float time = Duration - currentTime;
                    if (time == 0.0f)
                    {
                        time = 0.001f;
                    }
                    window.setPosition(Translation, LookAt, time);
                }
                else
                {
                    window.immediatlySetPosition(Translation, LookAt);
                    window.setPosition(Translation, LookAt, 0.001f); //Its weird that you have to do this, but the position won't visibly update if you don't.
                }
            }
        }

        public override void stopped(float timelineTime, Clock clock)
        {
            
        }

        public override void update(float timelineTime, Clock clock)
        {
            lastTime = timelineTime;
        }

        public override void reverseSides()
        {
            Translation = new Vector3(-Translation.x, Translation.y, Translation.z);
            LookAt = new Vector3(-LookAt.x, LookAt.y, LookAt.z);
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
            if (TimelineController != null)
            {
                SceneViewWindow window = TimelineController.SceneViewController.ActiveWindow;
                if (window != null)
                {
                    window.setPosition(Translation, LookAt);
                }
            }
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
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
