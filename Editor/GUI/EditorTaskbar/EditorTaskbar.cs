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
        private Button fileListButton;

        public EditorTaskbar(EditorTaskbarView view, MyGUIViewHost viewHost, EditorController editorController)
            :base("Medical.GUI.EditorTaskbar.EditorTaskbar.layout", viewHost)
        {
            closeAction = view.CloseAction;
            this.editorController = editorController;
            this.currentFile = view.File;

            fileListButton = (Button)widget.findWidget("FileListButton");
            fileListButton.MouseButtonClick += new MyGUIEvent(fileListButton_MouseButtonClick);

            //Always have a save all button
            int left = 0;
            CallbackTask saveAll = new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                editorController.saveAllCachedResources();
            });
            Button taskButton = (Button)widget.createWidgetT("Button", "TaskbarButton", left, 20, 48, 48, Align.Left | Align.Top, saveAll.UniqueName);
            taskButton.UserObject = saveAll;
            taskButton.NeedToolTip = true;
            taskButton.ImageBox.setItemResource(saveAll.IconName);
            taskButton.MouseButtonClick += new MyGUIEvent(taskButton_MouseButtonClick);
            taskButton.EventToolTip += new MyGUIEvent(taskButton_EventToolTip);
            left += taskButton.Width + 2;

            foreach (Task task in view.Tasks)
            {
                taskButton = (Button)widget.createWidgetT("Button", "TaskbarButton", left, 20, 48, 48, Align.Left | Align.Top, task.UniqueName);
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
            base.topLevelResized();
            refreshFileTabs();
        }

        private void refreshFileTabs()
        {
            clearFileTabs();
            int left = 0;
            foreach (String file in editorController.OpenFiles)
            {
                EditorTaskbarFileButton fileButton = new EditorTaskbarFileButton(widget, file, left);
                fileButton.CurrentFile = currentFile == file;
                fileButton.ChangeFile += new Action<EditorTaskbarFileButton>(fileButton_ChangeFile);
                fileButton.Closed += new Action<EditorTaskbarFileButton>(fileButton_Closed);
                left += fileButton.Width;
                if (left > fileListButton.Left)
                {
                    fileButton.Dispose();
                    break;
                }
                else
                {
                    fileButtons.Add(fileButton);
                }
            }
        }

        private void clearFileTabs()
        {
            foreach (EditorTaskbarFileButton fileButton in fileButtons)
            {
                fileButton.Dispose();
            }
            fileButtons.Clear();
        }

        void fileListButton_MouseButtonClick(Widget source, EventArgs e)
        {
            PopupMenu popupMenu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", source.AbsoluteLeft, source.AbsoluteTop + source.Height, 1, 1, Align.Default, "Overlapped", "");
            popupMenu.Visible = false;
            popupMenu.ItemAccept += (menu, evt) =>
            {
                MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)evt;
                editorController.openFile(mcae.Item.ItemName);
            };
            popupMenu.Closed += (menu, evt) =>
            {
                Gui.Instance.destroyWidget(menu);
            };

            foreach (String file in editorController.OpenFiles)
            {
                MenuItem menuItem = popupMenu.addItem(file, MenuItemType.Normal, file);
            }

            LayerManager.Instance.upLayerItem(popupMenu);
            popupMenu.ensureVisible();
            popupMenu.setVisibleSmooth(true);
        }
    }
}
