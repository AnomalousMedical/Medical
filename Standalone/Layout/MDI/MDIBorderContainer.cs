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
        private MDILayoutContainer left;
        private MDILayoutContainer right;
        private MDILayoutContainer top;
        private MDILayoutContainer bottom;
        private MDILayoutContainer center;
        private FloatingWindowContainer floating;

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
        {
            windowTargetWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, 10, 10, Align.Left | Align.Top, "Info", "");
            windowTargetWidget.Visible = false;

            leftContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, 25, 25, Align.Left | Align.Top, "Info", "");
            leftContainerWidget.Visible = false;

            rightContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, 25, 25, Align.Left | Align.Top, "Info", "");
            rightContainerWidget.Visible = false;

            topContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, 25, 25, Align.Left | Align.Top, "Info", "");
            topContainerWidget.Visible = false;

            bottomContainerWidget = Gui.Instance.createWidgetT("Widget", "MDILocationPreview", 0, 0, 25, 25, Align.Left | Align.Top, "Info", "");
            bottomContainerWidget.Visible = false;

            left = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, padding, DockLocation.Left);
            left._setParent(this);

            right = new MDILayoutContainer(MDILayoutContainer.LayoutType.Vertical, padding, DockLocation.Right);
            right._setParent(this);
            
            top = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding, DockLocation.Top);
            top._setParent(this);
            
            bottom = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding, DockLocation.Bottom);
            bottom._setParent(this);
            
            center = new MDILayoutContainer(MDILayoutContainer.LayoutType.Horizontal, padding, DockLocation.Center);
            center._setParent(this);

            floating = new FloatingWindowContainer();
            floating._setParent(this);
        }

        public void Dispose()
        {
            left.Dispose();
            right.Dispose();
            top.Dispose();
            bottom.Dispose();
            center.Dispose();
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

        private bool checkContainerWidget(Widget widget, MDILayoutContainer targetContainer, MDIWindow window, DockLocation dockLocation, float x, float y)
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
                            windowTargetWidget.setCoord((int)(WorkingSize.Width - width), (int)Location.y, (int)width, (int)WorkingSize.Height);
                            break;
                        case DockLocation.Top:
                            windowTargetWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)window.DesiredSize.Height);
                            break;
                        case DockLocation.Bottom:
                            float height = window.DesiredSize.Height;
                            windowTargetWidget.setCoord((int)Location.x, (int)(WorkingSize.Height - height), (int)WorkingSize.Width, (int)height);
                            break;
                    }

                    return true;
                }
            }
            return false;
        }

        private bool processFloating(MDIWindow source, float mouseX, float mouseY)
        {
            MDIWindow window = left.findWindowAtPosition(mouseX, mouseY);
            if (window != null)
            {
                dragTargetContainer = null;
                dragTargetWindow = window;
                findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                return true;
            }

            window = right.findWindowAtPosition(mouseX, mouseY);
            if (window != null)
            {
                dragTargetContainer = null;
                dragTargetWindow = window;
                findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                return true;
            }

            window = top.findWindowAtPosition(mouseX, mouseY);
            if (window != null)
            {
                dragTargetContainer = null;
                dragTargetWindow = window;
                findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                return true;
            }

            window = bottom.findWindowAtPosition(mouseX, mouseY);
            if (window != null)
            {
                dragTargetContainer = null;
                dragTargetWindow = window;
                findWindowLanding(source, dragTargetWindow, mouseX, mouseY);
                return true;
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
            dragTargetWindow = center.findWindowAtPosition(mouseX, mouseY);
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
                    center.addChild(child);
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
                    center.removeChild(child);
                    break;
                case DockLocation.Floating:
                    floating.removeChild(child);
                    break;
            }
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            MDIWindow retVal = null;
            retVal = findWindowPositionImpl(mouseX, mouseY, center);
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

        private MDIWindow findWindowPositionImpl(float mouseX, float mouseY, MDILayoutContainer container)
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
            Size2 leftDesired = left != null ? left.DesiredSize : new Size2();
            Size2 rightDesired = right != null ? right.DesiredSize : new Size2();
            Size2 topDesired = top != null ? top.DesiredSize : new Size2();
            Size2 bottomDesired = bottom != null ? bottom.DesiredSize : new Size2();

            //Determine center region size.
            Size2 centerSize = new Size2(WorkingSize.Width - leftDesired.Width - rightDesired.Width, WorkingSize.Height - topDesired.Height - bottomDesired.Height);

            top.Location = this.Location;
            top.WorkingSize = new Size2(WorkingSize.Width, topDesired.Height);
            top.layout();
            
            bottom.Location = new Vector2(this.Location.x, this.Location.y + topDesired.Height + centerSize.Height);
            bottom.WorkingSize = new Size2(WorkingSize.Width, bottomDesired.Height);
            bottom.layout();
            
            left.Location = new Vector2(this.Location.x, this.Location.y + topDesired.Height);
            left.WorkingSize = new Size2(leftDesired.Width, centerSize.Height);
            left.layout();
            
            center.Location = new Vector2(this.Location.x + leftDesired.Width, this.Location.y + topDesired.Height);
            center.WorkingSize = centerSize;
            center.layout();
            
            right.Location = new Vector2(this.Location.x + leftDesired.Width + centerSize.Width, this.Location.y + topDesired.Height);
            right.WorkingSize = new Size2(rightDesired.Width, centerSize.Height);
            right.layout();
        }

        public override Size2 DesiredSize
        {
            get
            {
                Size2 desiredSize = new Size2();
                Size2 leftDesired = left.DesiredSize;
                Size2 rightDesired = right.DesiredSize;
                Size2 topDesired = top.DesiredSize;
                Size2 bottomDesired = bottom.DesiredSize;
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
    }
}
