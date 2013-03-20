using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class sits between the CameraMovers and the SceneView, it keeps the near and
    /// far planes in check for the camera, which prevents ugly rendering artifacts.
    /// </summary>
    public class CameraPositioner
    {
        private SceneView sceneView;
        private float nearPlaneWorldPos;
        private float farPlaneWorldPos;

        public CameraPositioner(SceneView sceneView, float nearPlaneWorldPos, float farPlaneWorldPos)
        {
            this.sceneView = sceneView;
            this.nearPlaneWorldPos = nearPlaneWorldPos;
            this.farPlaneWorldPos = farPlaneWorldPos;
            updateNearFarPlanes();
        }

        public Vector3 LookAt
        {
            get
            {
                return sceneView.LookAt;
            }
            set
            {
                sceneView.LookAt = value;
                updateNearFarPlanes();
            }
        }

        public Vector3 Translation
        {
            get
            {
                return sceneView.Translation;
            }
            set
            {
                sceneView.Translation = value;
                updateNearFarPlanes();
            }
        }

        private void updateNearFarPlanes()
        {
            float distance = Translation.length();
            float near = distance - nearPlaneWorldPos;
            if (near < 1.0f)
            {
                near = 1.0f;
            }
            float far = distance - farPlaneWorldPos;
            if (far < near)
            {
                far = near + 500.0f;
            }
            sceneView.setNearClipDistance(near);
            sceneView.setFarClipDistance(far);
        }
    }
}
