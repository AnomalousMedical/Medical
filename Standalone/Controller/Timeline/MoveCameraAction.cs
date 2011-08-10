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
            IncludePoint = Vector3.Invalid;
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
                window.setPosition(computeTranslationWithIncludePoint(window), LookAt, Duration);
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
                    Vector3 finalTrans = computeTranslationWithIncludePoint(window);
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
                    window.setPosition(finalTrans, LookAt, time);
                }
                else
                {
                    Vector3 finalTrans = computeTranslationWithIncludePoint(window);
                    window.immediatlySetPosition(finalTrans, LookAt);
                    window.setPosition(finalTrans, LookAt, 0.001f); //Its weird that you have to do this, but the position won't visibly update if you don't.
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
            IncludePoint = new Vector3(-IncludePoint.x, IncludePoint.y, IncludePoint.z);
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

            //Make the include point projected out to the lookat location
            Ray3 camRay = currentWindow.getCameraToViewportRay(1, 0);
            IncludePoint = camRay.Origin + camRay.Direction * (LookAt - Translation).length();
        }

        public override void editing()
        {
            if (TimelineController != null)
            {
                SceneViewWindow window = TimelineController.SceneViewController.ActiveWindow;
                if (window != null)
                {
                    window.setPosition(computeTranslationWithIncludePoint(window), LookAt);
                }
            }
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            
        }

        public Vector3 Translation { get; set; }

        public Vector3 LookAt { get; set; }

        public String CameraName { get; set; }

        public Vector3 IncludePoint { get; set; }

        private Vector3 computeTranslationWithIncludePoint(SceneViewWindow sceneWindow)
        {
            if (IncludePoint.isNumber())
            {
                float aspect = sceneWindow.Camera.getAspectRatio();
                float fovy = sceneWindow.Camera.getFOVy() * 0.5f;

                Vector3 direction = LookAt - Translation;

                //Figure out direction, must use ogre fixed yaw calculation, first adjust direction to face -z
                Vector3 zAdjustVec = -direction;
                zAdjustVec.normalize();
                Quaternion targetWorldOrientation = Quaternion.shortestArcQuatFixedYaw(ref zAdjustVec);

                Matrix4x4 viewMatrix = Matrix4x4.makeViewMatrix(Translation, targetWorldOrientation);
                Matrix4x4 projectionMatrix = sceneWindow.Camera.getProjectionMatrix();
                float offset = SceneViewWindow.computeOffsetToIncludePoint(viewMatrix, projectionMatrix, IncludePoint, aspect, fovy);

                direction.normalize();
                Vector3 newTrans = Translation + offset * direction;
                return newTrans;
            }

            return Translation;
        }

        #region Saveable

        private static readonly String TRANSLATION = "Translation";
        private static readonly String LOOKAT = "LookAt";
        private static readonly String CAMERA_NAME = "CameraName";
        private static readonly String INCLUDE_POINT = "IncludePoint";

        protected MoveCameraAction(LoadInfo info)
            : base(info)
        {
            Translation = info.GetVector3(TRANSLATION);
            LookAt = info.GetVector3(LOOKAT);
            CameraName = info.GetString(CAMERA_NAME);
            IncludePoint = info.GetVector3(INCLUDE_POINT, Vector3.Invalid);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue(TRANSLATION, Translation);
            info.AddValue(LOOKAT, LookAt);
            info.AddValue(CAMERA_NAME, CameraName);
            info.AddValue(INCLUDE_POINT, IncludePoint);
            base.getInfo(info);
        }

        #endregion
    }
}
