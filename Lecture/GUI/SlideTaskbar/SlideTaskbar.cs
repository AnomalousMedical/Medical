using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using Medical;

namespace Lecture.GUI
{
    class SlideTaskbar : LayoutComponent
    {
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
                Button taskButton = (Button)widget.createWidgetT("Button", "TaskbarButton", left, 20, 48, 48, Align.Left | Align.Top, task.UniqueName);
                taskButton.UserObject = task;
                taskButton.NeedToolTip = true;
                taskButton.ImageBox.setItemResource(task.IconName);
                taskButton.MouseButtonClick += new MyGUIEvent(taskButton_MouseButtonClick);
                taskButton.EventToolTip += new MyGUIEvent(taskButton_EventToolTip);
                left += taskButton.Width + 2;
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
