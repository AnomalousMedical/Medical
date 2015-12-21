using Anomalous.GuiFramework.Editor;
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

        public InputStrategy(String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            switch (element.GetAttributeString("type"))
            {
                case "range":
                    attributeEditor = new ElementAttributeEditor(element, uiCallback);
                    editor.addElementEditor(attributeEditor);
                    break;
                default:
                    textEditor = new ElementTextEditor(element.InnerRml);
                    attributeEditor = new ElementAttributeEditor(element, uiCallback);
                    editor.addElementEditor(textEditor);
                    editor.addElementEditor(attributeEditor);
                    break;
            }
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            switch (element.GetAttributeString("type"))
            {
                case "range":
                    attributeEditor.applyToElement(element);
                    break;
                default:
                    String text = textEditor.Text;
                    element.InnerRml = textEditor.Text;
                    attributeEditor.applyToElement(element);
                    break;
            }

            
            return true;
        }
    }
}
