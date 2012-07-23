using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    public interface ProjectItemTemplate
    {
        String TypeName { get; }

        String ImageName { get; }

        void createItem(String path, EditorController editorController);

        EditInterface EditInterface { get; }
    }
}
