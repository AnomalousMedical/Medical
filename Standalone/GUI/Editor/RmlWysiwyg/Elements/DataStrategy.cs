using Anomalous.GuiFramework.Editor;
using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class DataStrategy : ElementStrategy
    {
        private ElementAttributeEditor attributeEditor;

        public DataStrategy(String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            attributeEditor = new ElementAttributeEditor(element, uiCallback);
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(attributeEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            attributeEditor.applyToElement(element);
            return true;
        }
    }
}
