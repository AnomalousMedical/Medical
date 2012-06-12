﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    class TextEditorComponent : LayoutComponent
    {
        private EditBox text;

        public TextEditorComponent(MyGUIViewHost viewHost, TextEditorView view)
            : base("Medical.GUI.TextEditor.TextEditorComponent.layout", viewHost)
        {
            text = (EditBox)widget.findWidget("Text");

            Text = view.Text;
            MaxLength = view.MaxLength;
            WordWrap = view.WordWrap;
        }

        public void cut()
        {
            text.cut();
        }

        public void copy()
        {
            text.copy();
        }

        public void paste()
        {
            text.paste();
        }

        public void selectAll()
        {
            text.setTextSelection(0, uint.MaxValue);
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