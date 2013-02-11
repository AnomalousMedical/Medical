using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class ElementStrategy
    {
        public ElementStrategy(String tagName, String previewIconName = CommonResources.NoIcon, bool allowDragAndDrop = false)
        {
            this.TagName = tagName;
            this.AllowDragAndDrop = allowDragAndDrop;
            this.PreviewIconName = previewIconName;
        }

        public String TagName { get; set; }
         
        public bool AllowDragAndDrop { get; set; }

        public String PreviewIconName { get; set; }

        public virtual RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            return null;
        }
    }
}
