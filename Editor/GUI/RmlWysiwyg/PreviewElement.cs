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
        }

        public void hidePreviewElement()
        {
            if (previewElement != null)
            {
                Element previewParent = previewElement.ParentNode;
                if (previewParent != null)
                {
                    Logging.Log.Debug("Preview Element before hide {0}", previewElement.ReferenceCount);
                    previewParent.RemoveChild(previewElement);
                    previewElement.removeReference(); //wtf this is leaking???
                    Logging.Log.Debug("Preview Element after hide {0}", previewElement.ReferenceCount);
                    previewElement = null;
                }
            }
        }

        public void showPreviewElement(ElementDocument document, String innerRmlHint, Element parent, Element sibling)
        {
            previewElement = document.CreateElement("div");
            String style = "border-width: 3px; border-color: red; display: block;";
            if (innerRmlHint != null)
            {
                previewElement.InnerRml = innerRmlHint;
            }
            else
            {
                style += "height: 25px; width: 98%;";
            }
            previewElement.SetAttribute("style", style);

            Logging.Log.Debug("Preview Element created {0}", previewElement.ReferenceCount);
            
            if (sibling == null)
            {
                parent.AppendChild(previewElement);
            }
            else
            {
                parent.InsertBefore(previewElement, sibling);
            }

            Logging.Log.Debug("Preview Element shown {0}", previewElement.ReferenceCount);
            //previewElement.removeReference();
            Logging.Log.Debug("Preview Element localrefgone {0}", previewElement.ReferenceCount);
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
