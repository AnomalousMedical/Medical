using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class ElementTextEditor : ElementEditorComponent
    {
        private EditBox text;

        public ElementTextEditor()
            : base("Medical.GUI.RmlWysiwyg.ElementEditorComponents.ElementTextEditor.layout", "Text")
        {
            text = (EditBox)widget;
            InputManager.Instance.setKeyFocusWidget(text);
        }

        public override void attachToParent(RmlElementEditor parentEditor, Widget parent)
        {
            base.attachToParent(parentEditor, parent);
            text.KeyButtonReleased += text_KeyButtonReleased;
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
