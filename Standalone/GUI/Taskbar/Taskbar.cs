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

    public class Taskbar : LayoutContainer, IDisposable
    {
        public delegate void OpenTaskMenuEvent(int left, int top, int width, int height);
        public event OpenTaskMenuEvent OpenTaskMenu;

        private Layout myGUIlayout;
        private Widget taskbarWidget;
        private Button appButton;
        private float padding = 3;
        private Size2 itemSize = new Size2(48, 48);
        private LayoutContainer child;
        private TaskbarAlignment alignment = TaskbarAlignment.Top;
        private IntCoord gapCoord = new IntCoord();
        private int gapIndex = -1;

        private String wideIcon;
        private String narrowIcon;

        private int appButtonWideWidth;
        private int appButtonNarrowWidth = 48;

        private List<TaskbarItem> taskbarItems  = new List<TaskbarItem>();

        internal Taskbar(StandaloneController controller)
        {
            myGUIlayout = LayoutManager.Instance.loadLayout("Medical.GUI.Taskbar.Taskbar.layout");

            taskbarWidget = myGUIlayout.getWidget(0);
            taskbarWidget.MouseDrag += new MyGUIEvent(taskbarWidget_MouseDrag);

            appButton = taskbarWidget.findWidget("AppButton") as Button;
            appButton.MouseButtonClick += new MyGUIEvent(appButton_MouseButtonClick);

            appButtonWideWidth = appButton.Width;

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

        public void setAppIcon(String wideIcon, String narrowIcon)
        {
            this.wideIcon = wideIcon;
            this.narrowIcon = narrowIcon;
            appButton.StaticImage.setItemResource(wideIcon);
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

        void appButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (OpenTaskMenu != null)
            {
                int left = 0;
                int top = 0;
                int width = (int)TopmostWorkingSize.Width;
                int height = (int)TopmostWorkingSize.Height;
                switch (alignment)
                {
                    case TaskbarAlignment.Left:
                        width -= taskbarWidget.Width;
                        left = taskbarWidget.Right;
                        break;
                    case TaskbarAlignment.Right:
                        width -= taskbarWidget.Width;
                        break;
                    case TaskbarAlignment.Top:
                        height -= taskbarWidget.Height;
                        top = taskbarWidget.Bottom;
                        break;
                    case TaskbarAlignment.Bottom:
                        height -= taskbarWidget.Height;
                        break;
                }
                OpenTaskMenu.Invoke(left, top, width, height);
            }
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

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2(itemSize.Width + 6, taskbarItems.Count * (itemSize.Height + padding) + appButton.Bottom);
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

        public LayoutContainer Child
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

        public bool AppButtonVisible
        {
            get
            {
                return appButton.Visible;
            }
            set
            {
                if (value != appButton.Visible)
                {
                    appButton.Visible = value;
                    layout();
                }
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

        private void layoutTaskbarVertical()
        {
            appButton.StaticImage.setItemResource(narrowIcon);
            appButton.StaticImage.setSize(32, 32);
            appButton.setSize(appButtonNarrowWidth, appButton.Height);
            Vector2 startLocation = new Vector2(appButton.Left, 0);
            Vector2 currentLocation = startLocation;

            int positionOffset = (int)padding;
            if (appButton.Visible)
            {
                positionOffset = (int)(appButton.Bottom + padding);
            }
            int iconAreaHeight = (int)(WorkingSize.Height - positionOffset);

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
                    gapCoord.top = (int)(currentLocation.y + positionOffset - padding);
                    gapCoord.width = (int)(itemSize.Width + padding);
                    gapCoord.height = (int)(itemSize.Height + padding);
                    currentLocation.y += itemSize.Height + padding;
                }

                if (currentLocation.y + itemSize.Height > iconAreaHeight)
                {
                    currentLocation.x += itemSize.Width + padding;
                    currentLocation.y = startLocation.y;
                }

                item.setCoord((int)currentLocation.x, (int)currentLocation.y + positionOffset, (int)itemSize.Width, (int)itemSize.Height);
                currentLocation.y += itemSize.Height + padding;
            }

            if (Alignment == TaskbarAlignment.Left)
            {
                taskbarWidget.setCoord((int)Location.x, (int)Location.y, (int)(currentLocation.x + itemSize.Width + 3), (int)WorkingSize.Height);

                if (Child != null)
                {
                    Child.Location = new Vector2(Location.x + taskbarWidget.Width, Location.y);
                    Child.WorkingSize = new Size2(WorkingSize.Width - taskbarWidget.Width, WorkingSize.Height);
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
                    Child.WorkingSize = new Size2(WorkingSize.Width - taskbarWidget.Width, WorkingSize.Height);
                    Child.layout();
                }
            }
        }

        private void layoutTaskbarHorizontal()
        {
            appButton.StaticImage.setItemResource(wideIcon);
            appButton.StaticImage.setSize(130, 32);
            appButton.setSize(appButtonWideWidth, appButton.Height);
            Vector2 startLocation = new Vector2(0, appButton.Top);
            Vector2 currentLocation = startLocation;

            int positionOffset = (int)padding;
            if (appButton.Visible)
            {
                positionOffset = (int)(appButton.Right + padding);
            }
            int iconAreaWidth = (int)(WorkingSize.Width - positionOffset);

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
                    gapCoord.left = (int)(currentLocation.x + positionOffset - padding);
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

                item.setCoord((int)currentLocation.x + positionOffset, (int)currentLocation.y, (int)itemSize.Width, (int)itemSize.Height);
                currentLocation.x += itemSize.Width + padding;
            }

            if (Alignment == TaskbarAlignment.Top)
            {
                taskbarWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)(currentLocation.y + itemSize.Height + 3));

                if (Child != null)
                {
                    Child.Location = new Vector2(Location.x, Location.y + taskbarWidget.Height);
                    Child.WorkingSize = new Size2(WorkingSize.Width, WorkingSize.Height - taskbarWidget.Height);
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
                    Child.WorkingSize = new Size2(WorkingSize.Width, WorkingSize.Height - taskbarWidget.Height);
                    Child.layout();
                }
            }
        }
    }
}
