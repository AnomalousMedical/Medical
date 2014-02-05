using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// A bit of description for an item that can be added and removed from a document easily.
    /// </summary>
    public class WysiwygDragDropItem : DragAndDropItem
    {
        private String markup;

        public WysiwygDragDropItem(String name, String icon, String markup, String previewTagType = "span")
            :base(name, icon)
        {
            this.markup = markup;
            this.PreviewTagType = previewTagType;
        }

        /// <summary>
        /// This is the markup that will be used while the element is being dragged onto the document.
        /// It should correlate resonably well to the createDocumentMarkup, and in this base class
        /// these two are actually one in the same.
        /// </summary>
        public String PreviewMarkup
        {
            get
            {
                return markup;
            }
        }

        /// <summary>
        /// This function will be called when the item is being created for real and added to the document.
        /// Any one time configuration can be done at this step.
        /// </summary>
        /// <returns></returns>
        public virtual String createDocumentMarkup()
        {
            return markup;
        }

        /// <summary>
        /// The tag type to use for previews.
        /// </summary>
        public String PreviewTagType { get; set; }
    }
}
