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
        private List<EditorTaskbarFileButton> fileButtons = new List<EditorTaskbarFileButton>();
        private EditorController editorController;
        private String currentFile;

        public EditorTaskbar(EditorTaskbarView view, MyGUIViewHost viewHost, EditorController editorController)
            :base("Medical.GUI.EditorTaskbar.EditorTaskbar.layout", viewHost)
        {
            closeAction = view.CloseAction;
            this.editorController = editorController;
            this.currentFile = view.File;

            int left = 0;
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
            clearFileTabs();
            base.Dispose();
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

        void fileButton_Closed(EditorTaskbarFileButton obj)
        {
            editorController.closeFile(obj.File);
            if (obj.CurrentFile)
            {
                if (editorController.OpenFileCount == 0)
                {
                    ViewHost.Context.runAction(closeAction, ViewHost);
                }
                else
                {
                    editorController.openFile(editorController.OpenFiles.First());
                }
            }
            else
            {
                refreshFileTabs();
            }
        }

        void fileButton_ChangeFile(EditorTaskbarFileButton obj)
        {
            editorController.openFile(obj.File);
        }

        public override void topLevelResized()
        {
            refreshFileTabs();
            base.topLevelResized();
        }

        private int refreshFileTabs()
        {
            clearFileTabs();
            int left = 0;
            foreach (String file in editorController.OpenFiles)
            {
                EditorTaskbarFileButton fileButton = new EditorTaskbarFileButton(widget, file, left);
                fileButton.CurrentFile = currentFile == file;
                fileButton.ChangeFile += new Action<EditorTaskbarFileButton>(fileButton_ChangeFile);
                fileButton.Closed += new Action<EditorTaskbarFileButton>(fileButton_Closed);
                fileButtons.Add(fileButton);
                left += fileButton.Width;
                if (left > widget.Width)
                {
                    break;
                }
            }
            return left;
        }

        private void clearFileTabs()
        {
            foreach (EditorTaskbarFileButton fileButton in fileButtons)
            {
                fileButton.Dispose();
            }
            fileButtons.Clear();
        }
    }
}
