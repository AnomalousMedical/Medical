using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI
{
    class EditorTaskbar : LayoutComponent
    {
        private String closeAction;

        public EditorTaskbar(EditorTaskbarView view, MyGUIViewHost viewHost)
            :base("Medical.GUI.EditorTaskbar.EditorTaskbar.layout", viewHost)
        {
            TextBox captionText = (TextBox)widget.findWidget("CaptionText");
            captionText.Caption = view.Caption;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
            closeAction = view.CloseAction;

            int left = 0;
            foreach (Task task in view.Tasks)
            {
                Button taskButton = (Button)widget.createWidgetT("Button", "TaskbarButton", left, captionText.Bottom, 48, 48, Align.Left | Align.Top, task.UniqueName);
                taskButton.UserObject = task;
                taskButton.NeedToolTip = true;
                taskButton.ImageBox.setItemResource(task.IconName);
                taskButton.MouseButtonClick += new MyGUIEvent(taskButton_MouseButtonClick);
                taskButton.EventToolTip += new MyGUIEvent(taskButton_EventToolTip);
                left += taskButton.Width + 2;
            }
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
