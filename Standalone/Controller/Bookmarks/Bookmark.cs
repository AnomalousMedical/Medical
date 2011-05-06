using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class Bookmark
    {
        public Bookmark(Vector3 cameraTrans, Vector3 cameraLookAt, LayerState layers)
        {
            this.CameraTranslation = cameraTrans;
            this.CameraLookAt = cameraLookAt;
            this.Layers = layers;
        }

        public Vector3 CameraTranslation { get; set; }

        public Vector3 CameraLookAt { get; set; }

        public LayerState Layers { get; set; }
    }
}
