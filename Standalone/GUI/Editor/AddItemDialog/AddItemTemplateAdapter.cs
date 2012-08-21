using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    public class AddItemTemplateAdapter : AddItemTemplate
    {
        public AddItemTemplateAdapter(String typeName, String imageName, String group)
        {
            this.TypeName = typeName;
            this.ImageName = imageName;
            this.Group = group;
        }

        public string TypeName { get; private set; }

        public string ImageName { get; private set; }

        public string Group { get; private set; }

        public virtual bool isValid(out String errorMessage)
        {
            if (String.IsNullOrEmpty(Name))
            {
                errorMessage = "Please enter a name.";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        public virtual void reset()
        {
            Name = null;
        }

        [Editable]
        public String Name { get; set; }

        public EditInterface EditInterface
        {
            get
            {
                return ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
            }
        }
    }
}
