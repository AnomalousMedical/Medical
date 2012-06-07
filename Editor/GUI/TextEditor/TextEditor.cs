using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TextEditor : MDIDialog
    {
        /// <summary>
        /// Open a text editor that disposes when it is closed.
        /// </summary>
        /// <returns></returns>
        public static TextEditor openTextEditor(GUIManager guiManager)
        {
            TextEditor textEditor = new TextEditor();
            guiManager.addManagedDialog(textEditor);

            textEditor.Closed += (sender, e) =>
            {
                guiManager.removeManagedDialog(textEditor);
                textEditor.Dispose();
            };

            textEditor.Visible = true;

            return textEditor;
        }

        private EditBox text;

        protected TextEditor()
            :base("Medical.GUI.TextEditor.TextEditor.layout")
        {
            text = (EditBox)window.findWidget("Text");
            Serialize = false;
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

        public String Caption
        {
            get
            {
                return window.Caption;
            }
            set
            {
                window.Caption = value;
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

        public String CurrentFile { get; set; }
    }
}
