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

        private String currentFile;
        private SlideTaskbarView view;
        private TextBox idLabel;
        private LinkedList<SlideTaskbarItem> taskbarItems = new LinkedList<SlideTaskbarItem>();

        public SlideTaskbar(SlideTaskbarView view, MyGUIViewHost viewHost)
            : base("Lecture.GUI.SlideTaskbar.SlideTaskbar.layout", viewHost)
        {
            this.view = view;
            this.currentFile = view.DisplayName;
            view.DisplayNameChanged += view_NameChanged;

            idLabel = (TextBox)widget.findWidget("IdLabel");
            idLabel.Caption = view.DisplayName;

            int left = 1;
            foreach (Task task in view.Tasks)
            {
                SlideTaskbarItem taskButton = new SlideTaskbarItem(task, widget, new IntRect(left, TaskButtonTop, TaskButtonWidth, TaskButtonHeight));
                left += taskButton.Width + TaskButtonPadding;
                taskbarItems.AddLast(taskButton);
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
    }
}
