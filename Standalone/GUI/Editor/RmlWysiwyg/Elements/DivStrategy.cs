using Anomalous.GuiFramework.Editor;
using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class DivStrategy : ElementStrategy
    {
        public DivStrategy(String tag, String previewIconName = "Editor/ButtonIcon")
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            return null;
        }
    }
}
