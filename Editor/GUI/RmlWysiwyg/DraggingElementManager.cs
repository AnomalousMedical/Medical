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
        private const String eraseIcon = CommonResources.NoIcon;

        private Element dragElement;
        private IntVector2 dragMouseStartPosition;
        bool firstDrag = false;
        ImageBox dragIconPreview;
        RmlWysiwygComponent rmlComponent;

        private String insertRml;
        private String undoRml = null;
        private String iconName;
        private bool allowDragging = false;

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
                        if (localCoord.x < 0 || localCoord.y < 0 || localCoord.x > dragElement.OffsetWidth || localCoord.y > dragElement.OffsetHeight)
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
                        rmlComponent.setPreviewElement(position, insertRml, "div");
                    }
                    if (rmlComponent.contains(position))
                    {
                        dragIconPreview.setItemResource(iconName);
                    }
                    else
                    {
                        dragIconPreview.setItemResource(eraseIcon);
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
