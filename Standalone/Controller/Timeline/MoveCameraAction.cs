using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Engine.Saving;
using Medical.Controller;
using Logging;
using Engine.Editing;

namespace Medical
{
    public class MoveCameraAction : TimelineAction
    {
        private float lastTime;

        public MoveCameraAction()
            : this(0.0f, null)
        {
            CameraPosition = new CameraPosition();
        }

        public MoveCameraAction(float startTime, String cameraName)
        {
            this.StartTime = startTime;
            this.CameraName = cameraName;
            this.Duration = 1.0f;
            UseSystemCameraTransitionTime = false;
            CameraPosition = new CameraPosition();
        }

        public MoveCameraAction(float startTime, String cameraName, Vector3 translation, Vector3 lookAt)
            : this(startTime, cameraName)
        {
            CameraPosition = new CameraPosition()
            {
                Translation = translation,
                LookAt = lookAt
            };
        }

        public override void started(float timelineTime, Clock clock)
        {
            MDISceneViewWindow window = TimelineController.SceneViewController.findWindow(CameraName);
            if (window != null)
            {
                window.setPosition(CameraPosition, Duration);
            }
            else
            {
                SceneViewWindow sceneViewWindow = TimelineController.SceneViewController.ActiveWindow;
                if (sceneViewWindow != null)
                {
                    sceneViewWindow.setPosition(CameraPosition, Duration);
                }
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
                    Vector3 finalTrans = window.computeAdjustedTranslation(CameraPosition);
                    Vector3 finalLookAt = CameraPosition.LookAt;
                    float percent = 1.0f;
                    float currentTime = timelineTime - StartTime;
                    if (Duration != 0.0f)
                    {
                        percent = currentTime / Duration;
                    }
                    CameraPosition immediatePos = new CameraPosition()
                    {
                        Translation = translation.lerp(ref finalTrans, ref percent),
                        LookAt = lookAt.lerp(ref finalLookAt, ref percent)
                    };
                    window.immediatlySetPosition(immediatePos);
                    float time = Duration - currentTime;
                    if (time <= 0.001f)
                    {
                        time = 0.001f;
                    }
                    CameraPosition cameraPosition = new CameraPosition(CameraPosition)
                    {
                        Translation = finalTrans,
                    };
                    window.setPosition(cameraPosition, time);
                }
                else
                {
                    Vector3 finalTrans = window.computeAdjustedTranslation(CameraPosition);
                    CameraPosition immediatePos = new CameraPosition()
                    {
                        Translation = finalTrans,
                        LookAt = CameraPosition.LookAt
                    };
                    window.immediatlySetPosition(immediatePos);

                    //Its weird that you have to do this, but the position won't visibly update if you don't.
                    CameraPosition cameraPosition = new CameraPosition(CameraPosition)
                    {
                        Translation = finalTrans,
                    };
                    window.setPosition(cameraPosition, 0.001f);
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
            CameraPosition.Translation = new Vector3(-CameraPosition.Translation.x, CameraPosition.Translation.y, CameraPosition.Translation.z);
            CameraPosition.LookAt = new Vector3(-CameraPosition.LookAt.x, CameraPosition.LookAt.y, CameraPosition.LookAt.z);
            CameraPosition.IncludePoint = new Vector3(-CameraPosition.IncludePoint.x, CameraPosition.IncludePoint.y, CameraPosition.IncludePoint.z);
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
            CameraPosition.Translation = currentWindow.Translation;
            CameraPosition.LookAt = currentWindow.LookAt;
            CameraName = currentWindow.Name;

            //Make the include point projected out to the lookat location
            currentWindow.calculateIncludePoint(CameraPosition);

            fireDataNeedsRefresh();
        }

        public override void editing()
        {
            if (TimelineController != null)
            {
                SceneViewWindow window = TimelineController.SceneViewController.ActiveWindow;
                if (window != null)
                {
                    window.setPosition(CameraPosition, MedicalConfig.CameraTransitionTime);
                }
            }
        }

        public override void findFileReference(TimelineStaticInfo info)
        {

        }

        public override void cleanup(CleanupInfo cleanupInfo)
        {

        }

        [Editable(Advanced = true)]
        public CameraPosition CameraPosition { get; set; }

        [Editable(Advanced = true)]
        public String CameraName { get; set; }

        [Editable(Advanced = true)]
        public bool UseSystemCameraTransitionTime { get; set; }

        public override float Duration
        {
            get
            {
                return UseSystemCameraTransitionTime ? MedicalConfig.CameraTransitionTime : base.Duration;
            }
            set
            {
                base.Duration = value;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addCommand(new EditInterfaceCommand("Capture", (callback, caller) =>
            {
                capture();
            }));
        }

        public override string TypeName
        {
            get
            {
                return "Move Camera";
            }
        }

        #region Saveable

        private static readonly String TRANSLATION = "Translation";
        private static readonly String LOOKAT = "LookAt";
        private static readonly String CAMERA_NAME = "CameraName";
        private static readonly String INCLUDE_POINT = "IncludePoint";
        private static readonly String USE_SYSTEM_CAMERA_TRANSITION_TIME = "UseSystemCameraTransitionTime";
        private static readonly String EASING = "Easing";
        private static readonly String CAMERA_POSITION = "Easing";

        protected MoveCameraAction(LoadInfo info)
            : base(info)
        {
            CameraPosition = info.GetValue<CameraPosition>(CAMERA_POSITION, null);
            if (CameraPosition == null)
            {
                CameraPosition = new CameraPosition()
                {
                    Translation = info.GetVector3(TRANSLATION),
                    LookAt = info.GetVector3(LOOKAT),
                    IncludePoint = info.GetVector3(INCLUDE_POINT, Vector3.Invalid),
                    Easing = info.GetValue(EASING, EasingFunction.EaseOutQuadratic),
                };
                CameraPosition.UseIncludePoint = CameraPosition.IncludePoint.isNumber();
            }
            
            CameraName = info.GetString(CAMERA_NAME);
            UseSystemCameraTransitionTime = info.GetBoolean(USE_SYSTEM_CAMERA_TRANSITION_TIME, false);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(CAMERA_POSITION, CameraPosition);
            info.AddValue(CAMERA_NAME, CameraName);
            info.AddValue(USE_SYSTEM_CAMERA_TRANSITION_TIME, UseSystemCameraTransitionTime);
            base.getInfo(info);
        }

        #endregion
    }
}
