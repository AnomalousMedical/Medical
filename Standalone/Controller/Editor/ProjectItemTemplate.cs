using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    public interface ProjectItemTemplate : AddItemTemplate
    {
        void createItem(String path, EditorController editorController);
    }
}
