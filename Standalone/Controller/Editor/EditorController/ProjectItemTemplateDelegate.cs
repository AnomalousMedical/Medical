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

        public ProjectItemTemplateDelegate(String typeName, String imageName, CreateItem createItemCallback)
        {
            this.TypeName = typeName;
            this.ImageName = imageName;
            this.createItemCallback = createItemCallback;
        }

        public string TypeName { get; private set; }

        public string ImageName { get; private set; }

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
