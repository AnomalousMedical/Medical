﻿using libRocketPlugin;
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
        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            float width = -1;
            String sizeAttrString = element.GetAttributeString("width");
            if (sizeAttrString != null && sizeAttrString.Length > 2)
            {
                sizeAttrString = sizeAttrString.Substring(0, sizeAttrString.Length - 2);
                if (!float.TryParse(sizeAttrString, out width))
                {
                    width = -1;
                }
            }
            slideImageEditor = new SlideImageComponent(editorResourceProvider, subdirectory, element.GetAttributeString("src"), width);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, applyChanges, delete);
            editor.addElementEditor(slideImageEditor);
            return editor;
        }

        private bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            return slideImageEditor.applyToElement(element);
        }

        private bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
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
