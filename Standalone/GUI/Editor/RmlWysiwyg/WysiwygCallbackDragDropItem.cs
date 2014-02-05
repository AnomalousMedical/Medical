using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class WysiwygCallbackDragDropItem : WysiwygDragDropItem
    {
        private Func<String> markupCallback;

        public WysiwygCallbackDragDropItem(Func<String> markupCallback, String name = null, String icon = null, String previewTagType = "span")
            :base(name, icon)
        {
            this.markupCallback = markupCallback;
            this.PreviewTagType = previewTagType;
        }

        public override String Markup
        {
            get
            {
                return markupCallback();
            }
        }
    }
}
