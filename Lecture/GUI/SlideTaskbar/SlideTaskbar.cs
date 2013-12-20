using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using Medical;
using Engine;

namespace Lecture.GUI
{
    class SlideTaskbar : LayoutComponent
    {
        private static int TaskButtonTop = ScaleHelper.Scaled(20);
        private static int TaskButtonWidth = ScaleHelper.Scaled(48);
        private static int TaskButtonHeight = ScaleHelper.Scaled(48);
        private static int TaskButtonPadding = ScaleHelper.Scaled(2);

        private int lastWidth = -1;
        private String currentFile;
        private SlideTaskbarView view;
        private TextBox idLabel;
        private LinkedList<SlideTaskbarItem> taskbarItems = new LinkedList<SlideTaskbarItem>();

        public SlideTaskbar(SlideTaskbarView view, MyGUIViewHost viewHost)
            : base("Lecture.GUI.SlideTaskbar.SlideTaskbar.layout", viewHost)
        {
            this.view = view;
            view.GetDesiredSizeOverride = getDesiredSize;
            this.currentFile = view.DisplayName;
            view.DisplayNameChanged += view_NameChanged;

            idLabel = (TextBox)widget.findWidget("IdLabel");
            idLabel.Caption = view.DisplayName;

            int left = 1;
            int top = TaskButtonTop;
            foreach (Task task in view.Tasks)
            {
                SlideTaskbarItem taskButton = new SlideTaskbarItem(task, widget, new IntRect(left, top, TaskButtonWidth, TaskButtonHeight));
                left += TaskButtonWidth + TaskButtonPadding;
                taskbarItems.AddLast(taskButton);
                if (left > widget.Width)
                {
                    left = 1;
                    top += TaskButtonHeight + TaskButtonPadding;
                }
                lastWidth = widget.Width;
                widget.setSize(lastWidth, top + TaskButtonHeight + TaskButtonPadding);
            }
        }

        public override void Dispose()
        {
            foreach (SlideTaskbarItem item in taskbarItems)
            {
                item.Dispose();
            }
            taskbarItems.Clear();
            view.DisplayNameChanged -= view_NameChanged;
            base.Dispose();
        }

        public override void closing()
        {
            base.closing();
        }

        void view_NameChanged(SlideTaskbarView obj)
        {
            idLabel.Caption = view.DisplayName;
        }

        public IntSize2 getDesiredSize(LayoutContainer layoutContainer, Widget widget, MyGUIView view)
        {
            int left = 1;
            int top = TaskButtonTop;
            int widgetWidth = layoutContainer.RigidParentWorkingSize.Width;
            foreach (SlideTaskbarItem taskButton in taskbarItems)
            {
                left += TaskButtonWidth + TaskButtonPadding;
                if (left + TaskButtonWidth > widgetWidth)
                {
                    left = 1;
                    top += TaskButtonHeight + TaskButtonPadding;
                }
            }
            return new IntSize2(widgetWidth, top + TaskButtonHeight + TaskButtonPadding);
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            if (lastWidth != widget.Width)
            {
                int left = 1;
                int top = TaskButtonTop;
                int widgetWidth = ViewHost.Container.WorkingSize.Width;
                foreach (SlideTaskbarItem taskButton in taskbarItems)
                {
                    taskButton.setPosition(left, top);
                    left += TaskButtonWidth + TaskButtonPadding;
                    if (left + TaskButtonWidth > widgetWidth)
                    {
                        left = 1;
                        top += TaskButtonHeight + TaskButtonPadding;
                    }
                }
                lastWidth = widget.Width;
            }
        }
    }
}
