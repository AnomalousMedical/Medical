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
        EditInterfaceEditor appearanceEditor;
        ElementStyleDefinition appearance;
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
            appearance = new ImageElementStyle(element);
            appearance.Changed += appearance_Changed;
            appearanceEditor = new EditInterfaceEditor("Appearance", appearance.getEditInterface(), uiCallback, browserProvider);
            slideImageEditor = new SlideImageComponent(editorResourceProvider, subdirectory, element.GetAttributeString("src"));
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(slideImageEditor);
            editor.addElementEditor(appearanceEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            element.ClearLocalStyles();
            StringBuilder styleString = new StringBuilder();
            StringBuilder classString = new StringBuilder();
            bool changesMade = slideImageEditor.applyToElement(element);
            changesMade = appearance.buildClassList(classString) | changesMade;
            changesMade = appearance.buildStyleAttribute(styleString) | changesMade;
            if (changesMade)
            {
                if (classString.Length > 0)
                {
                    element.SetAttribute("class", classString.ToString());
                }
                else
                {
                    element.RemoveAttribute("class");
                }
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

        void appearance_Changed(ElementStyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }
    }
}
