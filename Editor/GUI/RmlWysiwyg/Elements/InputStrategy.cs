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
        public InputStrategy(String tag, String previewIconName = "Editor/ButtonIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            ElementTextEditor textEditor = new ElementTextEditor(element.InnerRml);
            ElementAttributeEditor attributeEditor = new ElementAttributeEditor(element, uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openTextEditor(element, (int)(element.AbsoluteLeft + element.ClientWidth) + left, (int)element.AbsoluteTop + top,
                (updateElement, elementEditor, component) =>
                {
                    String text = textEditor.Text;
                    element.InnerRml = textEditor.Text;
                    attributeEditor.applyToElement(element);
                    return true;
                });
            editor.addElementEditor(textEditor);
            editor.addElementEditor(attributeEditor);
            return editor;
        }
    }
}
