using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class Taskbar : LayoutContainer, IDisposable
    {
        private Layout myGUIlayout;
        private Widget taskbarWidget;
        private Button appButton;
        private AppMenu appMenu;
        private Vector2 startLocation;
        private float padding = 3;
        private Size2 itemSize = new Size2(48, 48);
        private LayoutContainer child;

        private List<TaskbarItem> taskbarItems  = new List<TaskbarItem>();

        internal Taskbar(PiperJBOGUI piperGUI, StandaloneController controller)
        {
            myGUIlayout = LayoutManager.Instance.loadLayout("Medical.GUI.Taskbar.Taskbar.layout");

            taskbarWidget = myGUIlayout.getWidget(0);

            appButton = taskbarWidget.findWidget("AppButton") as Button;
            appButton.MouseButtonClick += new MyGUIEvent(appButton_MouseButtonClick);

            appMenu = new AppMenu(piperGUI, controller);

            startLocation = new Vector2(appButton.Left, appButton.Bottom + padding);
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
            item.TaskbarButton = taskbarWidget.createWidgetT("Button", "TaskbarButton", 0, 0, (int)itemSize.Width, (int)itemSize.Height, Align.Left | Align.Top, item.Name) as Button;
        }

        void appButton_MouseButtonClick(Widget source, EventArgs e)
        {
            appMenu.show(appButton.AbsoluteLeft, appButton.AbsoluteTop + appButton.Height);
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
                Vector2 currentLocation = startLocation;

                foreach (TaskbarItem item in taskbarItems)
                {
                    if (currentLocation.y + itemSize.Height > WorkingSize.Height)
                    {
                        currentLocation.x += itemSize.Width + padding;
                        currentLocation.y = startLocation.y;
                    }

                    item.TaskbarButton.setCoord((int)currentLocation.x, (int)currentLocation.y, (int)itemSize.Width, (int)itemSize.Height);
                    currentLocation.y += itemSize.Height + padding;
                }

                taskbarWidget.setCoord((int)Location.x, (int)Location.y, (int)(currentLocation.x + itemSize.Width + 3), (int)WorkingSize.Height);

                if (Child != null)
                {
                    Child.Location = new Vector2(Location.x + taskbarWidget.Width, Location.y);
                    Child.WorkingSize = new Size2(WorkingSize.Width - taskbarWidget.Width, WorkingSize.Height);
                    Child.layout();
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
    }
}
