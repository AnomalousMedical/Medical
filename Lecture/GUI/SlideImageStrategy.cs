using Engine;
using libRocketPlugin;
using Medical;
using Medical.GUI;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideImageStrategy : ElementStrategy
    {
        SlideImageComponent slideImageEditor;
        EditorResourceProvider editorResourceProvider;
        String subdirectory;

        public SlideImageStrategy(String tag, EditorResourceProvider editorResourceProvider, String subdirectory, String previewIconName = "Editor/ImageIcon")
            : base(tag, previewIconName, true)
        {
            this.editorResourceProvider = editorResourceProvider;
            this.subdirectory = subdirectory;
            Resizable = true;
        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            IntSize2 imageSize = new IntSize2(-1, -1);
            Variant widthAttr = element.GetAttribute("width");
            if (widthAttr != null)
            {
                imageSize.Width = widthAttr.IntValue;
            }
            Variant heightAttr = element.GetAttribute("height");
            if (heightAttr != null)
            {
                imageSize.Height = heightAttr.IntValue;
            }
            slideImageEditor = new SlideImageComponent(editorResourceProvider, subdirectory, element.GetAttributeString("src"), imageSize);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(slideImageEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            return slideImageEditor.applyToElement(element);
        }

        public override void changeSizePreview(Element element, IntSize2 newSize)
        {
            slideImageEditor.changeSize(newSize);
            element.SetAttribute("width", newSize.Width.ToString());
            element.SetAttribute("height", newSize.Height.ToString());
        }

        public override void applySizeChange(Element element)
        {
            slideImageEditor.applyChanges();
        }

        public override bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String src = element.GetAttributeString("src");
            if (String.IsNullOrEmpty(src))
            {
                component.deleteElement(element);
                return true;
            }
            return false;
        }
    }
}
