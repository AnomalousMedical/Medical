using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Anomalous.GuiFramework.Editor;

namespace Medical
{
    public interface ProjectItemTemplate : AddItemTemplate
    {
        void createItem(String path, EditorController editorController);
    }
}
