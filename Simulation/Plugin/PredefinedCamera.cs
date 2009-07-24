using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class PredefinedCamera : Saveable
    {
        [Editable]
        private Vector3 translation = Vector3.UnitX;
        [Editable]
        private Vector3 lookAt = Vector3.Zero;
        private String name;

        public PredefinedCamera(String name, Vector3 translation, Vector3 lookAt)
        {
            this.name = name;
            this.translation = translation;
            this.lookAt = lookAt;
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
        }

        public Vector3 LookAt
        {
            get
            {
                return lookAt;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        #region Saveable Members

        private const String NAME = "Name";
        private const String TRANSLATION = "Translation";
        private const String LOOK_AT = "LookAt";

        protected PredefinedCamera(LoadInfo info)
        {
            name = info.GetString(NAME);
            translation = info.GetVector3(TRANSLATION);
            lookAt = info.GetVector3(LOOK_AT);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, name);
            info.AddValue(TRANSLATION, translation);
            info.AddValue(LOOK_AT, lookAt);
        }

        #endregion
    }
}
