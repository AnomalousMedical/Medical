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
        private Element dragElement;
        private IntVector2 dragMouseStartPosition;
        bool firstDrag = false;
        ImageBox dragIconPreview;
        RmlWysiwygComponent rmlComponent;

        private String insertRml;

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

        public void dragStart(IntVector2 position, Element dragElement)
        {
            this.dragElement = dragElement;
            dragMouseStartPosition = position;
            firstDrag = true;
            if (dragElement != null)
            {
                insertRml = dragElement.ElementRml;
            }
        }

        public void dragging(IntVector2 position)
        {
            if (firstDrag)
            {
                firstDrag = false;
            }
            dragIconPreview.setPosition(position.x - (dragIconPreview.Width / 2), position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - position.x) > 5 || Math.Abs(dragMouseStartPosition.y - position.y) > 5))
            {
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(CommonResources.NoIcon);
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }
            if (IsDragging)
            {
                if (dragElement != null)
                {
                    IntVector2 localCoord = rmlComponent.localCoord(position);
                    if (localCoord.x < 0 || localCoord.y < 0 || localCoord.x > dragElement.OffsetWidth || localCoord.y > dragElement.OffsetHeight)
                    {
                        Element parent = dragElement.ParentNode;
                        if (parent != null)
                        {
                            parent.RemoveChild(dragElement);
                        }
                        rmlComponent.changeSelectedElement(position, insertRml, "div");
                        dragElement = null;
                    }
                }
                else
                {
                    rmlComponent.changeSelectedElement(position, insertRml, "div");
                }
            }
        }

        public void dragEnded(IntVector2 position)
        {
            if (IsDragging)
            {
                dragIconPreview.Visible = false;
                rmlComponent.insertRml(insertRml);
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
