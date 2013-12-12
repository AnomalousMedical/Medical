using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public enum TaskbarAlignment
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    public class Taskbar : SingleChildLayoutContainer, IDisposable
    {
        private Layout myGUIlayout;
        private int padding = ScaleHelper.Scaled(3);
        private IntSize2 itemSize;
        private LayoutContainer child;
        private TaskbarAlignment alignment = TaskbarAlignment.Top;
        private IntCoord gapCoord = new IntCoord();
        private int gapIndex = -1;

        private List<TaskbarItem> taskbarItems  = new List<TaskbarItem>();

        protected Widget taskbarWidget;
        protected int taskbarButtonWidth = ScaleHelper.Scaled(48);

        public Taskbar(String layout = "Medical.GUI.Taskbar.Taskbar.layout")
        {
            itemSize = new IntSize2(taskbarButtonWidth, taskbarButtonWidth);

            myGUIlayout = LayoutManager.Instance.loadLayout(layout);

            taskbarWidget = myGUIlayout.getWidget(0);
            taskbarWidget.MouseDrag += new MyGUIEvent(taskbarWidget_MouseDrag);

            clearGapIndex();
        }

        public void Dispose()
        {
            foreach (TaskbarItem item in taskbarItems)
            {
                item.Dispose();
            }
            LayoutManager.Instance.unloadLayout(myGUIlayout);
        }

        void taskbarWidget_MouseDrag(Widget source, EventArgs e)
        {
            MyGUIPlugin.MouseEventArgs me = (MouseEventArgs)e;
            float x = me.Position.x;
            float y = me.Position.y;
            float top = WorkingSize.Height * 0.48f;
            float bottom = WorkingSize.Height - top;
            float left = WorkingSize.Width * 0.48f;
            float right = WorkingSize.Width - left;

            if (y < top)
            {
                checkSides(x, TaskbarAlignment.Left, y, TaskbarAlignment.Top, WorkingSize.Width - x, TaskbarAlignment.Right);
            }
            else if (y > bottom)
            {
                checkSides(x, TaskbarAlignment.Left, WorkingSize.Height - y, TaskbarAlignment.Bottom, WorkingSize.Width - x, TaskbarAlignment.Right);
            }
            else if (x < left)
            {
                checkSides(x, TaskbarAlignment.Left, WorkingSize.Height - y, TaskbarAlignment.Bottom, y, TaskbarAlignment.Top);
            }
            else if (x > right)
            {
                checkSides(WorkingSize.Width - x, TaskbarAlignment.Right, WorkingSize.Height - y, TaskbarAlignment.Bottom, y, TaskbarAlignment.Top);
            }
        }

        void checkSides(float distance1, TaskbarAlignment alignment1, float distance2, TaskbarAlignment alignment2, float distance3, TaskbarAlignment alignment3)
        {
            if (distance1 < distance2)
            {
                if (distance3 < distance1)
                {
                    Alignment = alignment3;
                }
                else
                {
                    Alignment = alignment1;
                }
            }
            else
            {
                if (distance3 < distance2)
                {
                    Alignment = alignment3;
                }
                else
                {
                    Alignment = alignment2;
                }
            }
        }

        public void addItem(TaskbarItem item)
        {
            addItem(item, -1);
        }

        public void addItem(TaskbarItem item, int index)
        {
            if (index == -1)
            {
                taskbarItems.Add(item);
            }
            else
            {
                taskbarItems.Insert(index, item);
            }
            item._configureForTaskbar(this, taskbarWidget.createWidgetT("Button", "TaskbarButton", 0, 0, (int)itemSize.Width, (int)itemSize.Height, Align.Left | Align.Top, item.Name) as Button);
        }

        public void removeItem(TaskbarItem item)
        {
            int index = taskbarItems.IndexOf(item);
            if (index >= 0)
            {
                taskbarItems.RemoveAt(index);
                item._deconfigureForTaskbar();
            }
        }

        public int getIndexForPosition(IntVector2 position)
        {
            int xPos = position.x;
            int yPos = position.y;
            if (xPos >= taskbarWidget.AbsoluteLeft && xPos <= taskbarWidget.AbsoluteLeft + taskbarWidget.Width &&
                yPos >= taskbarWidget.AbsoluteTop && yPos <= taskbarWidget.AbsoluteTop + taskbarWidget.Height)
            {
                int counter = 0;
                foreach (TaskbarItem item in taskbarItems)
                {
                    int left = (int)(item.AbsoluteLeft - padding);
                    int width = (int)(item.Width + padding);
                    int right = left + width;

                    int top = (int)(item.AbsoluteTop - padding);
                    int height = (int)(item.Height + padding);
                    int bottom = top + height;
                    if (xPos >= left && xPos <= right && yPos >= top && yPos <= bottom)
                    {
                        switch (alignment)
                        {
                            case TaskbarAlignment.Left:
                                return yPos - top < height / 2 ? counter : counter + 1;
                            case TaskbarAlignment.Right:
                                return yPos - top < height / 2 ? counter : counter + 1;
                            default:
                                return xPos - left < width / 2 ? counter : counter + 1;
                        }
                    }
                    ++counter;
                }
                xPos -= taskbarWidget.AbsoluteLeft;
                yPos -= taskbarWidget.AbsoluteTop;
                if (xPos >= gapCoord.left && xPos <= gapCoord.left + gapCoord.width && yPos >= gapCoord.top && yPos <= gapCoord.top + gapCoord.height)
                {
                    return gapIndex;
                }
            }
            return -1;
        }

        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(taskbarWidget);
        }

        public override void setAlpha(float alpha)
        {
            taskbarWidget.Alpha = alpha;
        }

        public override void layout()
        {
            if (Visible)
            {
                if (Alignment == TaskbarAlignment.Left || Alignment == TaskbarAlignment.Right)
                {
                    layoutTaskbarVertical();
                }
                else
                {
                    layoutTaskbarHorizontal();
                }
            }
            else
            {
                if (Child != null)
                {
                    Child.Location = Location;
                    Child.WorkingSize = WorkingSize;
                    Child.layout();
                }
            }
        }

        public override IntSize2 DesiredSize
        {
            get 
            {
                return new IntSize2(itemSize.Width + 6, (taskbarItems.Count + 1) * (itemSize.Height + padding));
            }
        }

        public override bool Visible
        {
            get
            {
                return taskbarWidget.Visible;
            }
            set
            {
                taskbarWidget.Visible = value;
                invalidate();
            }
        }

        public override LayoutContainer Child
        {
            get
            {
                return child;
            }
            set
            {
                if (child != null)
                {
                    child._setParent(null);
                }
                child = value;
                if(child != null)
                {
                    child._setParent(this);
                }
                invalidate();
            }
        }

        public TaskbarAlignment Alignment
        {
            get
            {
                return alignment;
            }
            set
            {
                if (alignment != value)
                {
                    alignment = value;
                    layout();
                }
            }
        }

        public int GapIndex
        {
            get
            {
                return gapIndex;
            }
            set
            {
                gapIndex = value;
                if (gapIndex == -1)
                {
                    gapCoord = new IntCoord();
                }
            }
        }

        public int Width
        {
            get
            {
                return taskbarWidget.Width;
            }
        }

        public int Height
        {
            get
            {
                return taskbarWidget.Height;
            }
        }

        public int Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;
            }
        }

        public void clearGapIndex()
        {
            GapIndex = -1;
        }

        internal bool containsPosition(IntVector2 position)
        {
            return position.x > taskbarWidget.AbsoluteLeft &&
                position.x < taskbarWidget.AbsoluteLeft + taskbarWidget.Width &&
                position.y > taskbarWidget.AbsoluteTop &&
                position.y < taskbarWidget.AbsoluteTop + taskbarWidget.Height;
        }

        internal void getPinnedTasks(PinnedTaskSerializer pinnedTaskSerializer)
        {
            foreach (TaskbarItem item in taskbarItems)
            {
                item.addToPinnedTasksList(pinnedTaskSerializer);
            }
        }

        /// <summary>
        /// Layout any custom controls on the taskbar. Return where task icons should start going and an offset
        /// from the total taskbar height to begin wrapping around.
        /// </summary>
        /// <param name="startLocation">The start location for tasks.</param>
        /// <param name="heightOffset">The offset from the total height of the taskbar to begin wrapping.</param>
        protected virtual void layoutCustomElementsVertical(out Vector2 startLocation, out int heightOffset)
        {
            startLocation = new Vector2(padding, padding);
            heightOffset = padding;
        }

        private void layoutTaskbarVertical()
        {
            Vector2 startLocation;
            int heightOffset;

            layoutCustomElementsVertical(out startLocation, out heightOffset);
            heightOffset += padding;
            int iconAreaHeight = WorkingSize.Height - heightOffset;
            Vector2 currentLocation = startLocation;

            int counter = 0;
            foreach (TaskbarItem item in taskbarItems)
            {
                if (counter++ == GapIndex)
                {
                    if (currentLocation.y + itemSize.Height > iconAreaHeight)
                    {
                        currentLocation.x += itemSize.Width + padding;
                        currentLocation.y = startLocation.y;
                    }
                    gapCoord.left = (int)(currentLocation.x - padding);
                    gapCoord.top = (int)(currentLocation.y - padding);
                    gapCoord.width = (int)(itemSize.Width + padding);
                    gapCoord.height = (int)(itemSize.Height + padding);
                    currentLocation.y += itemSize.Height + padding;
                }

                if (currentLocation.y + itemSize.Height > iconAreaHeight)
                {
                    currentLocation.x += itemSize.Width + padding;
                    currentLocation.y = startLocation.y;
                }

                item.setCoord((int)currentLocation.x, (int)currentLocation.y, (int)itemSize.Width, (int)itemSize.Height);
                currentLocation.y += itemSize.Height + padding;
            }

            if (Alignment == TaskbarAlignment.Left)
            {
                taskbarWidget.setCoord((int)Location.x, (int)Location.y, (int)(currentLocation.x + itemSize.Width + 3), (int)WorkingSize.Height);

                if (Child != null)
                {
                    Child.Location = new IntVector2(Location.x + taskbarWidget.Width, Location.y);
                    Child.WorkingSize = new IntSize2(WorkingSize.Width - taskbarWidget.Width, WorkingSize.Height);
                    Child.layout();
                }
            }
            else if (Alignment == TaskbarAlignment.Right)
            {
                int width = (int)(currentLocation.x + itemSize.Width + 3);
                taskbarWidget.setCoord((int)WorkingSize.Width - width, (int)Location.y, width, (int)WorkingSize.Height);

                if (Child != null)
                {
                    Child.Location = Location;
                    Child.WorkingSize = new IntSize2(WorkingSize.Width - taskbarWidget.Width, WorkingSize.Height);
                    Child.layout();
                }
            }
        }

        /// <summary>
        /// Layout any custom controls on the taskbar. Return where task icons should start going and an offset
        /// from the total taskbar width to begin wrapping around.
        /// </summary>
        /// <param name="startLocation">The start location for tasks.</param>
        /// <param name="widthOffset">The offset from the total width of the taskbar to begin wrapping.</param>
        protected virtual void layoutCustomElementsHorizontal(out Vector2 startLocation, out int widthOffset)
        {
            startLocation = new Vector2(padding, padding);
            widthOffset = 0;
        }

        private void layoutTaskbarHorizontal()
        {
            Vector2 startLocation;
            int widthOffset;

            layoutCustomElementsHorizontal(out startLocation, out widthOffset);
            widthOffset += padding;
            int iconAreaWidth = (int)(WorkingSize.Width - widthOffset);
            Vector2 currentLocation = startLocation;

            int counter = 0;
            foreach (TaskbarItem item in taskbarItems)
            {
                if (counter++ == GapIndex)
                {
                    if (currentLocation.x + itemSize.Width > iconAreaWidth)
                    {
                        currentLocation.y += itemSize.Height + padding;
                        currentLocation.x = startLocation.x;
                    }
                    gapCoord.left = (int)(currentLocation.x - padding);
                    gapCoord.top = (int)(currentLocation.y - padding);
                    gapCoord.width = (int)(itemSize.Width + padding);
                    gapCoord.height = (int)(itemSize.Height + padding);
                    currentLocation.x += itemSize.Width + padding;
                }

                if (currentLocation.x + itemSize.Width > iconAreaWidth)
                {
                    currentLocation.y += itemSize.Height + padding;
                    currentLocation.x = startLocation.x;
                }

                item.setCoord((int)currentLocation.x, (int)currentLocation.y, (int)itemSize.Width, (int)itemSize.Height);
                currentLocation.x += itemSize.Width + padding;
            }

            if (Alignment == TaskbarAlignment.Top)
            {
                taskbarWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)(currentLocation.y + itemSize.Height + 3));

                if (Child != null)
                {
                    Child.Location = new IntVector2(Location.x, Location.y + taskbarWidget.Height);
                    Child.WorkingSize = new IntSize2(WorkingSize.Width, WorkingSize.Height - taskbarWidget.Height);
                    Child.layout();
                }
            }
            else if (Alignment == TaskbarAlignment.Bottom)
            {
                int height = (int)(currentLocation.y + itemSize.Height + 3);
                taskbarWidget.setCoord((int)Location.x, (int)WorkingSize.Height - height, (int)WorkingSize.Width, height);

                if (Child != null)
                {
                    Child.Location = Location;
                    Child.WorkingSize = new IntSize2(WorkingSize.Width, WorkingSize.Height - taskbarWidget.Height);
                    Child.layout();
                }
            }
        }
    }
}
