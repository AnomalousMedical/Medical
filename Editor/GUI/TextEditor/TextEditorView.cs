using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI
{
    class TextEditorView : MyGUIView
    {
        public TextEditorView(String name, String text = "", uint maxLength = 100000, bool wordWrap = false)
            :base(name)
        {
            this.Text = text;
            this.MaxLength = maxLength;
            this.WordWrap = wordWrap;
        }

        public String Text { get; set; }

        public uint MaxLength { get; set; }

        public bool WordWrap { get; set; }

        protected TextEditorView(LoadInfo info)
            : base(info)
        {

        }
    }
}
