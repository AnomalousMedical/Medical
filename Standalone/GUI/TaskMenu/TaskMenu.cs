using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public delegate void TaskDragDropEventDelegate(Task item, IntVector2 position);

    public class TaskMenu : PopupContainer
    {
        private class TaskMenuItemComparer : IComparer<ButtonGridItem>
        {
            public int Compare(ButtonGridItem x, ButtonGridItem y)
            {
                Task xItem = (Task)x.UserObject;
                Task yItem = (Task)y.UserObject;
                if(xItem != null && yItem != null)
                {
                    return xItem.Weight - yItem.Weight;
                }
                return 0;
            }
        }

        private const int UNKNOWN_GROUP_WEIGHT = int.MaxValue / 2;

        private ButtonGrid iconGrid;
        private ScrollView iconScroller;

        private TaskMenuRecentDocuments recentDocuments;

        private ButtonGroup viewButtonGroup;
        private Button tasksButton;
        private Button documentsButton;
        private TaskController taskController;

        public event TaskDelegate TaskItemOpened;
        public event TaskDragDropEventDelegate TaskItemDropped;
        public event TaskDragDropEventDelegate TaskItemDragged;

        private StaticImage dragIconPreview;
        private IntVector2 dragMouseStartPosition;
        private StaticImage adImage = null;

        private bool firstTimeShown = true;
        private bool showAdImage = true;

        private TaskMenuPositioner taskMenuPositioner = new TaskMenuPositioner();

        public TaskMenu(DocumentController documentController, TaskController taskController)
            :base("Medical.GUI.TaskMenu.TaskMenu.layout")
        {
            this.taskController = taskController;
            taskController.TaskAdded += new TaskDelegate(taskController_TaskAdded);
            taskController.TaskRemoved += new TaskDelegate(taskController_TaskRemoved);

            iconScroller = (ScrollView)widget.findWidget("IconScroller");
            iconGrid = new ButtonGrid(iconScroller, new ButtonGridTextAdjustedGridLayout(), new TaskMenuItemComparer(), GroupCompare);
            iconGrid.HighlightSelectedButton = false;

            iconGrid.defineGroup(TaskMenuCategories.Patient, 0);
            iconGrid.defineGroup(TaskMenuCategories.Navigation, 1);
            iconGrid.defineGroup(TaskMenuCategories.Exams, int.MaxValue - 5);
            iconGrid.defineGroup(TaskMenuCategories.Tools, int.MaxValue - 4);
            iconGrid.defineGroup(TaskMenuCategories.Editor, int.MaxValue - 3);
            iconGrid.defineGroup(TaskMenuCategories.Developer, int.MaxValue - 2);
            iconGrid.defineGroup(TaskMenuCategories.AnomalousMedical, int.MaxValue - 1);
            iconGrid.defineGroup(TaskMenuCategories.System, int.MaxValue);

            recentDocuments = new TaskMenuRecentDocuments(widget, documentController);
            recentDocuments.DocumentClicked += new EventDelegate(recentDocuments_DocumentClicked);

            viewButtonGroup = new ButtonGroup();
            viewButtonGroup.SelectedButtonChanged += new EventHandler(viewButtonGroup_SelectedButtonChanged);
            tasksButton = (Button)widget.findWidget("Tasks");
            viewButtonGroup.addButton(tasksButton);
            documentsButton = (Button)widget.findWidget("Documents");
            viewButtonGroup.addButton(documentsButton);

            this.Hidden += new EventHandler(TaskMenu_Hidden);

            dragIconPreview = (StaticImage)Gui.Instance.createWidgetT("StaticImage", "StaticImage", 0, 0, 32, 32, Align.Default, "Info", "TaskMenuDragIconPreview");
            dragIconPreview.Visible = false;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            this.Showing += new EventHandler(TaskMenu_Showing);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(dragIconPreview);
            base.Dispose();
        }

        public void setPosition(int left, int top)
        {
            widget.setPosition(left, top);
        }

        public void setSize(int width, int height)
        {
            widget.setSize(width, height);
            IntCoord clientCoord = iconScroller.ClientCoord;
            iconGrid.resizeAndLayout(clientCoord.width);
            recentDocuments.resizeAndLayout(clientCoord.width);
        }

        public bool SuppressLayout
        {
            get
            {
                return iconGrid.SuppressLayout;
            }
            set
            {
                iconGrid.SuppressLayout = value;
            }
        }

        public String AdImageKey { get; set; }

        public bool ShowAdImage
        {
            get
            {
                return showAdImage;
            }
            set
            {
                showAdImage = value;
                if (!showAdImage && adImage != null)
                {
                    adImage.MouseButtonClick -= adImage_MouseButtonClick;
                    Gui.Instance.destroyWidget(adImage);
                    adImage = null;
                    iconScroller.setPosition(2, iconScroller.Top);
                    iconScroller.setSize(widget.Width, iconScroller.Height);
                    iconGrid.resizeAndLayout(iconScroller.ClientCoord.width);
                }
            }
        }

        void TaskMenu_Showing(object sender, EventArgs e)
        {
            if (firstTimeShown)
            {
                if (ShowAdImage && AdImageKey != null)
                {
                    firstTimeShown = false;
                    iconScroller.setPosition(232, iconScroller.Top);
                    iconScroller.setSize(widget.Width - 230, iconScroller.Height);
                    iconGrid.resizeAndLayout(iconScroller.ClientCoord.width);
                    adImage = (StaticImage)widget.createWidgetT("StaticImage", "StaticImage", 2, iconScroller.Top, 200, 400, Align.Left | Align.Top, "");
                    adImage.setItemResource(AdImageKey);
                    adImage.MouseButtonClick += adImage_MouseButtonClick;
                }
            }
        }

        void adImage_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser(String.Format(MedicalConfig.ProductPageBaseURL, "1"));
        }

        void taskController_TaskRemoved(Task task)
        {
            task.IconChanged -= task_IconChanged;
        }

        void taskController_TaskAdded(Task task)
        {
            task.IconChanged += task_IconChanged;
            ButtonGridItem item = iconGrid.addItem(task.Category, task.Name, task.IconName);
            item.UserObject = task;
            item.ItemClicked += new EventHandler(item_ItemClicked);
            task.RequestShowInTaskbar += new TaskDelegate(taskItem_RequestShowInTaskbar);
            item.MouseButtonPressed += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonPressed);
            item.MouseDrag += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseDrag);
            item.MouseButtonReleased += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonReleased);
        }

        void task_IconChanged(Task task)
        {
            ButtonGridItem item = iconGrid.findItemByUserObject(task);
            if (item != null)
            {
                item.setImage(task.IconName);
            }
        }

        void taskItem_RequestShowInTaskbar(Task item)
        {
            if (TaskItemOpened != null)
            {
                TaskItemOpened.Invoke(item);
            }
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            Task item = (Task)iconGrid.SelectedItem.UserObject;
            taskMenuPositioner.CurrentItem = iconGrid.SelectedItem;
            item.clicked(taskMenuPositioner);
            hide();
            if (TaskItemOpened != null)
            {
                TaskItemOpened.Invoke(item);
            }
        }

        void viewButtonGroup_SelectedButtonChanged(object sender, EventArgs e)
        {
            iconScroller.Visible = viewButtonGroup.SelectedButton == tasksButton;
            recentDocuments.Visible = viewButtonGroup.SelectedButton == documentsButton;
        }

        void recentDocuments_DocumentClicked()
        {
            this.hide();
        }

        void TaskMenu_Hidden(object sender, EventArgs e)
        {
            viewButtonGroup.SelectedButton = tasksButton;
        }

        void item_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            dragMouseStartPosition = arg.Position;
        }

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
            {
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(((Task)source.UserObject).IconName);
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }
            if (TaskItemDragged != null)
            {
                TaskItemDragged.Invoke((Task)source.UserObject, arg.Position);
            }
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.Visible = false;
            if (TaskItemDropped != null)
            {
                TaskItemDropped.Invoke((Task)source.UserObject, arg.Position);
            }
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        public int GroupCompare(Object x, Object y)
        {
            int xWeight = UNKNOWN_GROUP_WEIGHT;
            int yWeight = UNKNOWN_GROUP_WEIGHT;
            if(x != null)
            {
                xWeight = (int)x;
            }
            if(y != null)
            {
                yWeight = (int)y;
            }
            return xWeight - yWeight;
        }
    }
}
