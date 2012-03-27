using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TextAnalysisViewer : Dialog
    {
        EditBox textArea;

        public TextAnalysisViewer(String caption, String text)
            :base("Medical.GUI.ExamViewer.TextAnalysisViewer.layout")
        {
            window.Caption = caption;

            textArea = (EditBox)window.findWidget("TextArea");
            textArea.MaxTextLength = (uint)text.Length;
            textArea.Caption = text;
        }

        protected override void onClosed(EventArgs args)
        {
            base.onClosed(args);
            Dispose();
        }
    }
}
