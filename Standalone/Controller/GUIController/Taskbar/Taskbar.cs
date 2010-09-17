using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;
using Engine;

namespace Medical.GUI
{
    class Taskbar : LayoutContainer, IDisposable
    {
        private Layout myGUIlayout;
        private Widget taskbarWidget;
        private Button appButton;
        private AppMenu appMenu;
        private Vector2 startLocation;
        private float padding = 3;

        private List<TaskbarItem> taskbarItems  = new List<TaskbarItem>();

        public Taskbar(PiperJBOGUI piperGUI, StandaloneController controller)
        {
            myGUIlayout = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.Taskbar.Taskbar.layout");

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
            Button taskbarButton = taskbarWidget.createWidgetT("Button", "RibbonButton", 0, 0, 48, 48, Align.Left | Align.Top, item.Name) as Button;
            item.TaskbarButton = taskbarButton;
            taskbarButton.StaticImage.setItemResource(item.IconName);
            taskbarButton.MouseButtonClick += item.clicked;
            MyGUILayoutContainer container = new MyGUILayoutContainer(taskbarButton);
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
            Size2 itemSize = new Size2(48, 48);
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

            taskbarWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)WorkingSize.Height);
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2(53, taskbarItems.Count * 51 + appButton.Bottom);
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
            }
        }
    }
}
