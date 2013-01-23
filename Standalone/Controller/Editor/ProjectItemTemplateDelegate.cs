using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    public class ProjectItemTemplateDelegate : ProjectItemTemplate
    {
        public delegate void CreateItem(String path, String fileName, EditorController editorController);

        private CreateItem createItemCallback;

        public ProjectItemTemplateDelegate(String typeName, String imageName, String group, CreateItem createItemCallback)
        {
            this.TypeName = typeName;
            this.ImageName = imageName;
            this.Group = group;
            this.createItemCallback = createItemCallback;
        }

        public string TypeName { get; private set; }

        public string ImageName { get; private set; }

        public string Group { get; private set; }

        public bool isValid(out String errorMessage)
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

        public void createItem(string path, EditorController editorController)
        {
            createItemCallback(path, Name, editorController);
        }

        public EditInterface EditInterface
        {
            get
            {
                return ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
            }
        }
    }
}
