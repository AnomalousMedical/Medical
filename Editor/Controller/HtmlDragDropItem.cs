using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class HtmlDragDropItem : DragAndDropItem
    {
        public HtmlDragDropItem(String name = null, String icon = null, String markup = null)
            :base(name, icon)
        {
            this.Markup = markup;
        }

        public String Markup { get; set; }
    }
}
