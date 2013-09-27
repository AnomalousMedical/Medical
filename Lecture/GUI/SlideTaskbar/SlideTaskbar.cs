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
                Button taskButton = (Button)widget.createWidgetT("Button", "TaskbarButton", left, TaskButtonTop, TaskButtonWidth, TaskButtonHeight, Align.Left | Align.Top, task.UniqueName);
                taskButton.UserObject = task;
                taskButton.NeedToolTip = true;
                taskButton.ImageBox.setItemResource(task.IconName);
                taskButton.MouseButtonClick += new MyGUIEvent(taskButton_MouseButtonClick);
                taskButton.EventToolTip += new MyGUIEvent(taskButton_EventToolTip);
                left += taskButton.Width + TaskButtonPadding;
            }
        }

        public override void Dispose()
        {
            view.DisplayNameChanged -= view_NameChanged;
            base.Dispose();
        }

        public override void closing()
        {
            base.closing();
        }

        void taskButton_EventToolTip(Widget source, EventArgs e)
        {
            TooltipManager.Instance.processTooltip(source, ((Task)source.UserObject).Name, (ToolTipEventArgs)e);
        }

        void taskButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ((Task)source.UserObject).clicked(null);
        }

        void view_NameChanged(SlideTaskbarView obj)
        {
            idLabel.Caption = view.DisplayName;
        }
    }
}
