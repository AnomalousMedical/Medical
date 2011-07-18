using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public delegate void TaskItemDelegate(TaskMenuItem item);
    public delegate void TaskItemDroppedDelegate(TaskMenuItem item, IntVector2 position);

    public class TaskMenu : PopupContainer
    {
        private class TaskMenuItemComparer : IComparer<ButtonGridItem>
        {
            public int Compare(ButtonGridItem x, ButtonGridItem y)
            {
                TaskMenuItem xItem = (TaskMenuItem)x.UserObject;
                TaskMenuItem yItem = (TaskMenuItem)y.UserObject;
                if(xItem != null && yItem != null)
                {
                    return xItem.Weight - yItem.Weight;
                }
                return 0;
            }
        }

        private TaskMenuSection tasksSection;
        private ButtonGrid iconGrid;
        private ScrollView iconScroller;

        private TaskMenuRecentDocuments recentDocuments;

        private ButtonGroup viewButtonGroup;
        private Button tasksButton;
        private Button documentsButton;

        public event TaskItemDelegate TaskItemOpened;
        public event TaskItemDroppedDelegate TaskItemDropped;

        private StaticImage dragIconPreview;

        public TaskMenu(DocumentController documentController)
            :base("Medical.GUI.TaskMenu.TaskMenu.layout")
        {
            tasksSection = new TaskMenuSection();
            tasksSection.TaskItemAdded += new TaskMenuSection.TaskEvent(tasksSection_TaskItemAdded);
            tasksSection.TaskItemRemoved += new TaskMenuSection.TaskEvent(tasksSection_TaskItemRemoved);

            iconScroller = (ScrollView)widget.findWidget("IconScroller");
            iconGrid = new ButtonGrid(iconScroller, new ButtonGridTextAdjustedGridLayout(), new TaskMenuItemComparer());
            iconGrid.HighlightSelectedButton = false;

            iconGrid.defineGroup(TaskMenuCategories.Patient);
            iconGrid.defineGroup(TaskMenuCategories.Navigation);
            iconGrid.defineGroup(TaskMenuCategories.Exams);
            iconGrid.defineGroup(TaskMenuCategories.Education);
            iconGrid.defineGroup(TaskMenuCategories.Simulation);
            iconGrid.defineGroup(TaskMenuCategories.Tools);
            iconGrid.defineGroup(TaskMenuCategories.Editor);
            iconGrid.defineGroup(TaskMenuCategories.System);

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

        public TaskMenuSection Tasks
        {
            get
            {
                return tasksSection;
            }
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

        void tasksSection_TaskItemRemoved(TaskMenuItem taskItem)
        {
            
        }

        void tasksSection_TaskItemAdded(TaskMenuItem taskItem)
        {
            ButtonGridItem item = iconGrid.addItem(taskItem.Category, taskItem.Name, taskItem.IconName);
            item.UserObject = taskItem;
            item.ItemClicked += new EventHandler(item_ItemClicked);
            taskItem.RequestShowInTaskbar += new TaskItemDelegate(taskItem_RequestShowInTaskbar);
            item.MouseDrag += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseDrag);
            item.MouseButtonReleased += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonReleased);
        }

        void taskItem_RequestShowInTaskbar(TaskMenuItem item)
        {
            if (TaskItemOpened != null)
            {
                TaskItemOpened.Invoke(item);
            }
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            TaskMenuItem item = (TaskMenuItem)iconGrid.SelectedItem.UserObject;
            item.clicked();
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

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible)
            {
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(((TaskMenuItem)source.UserObject).IconName);
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.Visible = false;
            if (TaskItemDropped != null)
            {
                TaskItemDropped.Invoke((TaskMenuItem)source.UserObject, arg.Position);
            }
        }
    }
}
