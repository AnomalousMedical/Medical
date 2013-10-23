using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class DraggingElementManager : IDisposable
    {
        public event RmlWysiwygComponent.ElementOffDocumentDelegate ElementDraggedOffDocument;
        public event RmlWysiwygComponent.ElementOffDocumentDelegate ElementDroppedOffDocument;
        public event RmlWysiwygComponent.ElementOffDocumentDelegate ElementReturnedToDocument;

        private Element dragElement;
        private IntVector2 dragMouseStartPosition;
        bool firstDrag = false;
        ImageBox dragIconPreview;
        RmlWysiwygComponent rmlComponent;

        private String insertRml;
        private String undoRml = null;
        private String iconName;
        private bool allowDragging = false;
        private bool offDocument = false;

        public DraggingElementManager(RmlWysiwygComponent rmlComponent)
        {
            this.rmlComponent = rmlComponent;
            this.dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, 32, 32, Align.Default, "Info", "");
            dragIconPreview.Visible = false;
        }

        public virtual void Dispose()
        {
            Gui.Instance.destroyWidget(dragIconPreview);
        }

        public void dragStart(IntVector2 position, Element dragElement, String iconName)
        {
            this.dragElement = dragElement;
            dragMouseStartPosition = position;
            firstDrag = true;
            insertRml = null;
            undoRml = null;
            allowDragging = true;
            this.iconName = iconName;
        }

        public void dragging(IntVector2 position)
        {
            if (allowDragging)
            {
                if (firstDrag)
                {
                    firstDrag = false;
                }
                dragIconPreview.setPosition(position.x - (dragIconPreview.Width / 2), position.y - (int)(dragIconPreview.Height * .75f));
                if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - position.x) > 5 || Math.Abs(dragMouseStartPosition.y - position.y) > 5))
                {
                    dragIconPreview.Visible = true;
                    dragIconPreview.setItemResource(iconName);
                    LayerManager.Instance.upLayerItem(dragIconPreview);
                }
                if (IsDragging)
                {
                    if (dragElement != null)
                    {
                        IntVector2 localCoord = rmlComponent.localCoord(position);
                        if (localCoord.x < dragElement.AbsoluteLeft || localCoord.y < dragElement.AbsoluteTop || localCoord.x > dragElement.OffsetWidth + dragElement.AbsoluteLeft || localCoord.y > dragElement.OffsetHeight + dragElement.AbsoluteTop)
                        {
                            insertRml = dragElement.ElementRml;
                            undoRml = rmlComponent.UnformattedRml;
                            rmlComponent.setPreviewElement(position, insertRml, "div");
                            Element parent = dragElement.ParentNode;
                            if (parent != null)
                            {
                                parent.RemoveChild(dragElement);
                            }
                            dragElement = null;
                        }
                    }
                    else
                    {
                        if (!rmlComponent.setPreviewElement(position, insertRml, "div"))
                        {
                            offDocument = true;
                            if (ElementDraggedOffDocument != null)
                            {
                                ElementDraggedOffDocument.Invoke(rmlComponent, position, insertRml, "div");
                            }
                        }
                        else if (offDocument)
                        {
                            offDocument = false;
                            if (ElementReturnedToDocument != null)
                            {
                                ElementReturnedToDocument.Invoke(rmlComponent, position, insertRml, "div");
                            }
                        }
                    }
                }
            }
        }

        public void dragEnded(IntVector2 position)
        {
            if (allowDragging)
            {
                dragIconPreview.Visible = false;
                if (dragElement == null)
                {
                    if (rmlComponent.contains(position))
                    {
                        rmlComponent.insertRml(insertRml, undoRml);
                    }
                    else
                    {
                        if (ElementDroppedOffDocument != null)
                        {
                            ElementDroppedOffDocument.Invoke(rmlComponent, position, insertRml, "div");
                        }
                        //This is effectively a delete, so save undo status
                        rmlComponent.updateUndoStatus(undoRml);
                        rmlComponent.clearPreviewElement();
                    }
                }
                else
                {
                    dragElement = null;
                    rmlComponent.clearPreviewElement();
                }
                allowDragging = false;
            }
        }

        public bool IsDragging
        {
            get
            {
                return dragIconPreview.Visible;
            }
        }
    }
}
