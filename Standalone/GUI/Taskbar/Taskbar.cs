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
        private Layout myGUIlayout;
        private Widget taskbarWidget;
        private Button appButton;
        private AppMenu appMenu;
        private float padding = 3;
        private Size2 itemSize = new Size2(48, 48);
        private LayoutContainer child;
        private TaskbarAlignment alignment = TaskbarAlignment.Top;

        private List<TaskbarItem> taskbarItems  = new List<TaskbarItem>();

        internal Taskbar(AppMenu appMenu, StandaloneController controller)
        {
            myGUIlayout = LayoutManager.Instance.loadLayout("Medical.GUI.Taskbar.Taskbar.layout");

            taskbarWidget = myGUIlayout.getWidget(0);
            taskbarWidget.MouseDrag += new MyGUIEvent(taskbarWidget_MouseDrag);

            appButton = taskbarWidget.findWidget("AppButton") as Button;

            if (appMenu != null)
            {
                this.appMenu = appMenu;
                appButton.MouseButtonClick += new MyGUIEvent(appButton_MouseButtonClick);
            }
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

        public void Dispose()
        {
            foreach (TaskbarItem item in taskbarItems)
            {
                item.Dispose();
            }
            LayoutManager.Instance.unloadLayout(myGUIlayout);
        }

        public void addItem(TaskbarItem item)
        {
            taskbarItems.Add(item);
            item._configureForTaskbar(this, taskbarWidget.createWidgetT("Button", "TaskbarButton", 0, 0, (int)itemSize.Width, (int)itemSize.Height, Align.Left | Align.Top, item.Name) as Button);
        }

        void appButton_MouseButtonClick(Widget source, EventArgs e)
        {
            switch (alignment)
            {
                case TaskbarAlignment.Left:
                    appMenu.show(appButton.AbsoluteLeft, appButton.AbsoluteTop + appButton.Height);
                    break;
                case TaskbarAlignment.Right:
                    appMenu.show(appButton.AbsoluteLeft - appMenu.Width + appButton.Width, appButton.AbsoluteTop + appButton.Height);
                    break;
                case TaskbarAlignment.Top:
                    appMenu.show(appButton.AbsoluteLeft, appButton.AbsoluteTop + appButton.Height);
                    break;
                case TaskbarAlignment.Bottom:
                    appMenu.show(appButton.AbsoluteLeft, appButton.AbsoluteTop - appMenu.Height);
                    break;
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

        private void layoutTaskbarVertical()
        {
            Vector2 startLocation = new Vector2(appButton.Left, appButton.Bottom + padding);
            Vector2 currentLocation = startLocation;

            foreach (TaskbarItem item in taskbarItems)
            {
                if (currentLocation.y + itemSize.Height > WorkingSize.Height)
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
            Vector2 startLocation = new Vector2(appButton.Right + padding, appButton.Top);
            Vector2 currentLocation = startLocation;

            foreach (TaskbarItem item in taskbarItems)
            {
                if (currentLocation.x + itemSize.Width > WorkingSize.Width)
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
