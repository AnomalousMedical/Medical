using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public abstract class PresetState : Saveable
    {
        private String category;
        private String name;
        private String imageName;

        public PresetState(String name, String category, String imageName)
        {
            this.category = category;
            this.name = name;
            this.imageName = imageName;
        }

        public abstract void applyToState(MedicalState state);

        public String Category
        {
            get
            {
                return category;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public String ImageName
        {
            get
            {
                return imageName;
            }
        }

        #region Saveable Members

        private const String CATEGORY = "Category";
        private const String NAME = "Name";
        private const String IMAGE_NAME = "ImageName";

        protected PresetState(LoadInfo info)
        {
            category = info.GetString(CATEGORY);
            name = info.GetString(NAME);
            imageName = info.GetString(IMAGE_NAME);
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue(CATEGORY, category);
            info.AddValue(NAME, name);
            info.AddValue(IMAGE_NAME, imageName);
        }

        #endregion
    }
}
