using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller.Exam;

namespace Medical.GUI
{
    class EndParagraphEditor : AnalysisEditorComponent
    {
        public EndParagraphEditor(AnalysisEditorComponentParent parent)
            : base("Medical.GUI.TextAnalysisEditor.EndParagraphEditor.layout", parent)
        {
            
        }
    }
}
