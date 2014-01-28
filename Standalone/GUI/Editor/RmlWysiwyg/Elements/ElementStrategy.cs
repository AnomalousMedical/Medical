using Engine;
using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public enum ResizeType
    {
        None = 0,
        Width = 1 << 0,
        Height = 1 << 1,
        Left = 1 << 2,
        Top = 1 << 3,
        WidthHeight = Width | Height,
        LeftTop = Left | Top,
        LeftHeight = Left | Height,
        TopWidth = Top | Width,
    }

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

        public ResizeType ResizeHandles { get; set; }

        public virtual void changeSizePreview(Element element, IntRect newRect, ResizeType resizeType, IntSize2 bounds)
        {
            element.SetProperty("width", newRect.Width.ToString() + "sp");
            element.SetProperty("height", newRect.Height.ToString() + "sp");
            element.SetProperty("margin-left", newRect.Left.ToString() + "sp");
            element.SetProperty("margin-top", newRect.Top.ToString() + "sp");
        }

        public virtual void applySizeChange(Element element)
        {
            
        }

        /// <summary>
        /// Get the raw (unscaled) starting rect size for this element.
        /// </summary>
        /// <param name="selectedElement"></param>
        /// <returns></returns>
        public virtual Rect getStartingRect(Element selectedElement)
        {
            Variant vLeft = selectedElement.GetPropertyVariant("margin-left");
            Variant vTop = selectedElement.GetPropertyVariant("margin-top");
            Variant vWidth = selectedElement.GetPropertyVariant("width");
            Variant vHeight = selectedElement.GetPropertyVariant("height");
            float left = vLeft != null ? vLeft.FloatValue : 0;
            float top = vTop != null ? vTop.FloatValue : 0;
            float width = vWidth != null ? vWidth.FloatValue : 0;
            float height = vHeight != null ? vHeight.FloatValue : 0;
            return new Rect(left, top, width, height);
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
