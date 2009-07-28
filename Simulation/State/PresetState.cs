using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class PresetState
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
    }
}
