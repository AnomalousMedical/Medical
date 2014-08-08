using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class InputStrategy : ElementStrategy
    {
        private ElementTextEditor textEditor;
        private ElementAttributeEditor attributeEditor;

        public InputStrategy(String tag, String previewIconName = "Editor/ButtonIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, int left, int top)
        {
            textEditor = new ElementTextEditor(element.InnerRml);
            attributeEditor = new ElementAttributeEditor(element, uiCallback);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(attributeEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = textEditor.Text;
            element.InnerRml = textEditor.Text;
            attributeEditor.applyToElement(element);
            return true;
        }
    }
}
