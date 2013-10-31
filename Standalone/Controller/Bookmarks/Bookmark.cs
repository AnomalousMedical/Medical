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
            CameraPosition = new CameraPosition()
            {
                Translation = cameraTrans,
                LookAt = cameraLookAt,
            };
            this.Layers = layers;
        }

        public CameraPosition CameraPosition { get; set; }

        public LayerState Layers { get; set; }

        public String Name { get; set; }

        internal String BackingFile { get; set; }

        #region Saveable Members

        private static String TRANSLATION = "CameraTranslation";
        private static String LOOK_AT = "CameraLookAt";
        private static String POSITION = "CameraPosition";
        private static String LAYERS = "Layers";
        private static String NAME = "Name";

        protected Bookmark(LoadInfo info)
        {
            CameraPosition = info.GetValue<CameraPosition>(POSITION, null);
            if (CameraPosition == null)
            {
                CameraPosition = new CameraPosition()
                {
                    Translation = info.GetVector3(TRANSLATION),
                    LookAt = info.GetVector3(LOOK_AT)
                };
            }
            Layers = info.GetValue<LayerState>(LAYERS);
            Name = info.GetString(NAME);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(POSITION, CameraPosition);
            info.AddValue(LAYERS, Layers);
            info.AddValue(NAME, Name);
        }

        #endregion
    }
}
