﻿using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class TextElementStrategy : ElementStrategy
    {
        private ElementTextEditor textEditor;
        private ElementAttributeEditor attributeEditor;

        public TextElementStrategy(String tag, String previewIconName = "Editor/HeaderIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            String rml = element.InnerRml;
            if (rml != null)
            {
                rml = rml.Replace("<br />", "\n");
            }
            textEditor = new ElementTextEditor(rml);
            attributeEditor = new ElementAttributeEditor(element, uiCallback, browserProvider);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, applyChanges, delete);
            editor.addElementEditor(textEditor);
            editor.addElementEditor(attributeEditor);
            return editor;
        }

        private bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = textEditor.Text;
            element.InnerRml = text.Replace("\n", "<br />");
            attributeEditor.applyToElement(element);
            return true;
        }

        private bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            String text = element.InnerRml;
            if (String.IsNullOrEmpty(text))
            {
                component.deleteElement(element);
                return true;
            }
            return false;
        }
    }
}
