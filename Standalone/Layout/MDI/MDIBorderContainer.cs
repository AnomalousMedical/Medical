using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.Controller
{

    /// <summary>
    /// Creates a border layout. The top and bottom will fill the width and
    /// height of the screen and left, right and center will be sandwiched in
    /// between.
    /// </summary>
    public class MDIBorderContainer : MDIContainerBase, IDisposable
    {
        private static readonly int LocationPreviewSize = ScaleHelper.Scaled(25);

        private MDIBorderContainerDock left;
        private MDIBorderContainerDock right;
        private MDIBorderContainerDock top;
        private MDIBorderContainerDock bottom;
        private LayoutContainer center;
        private MDILayoutContainer documentArea;
        private FloatingWindowContainer floating;
        private LayoutContainer centerParentContainer = null;

        private MDIWindow dragSourceWindow;
        private MDIWindow dragTargetWindow;
        private MDIChildContainerBase dragTargetContainer;
        private WindowAlignment finalWindowAlignment = WindowAlignment.Right;

        private bool visible = true;

        private Widget windowTargetWidget;
        private Widget leftContainerWidget;
        private Widget rightContainerWidget;
        private Widget topContainerWidget;
        private Widget bottomContainerWidget;

        public MDIBorderContainer(int padding)
            :base(DockLocation.None)
        {
            windowTargetWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, 10, 10, Align.Left | Align.Top, "Info", "");
            windowTargetWidget.Visible = false;

            left = new MDIBorderContainerDock(new MDILayoutContainerAbsolute(MDILayoutContainer.LayoutType.Horizontal, padding, DockLocation.Left));
            left._setParent(this);
            leftContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, LocationPreviewSize, LocationPreviewSize, Align.Left | Align.Top, "Info", "");
            leftContainerWidget.Visible = false;

            right = new MDIBorderContainerDock(new MDILayoutContainerAbsolute(MDILayoutContainer.LayoutType.Horizontal, padding, DockLocation.Right));
            right._setParent(this);
            rightContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, LocationPreviewSize, LocationPreviewSize, Align.Left | Align.Top, "Info", "");
            rightContainerWidget.Visible = false;

            top = new MDIBorderContainerDock(new MDILayoutContainerAbsolute(MDILayoutContainer.LayoutType.Vertical, padding, DockLocation.Top));
            top._setParent(this);
            topContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, LocationPreviewSize, LocationPreviewSize, Align.Left | Align.Top, "Info", "");
            topContainerWidget.Visible = false;

            bottom = new MDIBorderContainerDock(new MDILayoutContainerAbsolute(MDILayoutContainer.LayoutType.Vertical, padding, DockLocation.Bottom));
            bottom._setParent(this);
            bottomContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, LocationPreviewSize, LocationPreviewSize, Align.Left | Align.Top, "Info", "");
            bottomContainerWidget.Visible = false;

            documentArea = new MDILayoutContainerScale(MDILayoutContainer.LayoutType.Horizontal, padding, DockLocation.Center);
            Center = documentArea;

            floating = new FloatingWindowContainer();
            floating._setParent(this);
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
            top.Dispose();
            bottom.Dispose();
            documentArea.Dispose();
            Gui.Instance.destroyWidget(windowTargetWidget);
            Gui.Instance.destroyWidget(leftContainerWidget);
            Gui.Instance.destroyWidget(rightContainerWidget);
            Gui.Instance.destroyWidget(topContainerWidget);
            Gui.Instance.destroyWidget(bottomContainerWidget);
        }

        public void windowDragStarted(MDIWindow source, float mouseX, float mouseY)
        {
            dragSourceWindow = source;
            dragTargetWindow = null;
            dragTargetContainer = null;

            if ((source.AllowedDockLocations & DockLocation.Left) != 0)
            {
                int x = (int)(left.DesiredSize.Width / 2 + Location.x);
                int y = (int)WorkingSize.Height / 2;
                leftContainerWidget.setPosition(x, y);
                leftContainerWidget.Visible = true;
            }

            if ((source.AllowedDockLocations & DockLocation.Right) != 0)
            {
                int x = (int)(right.DesiredSize.Width / 2 + right.Location.x - rightContainerWidget.Width);
                int y = (int)WorkingSize.Height / 2;
                rightContainerWidget.setPosition(x, y);
                rightContainerWidget.Visible = true;
            }

            if ((source.AllowedDockLocations & DockLocation.Top) != 0)
            {
                int x = (int)WorkingSize.Width / 2;
                int y = (int)(top.DesiredSize.Height / 2 + Location.y);
                topContainerWidget.setPosition(x, y);
                topContainerWidget.Visible = true;
            }

            if ((source.AllowedDockLocations & DockLocation.Bottom) != 0)
            {
                int x = (int)WorkingSize.Width / 2;
                int y = (int)(bottom.DesiredSize.Width / 2 + bottom.Location.y - bottomContainerWidget.Width);
                bottomContainerWidget.setPosition(x, y);
                bottomContainerWidget.Visible = true;
            }
        }

        public void windowDragged(MDIWindow source, float mouseX, float mouseY)
        {
            //If we are currently docked and are allowed to float, start floating immediatly on movement.
            if (((source.AllowedDockLocations & DockLocation.Floating) != 0) &&
                (source.CurrentDockLocation == DockLocation.Left ||
                source.CurrentDockLocation == DockLocation.Right ||
                source.CurrentDockLocation == DockLocation.Top ||
                source.CurrentDockLocation == DockLocation.Bottom))
            {
                source._ParentContainer.removeChild(source);
                floating.addChild(source);
                this.layout();
            }

            if (checkContainerWidget(leftContainerWidget, left, source, DockLocation.Left, mouseX, mouseY) ||
            checkContainerWidget(rightContainerWidget, right, source, DockLocation.Right, mouseX, mouseY) ||
            checkContainerWidget(topContainerWidget, top, source, DockLocation.Top, mouseX, mouseY) ||
            checkContainerWidget(bottomContainerWidget, bottom, source, DockLocation.Bottom, mouseX, mouseY) ||
            processFloating(source, mouseX, mouseY) || 
            processCenter(source, mouseX, mouseY))
            {

            }
        }

        private bool checkContainerWidget(Widget widget, MDIChildContainerBase targetContainer, MDIWindow window, DockLocation dockLocation, float x, float y)
        {
            if (widget.Visible)
            {
                float left = widget.AbsoluteLeft;
                float top = widget.AbsoluteTop;
                float right = left + widget.Width;
                float bottom = top + widget.Height;
                if (x > left && x < right && y > top && y < bottom)
                {
                    dragTargetContainer = targetContainer;
                    dragTargetWindow = null;

                    windowTargetWidget.Visible = true;
                    switch (dockLocation)
                    {
                        case DockLocation.Left:
                            windowTargetWidget.setCoord((int)Location.x, (int)Location.y, (int)window.DesiredSize.Width, (int)WorkingSize.Height);
                            break;
                        case DockLocation.Right:
                            float width = window.DesiredSize.Width;
                            windowTargetWidget.setCoord((int)(WorkingSize.Width - width + Location.x), (int)Location.y, (int)width, (int)WorkingSize.Height);
                            break;
                        case DockLocation.Top:
                            windowTargetWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)window.DesiredSize.Height);
                            break;
                        case DockLocation.Bottom:
                            float height = window.DesiredSize.Height;
                            windowTargetWidget.setCoord((int)Location.x, (int)(WorkingSize.Height - height + Location.y), (int)WorkingSize.Width, (int)height);
                            break;
                    }

                    return true;
                }
            }
            return false;
        }

        private bool processFloating(MDIWindow source, float mouseX, float mouseY)
        {
            if ((source.AllowedDockLocations & DockLocation.Left) != 0)
            {
                MDIWindow window = left.findWindowAtPosition(mouseX, mouseY);
                if (window != null)
                {
                    dragTargetContainer = null;
                    dragTargetWindow = window;
                    findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                    return true;
                }
            }

            if ((source.AllowedDockLocations & DockLocation.Right) != 0)
            {
                MDIWindow window = right.findWindowAtPosition(mouseX, mouseY);
                if (window != null)
                {
                    dragTargetContainer = null;
                    dragTargetWindow = window;
                    findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                    return true;
                }
            }

            if ((source.AllowedDockLocations & DockLocation.Top) != 0)
            {
                MDIWindow window = top.findWindowAtPosition(mouseX, mouseY);
                if (window != null)
                {
                    dragTargetContainer = null;
                    dragTargetWindow = window;
                    findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                    return true;
                }
            }

            if ((source.AllowedDockLocations & DockLocation.Bottom) != 0)
            {
                MDIWindow window = bottom.findWindowAtPosition(mouseX, mouseY);
                if (window != null)
                {
                    dragTargetContainer = null;
                    dragTargetWindow = window;
                    findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                    return true;
                }
            }
            
            if ((source.AllowedDockLocations & DockLocation.Floating) != 0)
            {
                windowTargetWidget.Visible = false;
                dragTargetContainer = floating;
                dragTargetWindow = null;
                return true;
            }
            return false;
        }

        private bool processCenter(MDIWindow source, float mouseX, float mouseY)
        {
            dragTargetContainer = null;
            if((source.AllowedDockLocations & DockLocation.Center) == 0)
            {
                dragTargetWindow = null;
                return false;
            }
            dragTargetWindow = documentArea.findWindowAtPosition(mouseX, mouseY);
            findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
            return true;
        }

        private void findWindowLanding(MDIWindow source, MDIWindow target, float mouseX, float mouseY)
        {
            if (target != null)
            {
                if (source != target)
                {
                    float top = target.Location.y;
                    float bottom = top + target.WorkingSize.Height;
                    float left = target.Location.x;
                    float right = left + target.WorkingSize.Width;

                    float topDelta = mouseY - top;
                    float bottomDelta = bottom - mouseY;
                    float leftDelta = mouseX - left;
                    float rightDelta = right - mouseX;
                    if (topDelta < bottomDelta && topDelta < leftDelta && topDelta < rightDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Top;
                        windowTargetWidget.setCoord((int)left, (int)top, (int)target.WorkingSize.Width, (int)(target.WorkingSize.Height * 0.5f));
                    }
                    else if (bottomDelta < topDelta && bottomDelta < leftDelta && bottomDelta < rightDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Bottom;
                        windowTargetWidget.setCoord((int)left, (int)(top + target.WorkingSize.Height * 0.5f), (int)target.WorkingSize.Width, (int)(target.WorkingSize.Height * 0.5f));
                    }
                    else if (leftDelta < topDelta && leftDelta < bottomDelta && leftDelta < rightDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Left;
                        windowTargetWidget.setCoord((int)left, (int)top, (int)(target.WorkingSize.Width * 0.5f), (int)target.WorkingSize.Height);
                    }
                    else if (rightDelta < leftDelta && rightDelta < bottomDelta && rightDelta < topDelta)
                    {
                        finalWindowAlignment = WindowAlignment.Right;
                        windowTargetWidget.setCoord((int)(left + target.WorkingSize.Width * 0.5f), (int)top, (int)(target.WorkingSize.Width * 0.5f), (int)target.WorkingSize.Height);
                    }

                    windowTargetWidget.Visible = true;
                }
                else
                {
                    windowTargetWidget.Visible = false;
                }
            }
        }

        public void windowDragEnded(MDIWindow source, float mouseX, float mouseY)
        {
            windowTargetWidget.Visible = false;
            leftContainerWidget.Visible = false;
            rightContainerWidget.Visible = false;
            topContainerWidget.Visible = false;
            bottomContainerWidget.Visible = false;

            if (dragTargetContainer != null)
            {
                dragSourceWindow._ParentContainer.removeChild(dragSourceWindow);
                dragTargetContainer.addChild(dragSourceWindow);
                this.layout();
            }
            else if (dragTargetWindow != dragSourceWindow && dragTargetWindow != null)
            {
                dragSourceWindow._ParentContainer.removeChild(dragSourceWindow);
                dragTargetWindow._ParentContainer.addChild(dragSourceWindow, dragTargetWindow, finalWindowAlignment);
                this.layout();
            }
        }

        public void addChild(MDIWindow child)
        {
            switch (child.CurrentDockLocation)
            {
                case DockLocation.Left:
                    left.addChild(child);
                    break;
                case DockLocation.Right:
                    right.addChild(child);
                    break;
                case DockLocation.Top:
                    top.addChild(child);
                    break;
                case DockLocation.Bottom:
                    bottom.addChild(child);
                    break;
                case DockLocation.Center:
                    documentArea.addChild(child);
                    break;
                case DockLocation.Floating:
                    floating.addChild(child);
                    break;
            }
        }

        public void removeChild(MDIWindow child)
        {
            switch (child.CurrentDockLocation)
            {
                case DockLocation.Left:
                    left.removeChild(child);
                    break;
                case DockLocation.Right:
                    right.removeChild(child);
                    break;
                case DockLocation.Top:
                    top.removeChild(child);
                    break;
                case DockLocation.Bottom:
                    bottom.removeChild(child);
                    break;
                case DockLocation.Center:
                    documentArea.removeChild(child);
                    break;
                case DockLocation.Floating:
                    floating.removeChild(child);
                    break;
            }
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            MDIWindow retVal = null;
            retVal = findWindowPositionImpl(mouseX, mouseY, documentArea);
            if (retVal != null)
            {
                return retVal;
            } 
            retVal = findWindowPositionImpl(mouseX, mouseY, left);
            if (retVal != null)
            {
                return retVal;
            }
            retVal = findWindowPositionImpl(mouseX, mouseY, right);
            if (retVal != null)
            {
                return retVal;
            }
            retVal = findWindowPositionImpl(mouseX, mouseY, top);
            if (retVal != null)
            {
                return retVal;
            }
            retVal = findWindowPositionImpl(mouseX, mouseY, bottom);
            if (retVal != null)
            {
                return retVal;
            }
            return null;
        }

        private MDIWindow findWindowPositionImpl(float mouseX, float mouseY, MDIContainerBase container)
        {
            float left = container.Location.x;
            float top = container.Location.y;
            float right = left + container.WorkingSize.Width;
            float bottom = top + container.WorkingSize.Height;
            if (mouseX > left && mouseX < right &&
                mouseY > top && mouseY < bottom)
            {
                return container.findWindowAtPosition(mouseX, mouseY);
            }
            return null;
        }

        protected internal override bool isControlWidgetAtPosition(int x, int y)
        {
            return left.isControlWidgetAtPosition(x, y)
                || right.isControlWidgetAtPosition(x, y)
                || top.isControlWidgetAtPosition(x, y)
                || bottom.isControlWidgetAtPosition(x, y);
        }

        public override void setAlpha(float alpha)
        {
            top.setAlpha(alpha);
            bottom.setAlpha(alpha);
            left.setAlpha(alpha);
            center.setAlpha(alpha);
            right.setAlpha(alpha);
        }

        public override void layout()
        {
            IntSize2 leftDesired = left.DesiredSize;
            IntSize2 rightDesired = right.DesiredSize;
            IntSize2 topDesired = top.DesiredSize;
            IntSize2 bottomDesired = bottom.DesiredSize;

            //Determine center region size.
            IntSize2 centerSize = new IntSize2(WorkingSize.Width - leftDesired.Width - rightDesired.Width, WorkingSize.Height - topDesired.Height - bottomDesired.Height);

            top.Location = this.Location;
            top.WorkingSize = new IntSize2(WorkingSize.Width, topDesired.Height);
            top.layout();

            bottom.Location = new IntVector2(this.Location.x, this.Location.y + topDesired.Height + centerSize.Height);
            bottom.WorkingSize = new IntSize2(WorkingSize.Width, bottomDesired.Height);
            bottom.layout();

            left.Location = new IntVector2(this.Location.x, this.Location.y + topDesired.Height);
            left.WorkingSize = new IntSize2(leftDesired.Width, centerSize.Height);
            left.layout();

            if (centerParentContainer != null)
            {
                centerParentContainer.Location = new IntVector2(this.Location.x + leftDesired.Width, this.Location.y + topDesired.Height);
                centerParentContainer.WorkingSize = centerSize;
                centerParentContainer.layout();
            }
            else
            {
                center.Location = new IntVector2(this.Location.x + leftDesired.Width, this.Location.y + topDesired.Height);
                center.WorkingSize = centerSize;
                center.layout();
            }

            right.Location = new IntVector2(this.Location.x + leftDesired.Width + centerSize.Width, this.Location.y + topDesired.Height);
            right.WorkingSize = new IntSize2(rightDesired.Width, centerSize.Height);
            right.layout();
        }

        public override IntSize2 DesiredSize
        {
            get
            {
                IntSize2 desiredSize = new IntSize2();
                IntSize2 leftDesired = left.DesiredSize;
                IntSize2 rightDesired = right.DesiredSize;
                IntSize2 topDesired = top.DesiredSize;
                IntSize2 bottomDesired = bottom.DesiredSize;
                if (leftDesired.Height > rightDesired.Height)
                {
                    desiredSize.Height = leftDesired.Height + topDesired.Height + bottomDesired.Height;
                }
                else
                {
                    desiredSize.Height = rightDesired.Height + topDesired.Height + bottomDesired.Height;
                }
                if (topDesired.Width > bottomDesired.Width)
                {
                    desiredSize.Width = topDesired.Width;
                }
                else
                {
                    desiredSize.Width = bottomDesired.Width;
                }
                return desiredSize;
            }
        }

        public override IntSize2 ActualSize
        {
            get
            {
                //These will not be used for now, so don't worry about them
                return new IntSize2();
            }
            set
            {
                //These will not be used for now, so don't worry about them
            }
        }

        internal StoredBorderContainer storeCurrentLayout()
        {
            StoredBorderContainer storedBorderContainer = new StoredBorderContainer();
            storedBorderContainer.Left = left.storeCurrentLayout();
            storedBorderContainer.Right = right.storeCurrentLayout();
            storedBorderContainer.Top = top.storeCurrentLayout();
            storedBorderContainer.Bottom = bottom.storeCurrentLayout();
            storedBorderContainer.Floating = floating.storeCurrentLayout();
            return storedBorderContainer;
        }

        internal void restoreLayout(StoredBorderContainer storedBorderContainer)
        {
            left.restoreLayout(storedBorderContainer.Left);
            right.restoreLayout(storedBorderContainer.Right);
            top.restoreLayout(storedBorderContainer.Top);
            bottom.restoreLayout(storedBorderContainer.Bottom);
            floating.restoreLayout(storedBorderContainer.Floating);
        }

        public override void bringToFront()
        {
            center.bringToFront();
            left.bringToFront();
            right.bringToFront();
            top.bringToFront();
            bottom.bringToFront();
        }

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                center.Visible = value;
                left.Visible = value;
                right.Visible = value;
                top.Visible = value;
                bottom.Visible = value;
            }
        }

        public LayoutContainer Center
        {
            get
            {
                return center;
            }
            set
            {
                if (center != null)
                {
                    center._setParent(null);
                }
                center = value;
                if (center != null)
                {
                    center._setParent(this);
                }
            }
        }

        public MDILayoutContainer DocumentArea
        {
            get
            {
                return documentArea;
            }
        }
    }
}
