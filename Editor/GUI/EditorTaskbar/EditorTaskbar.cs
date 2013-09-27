using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class EditorTaskbar : LayoutComponent
    {
        private static int TaskButtonTop = ScaleHelper.Scaled(20);
        private static int TaskButtonWidth = ScaleHelper.Scaled(48);
        private static int TaskButtonHeight = ScaleHelper.Scaled(48);
        private static int TaskButtonPadding = ScaleHelper.Scaled(2);

        private String closeAction;
        private List<EditorTaskbarFileButton> fileButtons = new List<EditorTaskbarFileButton>();
        private EditorController editorController;
        private String currentFile;
        private Button fileListButton;
        private bool allowFileRefresh = true;
        private EditorFilesystemWatcher filesystemWatcher;
        private ResourceCache resourceCache;

        public EditorTaskbar(EditorTaskbarView view, MyGUIViewHost viewHost, EditorController editorController, EditorFilesystemWatcher filesystemWatcher)
            :base("Medical.GUI.EditorTaskbar.EditorTaskbar.layout", viewHost)
        {
            closeAction = view.CloseAction;
            this.editorController = editorController;
            this.currentFile = view.File;
            this.filesystemWatcher = filesystemWatcher;
            this.resourceCache = editorController.ResourceProvider.ResourceCache;

            fileListButton = (Button)widget.findWidget("FileListButton");
            fileListButton.MouseButtonClick += new MyGUIEvent(fileListButton_MouseButtonClick);

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

            resourceCache.CachedResourceAdded += ResourceCache_CachedResourceAdded;
            filesystemWatcher.FileRenamed += ResourceProvider_FileRenamed;
            filesystemWatcher.FileDeleted += ResourceProvider_FileDeleted;
        }

        public override void Dispose()
        {
            resourceCache.CachedResourceAdded -= ResourceCache_CachedResourceAdded;
            filesystemWatcher.FileRenamed -= ResourceProvider_FileRenamed;
            filesystemWatcher.FileDeleted -= ResourceProvider_FileDeleted;
            clearFileTabs();
            base.Dispose();
        }

        public override void closing()
        {
            base.closing();
            allowFileRefresh = false;
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
                    editorController.openEditor(editorController.OpenFiles.First());
                }
            }
            else
            {
                refreshFileTabs();
            }
        }

        void fileButton_ChangeFile(EditorTaskbarFileButton obj)
        {
            editorController.openEditor(obj.File);
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            refreshFileTabs();
        }

        void ResourceProvider_FileRenamed(string path, string oldPath, bool isDirectory)
        {
            if (currentFile == System.IO.Path.GetFileName(oldPath))
            {
                ViewHost.Context.runAction(closeAction, ViewHost);
            }
            else
            {
                refreshFileTabs();
            }
        }

        void ResourceProvider_FileDeleted(string path)
        {
            if (currentFile == System.IO.Path.GetFileName(path))
            {
                ViewHost.Context.runAction(closeAction, ViewHost);
            }
            else
            {
                refreshFileTabs();
            }
        }

        void ResourceCache_CachedResourceAdded(ResourceCache resourceCache, CachedResource resource)
        {
            refreshFileTabs();
        }

        private void refreshFileTabs()
        {
            if (allowFileRefresh)
            {
                clearFileTabs();
                int left = 0;
                int top = fileListButton.Top;
                foreach (String file in editorController.OpenFiles)
                {
                    EditorTaskbarFileButton fileButton = new EditorTaskbarFileButton(widget, file, left, top);
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
                editorController.openEditor(mcae.Item.ItemName);
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
