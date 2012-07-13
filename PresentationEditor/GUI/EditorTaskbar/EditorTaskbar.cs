using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using Medical;

namespace PresentationEditor.GUI
{
    class EditorTaskbar : LayoutComponent
    {
        private String closeAction;
        private EditorController editorController;
        private String currentFile;

        public EditorTaskbar(EditorTaskbarView view, MyGUIViewHost viewHost, EditorController editorController)
            : base("PresentationEditor.GUI.EditorTaskbar.EditorTaskbar.layout", viewHost)
        {
            closeAction = view.CloseAction;
            this.editorController = editorController;
            this.currentFile = view.File;

            //Create task buttons
            int left = 0;
            foreach (Task task in view.Tasks)
            {
                Button taskButton = (Button)widget.createWidgetT("Button", "TaskbarButton", left, 0, 48, 48, Align.Left | Align.Top, task.UniqueName);
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
    }
}
