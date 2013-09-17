using Engine;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class DragAndDropComponent : LayoutComponent
    {
        private NoSelectButtonGrid dragItems;
        private ImageBox dragIconPreview;
        private IntVector2 dragMouseStartPosition;
        private DragAndDropViewBase view;
        private bool firstDrag;

        public DragAndDropComponent(MyGUIViewHost viewHost, DragAndDropViewBase view)
            :base("Medical.GUI.Editor.DragAndDrop.DragAndDropComponent.layout", viewHost)
        {
            dragItems = new NoSelectButtonGrid((ScrollView)widget);
            dragItems.ItemActivated += dragItems_ItemActivated;
            this.view = view;

            dragItems.SuppressLayout = true;
            foreach (DragAndDropItem item in view.BaseItems)
            {
                ButtonGridItem gridItem = dragItems.addItem("", item.Name, item.Icon);
                gridItem.UserObject = item;
                gridItem.MouseButtonPressed += gridItem_MouseButtonPressed;
                gridItem.MouseButtonReleased += gridItem_MouseButtonReleased;
                gridItem.MouseDrag += gridItem_MouseDrag;
            }
            dragItems.SuppressLayout = false;
            dragItems.layout();

            dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, ScaleHelper.Scaled(32), ScaleHelper.Scaled(32), Align.Default, "Info", view.Name + "DragAndDropPreview");
            dragIconPreview.Visible = false;
        }

        public override void Dispose()
        {
            dragItems.Dispose();
            Gui.Instance.destroyWidget(dragIconPreview);
            base.Dispose();
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            dragItems.resizeAndLayout(widget.Width);
        }

        void gridItem_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            dragMouseStartPosition = arg.Position;
            firstDrag = true;
        }

        void gridItem_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            if (firstDrag)
            {
                view._fireDragStarted((DragAndDropItem)source.UserObject, arg.Position);
                firstDrag = false;
            }
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
            {
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(((DragAndDropItem)source.UserObject).Icon);
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }
            view._fireDragging((DragAndDropItem)source.UserObject, arg.Position);
        }

        void gridItem_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.Visible = false;
            view._fireDragEnded((DragAndDropItem)source.UserObject, arg.Position);
        }

        void dragItems_ItemActivated(object sender, EventArgs e)
        {
            ButtonGridItem item = sender as ButtonGridItem;
            if(item != null)
            {
                view._fireItemActivated((DragAndDropItem)item.UserObject);
            }
        }
    }
}
