using Engine;
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

        public bool Resizable { get; set; }

        public virtual void changeSizePreview(Element element, IntSize2 newSize)
        {

        }

        public virtual void applySizeChange(Element element)
        {

        }

        public virtual RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider, int left, int top)
        {
            return null;
        }

        public virtual bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            return false;
        }

        public virtual bool delete(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            return false;
        }
    }
}
