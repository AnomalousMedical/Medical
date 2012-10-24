using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.GUI
{
    class TextEditorView : MyGUIView
    {
        public event Action<TextEditorView, TextEditorComponent> ComponentCreated;
        public delegate String TextProviderDelegate();
        private TextProviderDelegate textProvider;

        public TextEditorView(String name, TextProviderDelegate textProvider, uint maxLength = 100000, bool wordWrap = false, TextHighlighter textHighlighter = null)
            :base(name)
        {
            this.textProvider = textProvider;
            this.MaxLength = maxLength;
            this.WordWrap = wordWrap;
            this.TextHighlighter = textHighlighter;
        }

        public String Text
        {
            get
            {
                return textProvider();
            }
        }

        public uint MaxLength { get; set; }

        public bool WordWrap { get; set; }

        public TextHighlighter TextHighlighter { get; set; }

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
