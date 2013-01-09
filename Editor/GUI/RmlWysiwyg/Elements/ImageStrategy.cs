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
            RmlElementEditor editor = RmlElementEditor.openTextEditor(element, (int)(element.AbsoluteLeft + element.ClientWidth) + left, (int)element.AbsoluteTop + top);
            editor.addElementEditor(new ElementAttributeEditor(element, uiCallback, browserProvider));
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String src = element.GetAttributeString("src");
            if (String.IsNullOrEmpty(src))
            {
                component.deleteElement(element);
            }
            //if (!Core.GetFileInterface().Exists(src, element.OwnerDocument.SourceURL))
            //{
            //    element.SetAttribute("src", RmlWysiwygComponent.DefaultImage);
            //}
            return true;
        }
    }
}
