using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class RmlElementEditor : PopupContainer
    {
        /// <summary>
        /// Open a text editor that disposes when it is closed.
        /// </summary>
        /// <returns></returns>
        public static RmlElementEditor openTextEditor(int left, int top)
        {
            RmlElementEditor textEditor = new RmlElementEditor();
            textEditor.show(left, top);
            textEditor.Hidden += (source, e) =>
            {
                ((RmlElementEditor)source).Dispose();
            };
            return textEditor;
        }

        private EditBox text;

        protected RmlElementEditor()
            :base("Medical.GUI.RmlWysiwyg.RmlElementEditor.layout")
        {
            text = (EditBox)widget.findWidget("Text");
        }

        public String Text
        {
            get
            {
                return text.OnlyText;
            }
            set
            {
                text.OnlyText = value.Replace("\r", "");            
            }
        }

        public uint MaxLength
        {
            get
            {
                return text.MaxTextLength;
            }
            set
            {
                text.MaxTextLength = value;
            }
        }

        public bool WordWrap
        {
            get
            {
                return text.EditWordWrap;
            }
            set
            {
                text.EditWordWrap = value;
            }
        }
    }
}
