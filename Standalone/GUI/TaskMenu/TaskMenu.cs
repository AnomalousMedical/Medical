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
        private AppMenu appMenu;
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
            iconGrid = new ButtonGrid(iconScroller, new ButtonGridTextAdjustedGridLayout());
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
            appMenu.layout(width, height);
            IntCoord clientCoord = iconScroller.ClientCoord;
            iconGrid.resizeAndLayout(clientCoord.width);
        }

        public void setAppMenu(AppMenu appMenu)
        {
            this.appMenu = appMenu;
            widget.Visible = true;
            appMenu.putOnTaskMenu(widget);
            widget.Visible = false;
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
