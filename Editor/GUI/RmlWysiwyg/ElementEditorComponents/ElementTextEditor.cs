using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class ElementTextEditor : Component, ElementEditorComponent
    {
        private EditBox text;
        private RmlElementEditor parentEditor;

        public ElementTextEditor()
            : base("Medical.GUI.RmlWysiwyg.ElementEditorComponents.ElementTextEditor.layout")
        {
            text = (EditBox)widget;
            InputManager.Instance.setKeyFocusWidget(text);
            Name = "Text";
        }

        public void attachToParent(RmlElementEditor parentEditor, Widget parent)
        {
            IntCoord clientCoord = parent.ClientCoord;
            text.setSize(clientCoord.width, clientCoord.height);
            text.Align = Align.Stretch;
            text.KeyButtonReleased += text_KeyButtonReleased;
            text.attachToWidget(parent);
            this.parentEditor = parentEditor;
            InputManager.Instance.setKeyFocusWidget(text);
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

        public String Name { get; set; }

        void text_KeyButtonReleased(Widget source, EventArgs e)
        {
            KeyEventArgs ke = (KeyEventArgs)e;
            if (ke.Key == Engine.Platform.KeyboardButtonCode.KC_RETURN && InputManager.Instance.isControlPressed())
            {
                parentEditor.hide();
            }
        }
    }
}
