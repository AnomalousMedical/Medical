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
            RmlElementEditor editor = RmlElementEditor.openTextEditor(uiCallback, browserProvider, element, (int)(element.AbsoluteLeft + element.ClientWidth) + left, (int)element.AbsoluteTop + top);
            editor.addElementEditor(new ElementTextEditor() { Text = element.InnerRml });
            return editor;
        }

        /// <summary>
        /// Apply the changes from an editor to an element. Return true if changes are made.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="editor"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            //String text = editor.Text;
            //if (String.IsNullOrEmpty(text))
            //{
            //    component.deleteElement(element);
            //}
            //else
            //{
            //    element.InnerRml = editor.Text;
            //}
            //return true;
            return false;
        }
    }
}
