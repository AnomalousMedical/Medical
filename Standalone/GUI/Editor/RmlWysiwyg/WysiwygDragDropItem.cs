using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class WysiwygDragDropItem : DragAndDropItem
    {
        private String markup;

        public WysiwygDragDropItem(String name = null, String icon = null, String markup = null, String previewTagType = "span")
            :base(name, icon)
        {
            this.markup = markup;
            this.PreviewTagType = previewTagType;
        }

        public virtual String Markup
        {
            get
            {
                return markup;
            }
        }

        public String PreviewTagType { get; set; }
    }
}
