using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;
using Engine;

namespace Medical.GUI
{
    class Taskbar : IDisposable
    {
        private Layout layout;
        private Widget taskbarWidget;
        private Button appButton;
        private MyGUILayoutContainer myGUIContainer;
        private AppMenu appMenu;

        private GridLayoutContainer itemContainer;
        private List<TaskbarItem> taskbarItems  = new List<TaskbarItem>();

        public Taskbar(PiperJBOGUI piperGUI, StandaloneController controller)
        {
            layout = LayoutManager.Instance.loadLayout("Medical.Controller.GUIController.Taskbar.Taskbar.layout");

            taskbarWidget = layout.getWidget(0);
            myGUIContainer = new MyGUILayoutContainer(taskbarWidget);
            myGUIContainer.LaidOut += new EventHandler(myGUIContainer_LaidOut);

            appButton = taskbarWidget.findWidget("AppButton") as Button;
            appButton.MouseButtonClick += new MyGUIEvent(appButton_MouseButtonClick);

            appMenu = new AppMenu(piperGUI, controller);

            itemContainer = new GridLayoutContainer(GridLayoutContainer.LayoutType.Vertical, 3.0f, new Vector2(appButton.Left, appButton.Bottom + 3));
            itemContainer.GridLaidOut += new EventHandler(itemContainer_GridLaidOut);
        }

        public void Dispose()
        {
            foreach (TaskbarItem item in taskbarItems)
            {
                item.Dispose();
            }
        }

        public void beginSetup()
        {
            itemContainer.SuppressLayout = true;
        }

        public void endSetup()
        {
            itemContainer.SuppressLayout = false;
        }

        public void addItem(TaskbarItem item)
        {
            taskbarItems.Add(item);
            Button taskbarButton = taskbarWidget.createWidgetT("Button", "RibbonButton", 0, 0, 48, 48, Align.Left | Align.Top, item.Name) as Button;
            item.TaskbarButton = taskbarButton;
            taskbarButton.StaticImage.setItemResource(item.IconName);
            taskbarButton.MouseButtonClick += item.clicked;
            MyGUILayoutContainer container = new MyGUILayoutContainer(taskbarButton);
            itemContainer.addChild(container);
        }

        public MyGUILayoutContainer Container
        {
            get
            {
                return myGUIContainer;
            }
        }

        public bool Visible
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

        void appButton_MouseButtonClick(Widget source, EventArgs e)
        {
            appMenu.show(appButton.AbsoluteLeft, appButton.AbsoluteTop + appButton.Height);
        }

        void myGUIContainer_LaidOut(object sender, EventArgs e)
        {
            itemContainer.WorkingSize = myGUIContainer.WorkingSize;
            itemContainer.invalidate();
        }

        void itemContainer_GridLaidOut(object sender, EventArgs e)
        {
            //Size2 newSize = new Size2(itemContainer.GridSize.Width, taskbarWidget.Height);
            //if (newSize != myGUIContainer.DesiredSize)
            //{
            //    myGUIContainer.changeDesiredSize(newSize);
            //}
        }
    }
}
