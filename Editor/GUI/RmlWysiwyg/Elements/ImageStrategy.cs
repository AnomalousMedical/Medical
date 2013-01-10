using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class ImageStrategy : ElementStrategy
    {
        public ImageStrategy(String tag, String previewIconName = "Editor/ImageIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            ElementAttributeEditor attributeEditor = new ElementAttributeEditor(element, uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top,
                (updateElement, elementEditor, component) =>
                {
                    attributeEditor.applyToElement(element);
                    String src = updateElement.GetAttributeString("src");
                    if (String.IsNullOrEmpty(src))
                    {
                        component.deleteElement(element);
                    }
                    return true;
                });
            editor.addElementEditor(attributeEditor);
            return editor;
        }
    }
}
