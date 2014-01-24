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
        ElementStyleEditor elementStyleEditor;
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
            slideImageEditor = new SlideImageComponent(editorResourceProvider, subdirectory, element.GetAttributeString("src"));
            elementStyleEditor = new ElementStyleEditor(element, uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(slideImageEditor);
            editor.addElementEditor(elementStyleEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            element.ClearLocalStyles();
            StringBuilder styleString = new StringBuilder();
            bool changesMade = elementStyleEditor.buildStyleString(styleString);
            changesMade = slideImageEditor.applyToElement(element) | changesMade;
            if (changesMade)
            {
                if (styleString.Length > 0)
                {
                    element.SetAttribute("style", styleString.ToString());
                }
                else
                {
                    element.RemoveAttribute("style");
                }
            }
            return changesMade;
        }

        public override void changeSizePreview(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            //slideImageEditor.changeSize(newRect, resizeType, bounds);
            //IntSize2 correctedSize = slideImageEditor.ImageSize;
            //element.SetAttribute("width", correctedSize.Width.ToString());
            //element.SetAttribute("height", correctedSize.Height.ToString());
            //IntVector2 position = slideImageEditor.ImagePosition;
            //element.SetProperty("margin-left", position.x.ToString() + "sp");
            //element.SetProperty("margin-top", position.y.ToString() + "sp");
        }

        public override Rect getStartingRect(Element selectedElement)
        {
            Variant vLeft = selectedElement.GetPropertyVariant("margin-left");
            Variant vTop = selectedElement.GetPropertyVariant("margin-top");
            Variant vWidth = selectedElement.GetAttribute("width");
            Variant vHeight = selectedElement.GetAttribute("height");
            float left = vLeft != null ? vLeft.FloatValue : 0;
            float top = vTop != null ? vTop.FloatValue : 0;
            float width = vWidth != null ? vWidth.FloatValue : 0;
            float height = vHeight != null ? vHeight.FloatValue : 0;
            return new Rect(left, top, width, height);
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
