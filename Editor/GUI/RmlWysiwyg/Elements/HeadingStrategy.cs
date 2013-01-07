using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class HeadingStrategy : ElementStrategy
    {
        public HeadingStrategy(String tag, String previewIconName = "Editor/HeaderIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            RmlElementEditor editor = RmlElementEditor.openTextEditor(uiCallback, browserProvider, element, (int)(element.AbsoluteLeft + element.ClientWidth) + left, (int)element.AbsoluteTop + top);
            return editor;
        }

        public override bool shouldDelete(Element element)
        {
            throw new NotImplementedException();
        }
    }
}
