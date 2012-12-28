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

        public void showPreviewElement(ElementDocument document, String innerRmlHint, Element parent, Element sibling)
        {
            if (previewElement == null)
            {
                previewElement = document.CreateElement("div");
                previewElement.SetAttribute("style", "border-width: 3px; border-color: red; display: block;");
            }
            
            previewElement.InnerRml = innerRmlHint;

            if (sibling == null)
            {
                parent.AppendChild(previewElement);
            }
            else
            {
                parent.InsertBefore(previewElement, sibling);
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
    }
}
