using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ElementStrategy
    {
        public ElementStrategy(String tagName, bool allowDragAndDrop = false)
        {
            this.TagName = tagName;
            this.AllowDragAndDrop = allowDragAndDrop;
        }

        public String TagName { get; set; }
         
        public bool AllowDragAndDrop { get; set; }

        public virtual RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            return null;
        }

        public virtual bool shouldDelete(Element element)
        {
            return false;
        }
    }
}
