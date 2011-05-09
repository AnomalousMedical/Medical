using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using System.Drawing;

namespace Medical
{
    public class Bookmark : Saveable
    {
        public Bookmark(String name, Vector3 cameraTrans, Vector3 cameraLookAt, LayerState layers)
        {
            this.Name = name;
            this.CameraTranslation = cameraTrans;
            this.CameraLookAt = cameraLookAt;
            this.Layers = layers;
        }

        public Vector3 CameraTranslation { get; set; }

        public Vector3 CameraLookAt { get; set; }

        public LayerState Layers { get; set; }

        public String Name { get; set; }

        #region Saveable Members

        private static String TRANSLATION = "CameraTranslation";
        private static String LOOK_AT = "CameraLookAt";
        private static String LAYERS = "Layers";
        private static String NAME = "Name";

        protected Bookmark(LoadInfo info)
        {
            CameraTranslation = info.GetVector3(TRANSLATION);
            CameraLookAt = info.GetVector3(LOOK_AT);
            Layers = info.GetValue<LayerState>(LAYERS);
            Name = info.GetString(NAME);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(TRANSLATION, CameraTranslation);
            info.AddValue(LOOK_AT, CameraLookAt);
            info.AddValue(LAYERS, Layers);
            info.AddValue(NAME, Name);
        }

        #endregion
    }
}
