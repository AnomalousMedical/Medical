using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class PreviewElement : IDisposable
    {
        private Element previewElement = null;

        public PreviewElement()
        {
            
        }

        public void Dispose()
        {
            hidePreviewElement();
            if (previewElement != null)
            {
                previewElement.removeReference();
                previewElement = null;
            }
        }

        public void hidePreviewElement()
        {
            if (previewElement != null)
            {
                Element previewParent = previewElement.ParentNode;
                if (previewParent != null)
                {
                    previewParent.RemoveChild(previewElement);
                }
            }
        }

        public void showPreviewElement(ElementDocument document, String innerRmlHint, Element parent, Element sibling, String previewElementTagType)
        {
            if (previewElement == null)
            {
                previewElement = document.CreateElement(previewElementTagType);
            }
            
            previewElement.InnerRml = innerRmlHint;

            if (sibling == null)
            {
                parent.AppendChild(previewElement);
            }
            else
            {
                parent.InsertAfter(previewElement, sibling);
            }
        }

        public bool isPreviewOrAncestor(Element element)
        {
            bool isNotPreview = true;
            if (previewElement != null)
            {
                Element parentWalker = element;
                while (parentWalker != null && isNotPreview)
                {
                    isNotPreview = parentWalker != previewElement;
                    parentWalker = parentWalker.ParentNode;
                }
            }
            return !isNotPreview;
        }

        /// <summary>
        /// This gets a good element to show as the highlight.
        /// </summary>
        public Element HighlightPreviewElement
        {
            get
            {
                if (previewElement != null)
                {
                    if (previewElement.NumChildren == 1)
                    {
                        return previewElement.GetChild(0);
                    }
                }
                return previewElement;
            }
        }
    }
}
