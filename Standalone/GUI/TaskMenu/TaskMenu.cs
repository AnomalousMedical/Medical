using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
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

        public TaskMenu()
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
            iconGrid.defineGroup(TaskMenuCategories.Simulation);
            iconGrid.defineGroup(TaskMenuCategories.Tools);
            iconGrid.defineGroup(TaskMenuCategories.Editor);
            iconGrid.defineGroup(TaskMenuCategories.System);
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
        }

        public TaskMenuSection Tasks
        {
            get
            {
                return tasksSection;
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
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ((TaskMenuItem)iconGrid.SelectedItem.UserObject).clicked();
            hide();
        }
    }
}
