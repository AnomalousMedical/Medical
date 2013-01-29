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
        private String closeAction;
        private String currentFile;

        public SlideTaskbar(SlideTaskbarView view, MyGUIViewHost viewHost)
            : base("Lecture.GUI.SlideTaskbar.SlideTaskbar.layout", viewHost)
        {
            closeAction = view.CloseAction;
            this.currentFile = view.File;

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

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ViewHost.Context.runAction(closeAction, ViewHost);
        }
    }
}
