using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class HtmlDragDropItem : DragAndDropItem
    {
        public HtmlDragDropItem(String name = null, String icon = null, String markup = null, String previewTagType = "span")
            :base(name, icon)
        {
            this.Markup = markup;
            this.PreviewTagType = previewTagType;
        }

        public String Markup { get; set; }

        public String PreviewTagType { get; set; }
    }
}
