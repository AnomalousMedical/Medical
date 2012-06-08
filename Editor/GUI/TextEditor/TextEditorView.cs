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
        public event Action<TextEditorView, TextEditorComponent> ComponentCreated;

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

        /// <summary>
        /// This is used by the factory to fire when one of these components has
        /// been created. DO NOT call from anywhere else.
        /// </summary>
        /// <param name="component">The TextEditorComponent that was created.</param>
        internal void _fireComponentCreated(TextEditorComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        protected TextEditorView(LoadInfo info)
            : base(info)
        {

        }
    }
}
