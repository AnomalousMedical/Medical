using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class provides a callback when calling the createDocumentMarkup function. This
    /// allows for interesting cases for rml manipulation before adding it to the document.
    /// </summary>
    public class WysiwygCallbackDragDropItem : WysiwygDragDropItem
    {
        private Func<String> markupCallback;

        public WysiwygCallbackDragDropItem(String name, String icon, String markup, Func<String> markupCallback, String previewTagType = "span")
            :base(name, icon, markup, previewTagType)
        {
            this.markupCallback = markupCallback;
            this.PreviewTagType = previewTagType;
        }

        /// <summary>
        /// Calls the markupCallback function.
        /// </summary>
        /// <returns></returns>
        public override String createDocumentMarkup()
        {
            return markupCallback();
        }
    }
}
