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
            
        }

        public virtual void applySizeChange(Element element)
        {
            
        }

        /// <summary>
        /// Get the raw (unscaled) starting rect size for this element.
        /// </summary>
        /// <param name="selectedElement"></param>
        /// <returns></returns>
        public virtual Rect getStartingRect(Element selectedElement, out bool leftAnchor)
        {
            leftAnchor = true;
            return new Rect();
        }

        public virtual RmlElementEditor openEditor(Element element, MedicalUICallback uiCallback, int left, int top)
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

        /// <summary>
        /// Get a highlight provider for the passed element. If you want to customize the highlight
        /// do it here. It is valid to return null, which means you wish to use whatever default provider
        /// exists.
        /// </summary>
        public virtual HighlightProvider HighlightProvider
        {
            get
            {
                return null;
            }
        }
    }
}
