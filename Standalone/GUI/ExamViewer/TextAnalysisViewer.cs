using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TextAnalysisViewer : Dialog
    {
        Edit textArea;

        public TextAnalysisViewer(String caption, String text)
            :base("Medical.GUI.ExamViewer.TextAnalysisViewer.layout")
        {
            window.Caption = caption;

            textArea = (Edit)window.findWidget("TextArea");
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
