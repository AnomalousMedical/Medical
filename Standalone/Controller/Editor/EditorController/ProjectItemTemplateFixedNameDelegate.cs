using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    public class ProjectItemTemplateFixedNameDelegate : ProjectItemTemplate
    {
        public delegate void CreateItem(String path, EditorController editorController);

        private CreateItem createItemCallback;

        public ProjectItemTemplateFixedNameDelegate(String typeName, String imageName, CreateItem createItemCallback)
        {
            this.TypeName = typeName;
            this.ImageName = imageName;
            this.createItemCallback = createItemCallback;
        }

        public string TypeName { get; private set; }

        public string ImageName { get; private set; }

        public void createItem(string path, EditorController editorController)
        {
            createItemCallback(path, editorController);
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
