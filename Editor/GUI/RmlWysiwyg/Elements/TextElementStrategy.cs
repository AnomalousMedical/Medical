using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class TextElementStrategy : ElementStrategy
    {
        public TextElementStrategy(String tag, String previewIconName = "Editor/HeaderIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            ElementTextEditor textEditor = new ElementTextEditor(element.InnerRml);
            ElementAttributeEditor attributeEditor = new ElementAttributeEditor(element, uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top,
                (updateElement, elementEditor, component) =>
                {
                    String text = textEditor.Text;
                    if (String.IsNullOrEmpty(text))
                    {
                        component.deleteElement(element);
                    }
                    else
                    {
                        element.InnerRml = textEditor.Text;
                        attributeEditor.applyToElement(element);
                    }
                    return true;
                });
            editor.addElementEditor(textEditor);
            editor.addElementEditor(attributeEditor);
            return editor;
        }
    }
}
