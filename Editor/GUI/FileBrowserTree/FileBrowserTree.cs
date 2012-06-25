using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine.Platform;
using Engine;

namespace Medical.GUI
{
    public delegate void FileBrowserEvent(FileBrowserTree tree, String path);
    public delegate void FileBrowserNodeContextEvent(FileBrowserTree tree, String path, bool isDirectory, bool isTopLevel);

    /// <summary>
    /// A lazy loading tree view of a filesystem provided by a resourceProvider.
    /// </summary>
    public class FileBrowserTree : IDisposable
    {
        private readonly char[] SEPS = new char[] { '/', '\\' };

        private IntVector2 lastMouseEventPos;
        private Tree fileTree;
        private ResourceProvider resourceProvider;
        private DirectoryNode baseNode;

        public event FileBrowserEvent FileSelected;
        public event FileBrowserNodeContextEvent NodeContextEvent;

        public FileBrowserTree(ScrollView treeScrollView)
        {
            fileTree = new Tree(treeScrollView);
            fileTree.NodeMouseDoubleClick += new EventHandler<TreeEventArgs>(fileTree_NodeMouseDoubleClick);
            fileTree.NodeMouseReleased += new EventHandler<TreeMouseEventArgs>(fileTree_NodeMouseReleased);
        }

        public void Dispose()
        {
            fileTree.Dispose();
        }

        public void layout()
        {
            fileTree.layout();
        }

        public void setResourceProvider(ResourceProvider resourceProvider)
        {
            fileTree.Nodes.clear();
            this.resourceProvider = resourceProvider;
            if (resourceProvider != null)
            {
                fileTree.SuppressLayout = true;
                baseNode = new DirectoryNode("", this);
                baseNode.Text = Path.GetFileName(resourceProvider.BackingLocation);
                fileTree.Nodes.add(baseNode);
                baseNode.Expanded = true;
                fileTree.SuppressLayout = false;
                fileTree.layout();
            }
        }

        public void fileCreated(string path, bool isDirectory)
        {
            String parentPath = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(parentPath))
            {
                fileTree.SuppressLayout = true;
                if (isDirectory)
                {
                    baseNode.addDirectoryNode(new DirectoryNode(path, this));
                }
                else
                {
                    baseNode.addFileNode(new FileNode(path));
                }
                baseNode.alertHasChildrenChanged();
                fileTree.SuppressLayout = false;
                fileTree.layout();
            }
            else
            {
                DirectoryNode node = findNodeForPath(parentPath) as DirectoryNode;
                if (node != null)
                {
                    node.alertHasChildrenChanged();
                    if (node.ListedChildren)
                    {
                        fileTree.SuppressLayout = true;
                        if (isDirectory)
                        {
                            node.addDirectoryNode(new DirectoryNode(path, this));
                        }
                        else
                        {
                            node.addFileNode(new FileNode(path));
                        }
                        fileTree.SuppressLayout = false;
                        fileTree.layout();
                    }
                }
            }
        }

        public void fileRenamed(string path, string oldPath, bool isDirectory)
        {
            TreeNode node = findNodeForPath(oldPath);
            if (node != null)
            {
                if (isDirectory)
                {
                    ((DirectoryNode)node).changePath(path);
                }
                else
                {
                    ((FileNode)node).changePath(path);
                }
            }
        }

        public void fileDeleted(string path)
        {
            TreeNode node = findNodeForPath(path);
            if (node != null)
            {
                fileTree.SuppressLayout = true;
                if (node.Parent == null)
                {
                    baseNode.Children.remove(node);
                }
                else
                {
                    node.Parent.Children.remove(node);
                }
                node.alertHasChildrenChanged();
                fileTree.SuppressLayout = false;
                fileTree.layout();
            }
        }

        internal void createNodesForPath(TreeNodeCollection parentCollection, String path)
        {
            if (resourceProvider != null)
            {
                fileTree.SuppressLayout = true;
                String[] directories = resourceProvider.listDirectories("*", path, false);
                foreach (String dir in directories)
                {
                    parentCollection.add(new DirectoryNode(dir, this));
                }
                String[] files = resourceProvider.listFiles("*", path, false);
                foreach (String file in files)
                {
                    parentCollection.add(new FileNode(file));
                }
                fileTree.SuppressLayout = false;
            }
        }

        internal bool checkAnyFilesInDirectory(String path)
        {
            if (resourceProvider != null)
            {
                return resourceProvider.directoryHasEntries(path);
            }
            return false;
        }

        void fileTree_NodeMouseDoubleClick(object sender, TreeEventArgs e)
        {
            if (FileSelected != null)
            {
                FileNode fileNode = e.Node as FileNode;
                if (fileNode != null)
                {
                    if (resourceProvider.exists(fileNode.FilePath))
                    {
                        FileSelected.Invoke(this, fileNode.FilePath);
                    }
                }
            }
        }

        TreeNode findNodeForPath(String path)
        {
            String[] names = path.Split(SEPS);
            TreeNode result = null;
            TreeNodeCollection nodes = baseNode.Children;
            for (int i = 0; i < names.Length; ++i)
            {
                result = nodes.findByText(names[i]);
                if (result != null)
                {
                    nodes = result.Children;
                }
                else
                {
                    i = names.Length;
                }
            }
            return result;
        }

        public void showContextMenu(ContextMenu contextMenu)
        {
            PopupMenu popupMenu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1, 1, Align.Default, "Overlapped", "");
            popupMenu.Visible = false;
            popupMenu.ItemAccept += new MyGUIEvent(popupMenu_ItemAccept);
            popupMenu.Closed += new MyGUIEvent(popupMenu_Closed);
            foreach (ContextMenuItem item in contextMenu.Items)
            {
                MenuItem menuItem = popupMenu.addItem(item.Text, MenuItemType.Normal, item.Text);
                menuItem.UserObject = item;
            }
            LayerManager.Instance.upLayerItem(popupMenu);
            popupMenu.setPosition(lastMouseEventPos.x, lastMouseEventPos.y);
            popupMenu.ensureVisible();
            popupMenu.setVisibleSmooth(true);
        }

        void fileTree_NodeMouseReleased(object sender, TreeMouseEventArgs e)
        {
            if (e.Button == MouseButtonCode.MB_BUTTON1)
            {
                lastMouseEventPos = e.MousePosition;
                fileTree.SelectedNode = e.Node;
                DirectoryNode dirNode = e.Node as DirectoryNode;
                if (dirNode != null)
                {
                    if (NodeContextEvent != null)
                    {
                        NodeContextEvent.Invoke(this, dirNode.DirectoryPath, true, dirNode.Parent == null);
                    }
                }
                else
                {
                    FileNode fileNode = e.Node as FileNode;
                    if (fileNode != null)
                    {
                        if (NodeContextEvent != null)
                        {
                            NodeContextEvent.Invoke(this, fileNode.FilePath, false, fileNode.Parent == null);
                        }
                    }
                }
            }
        }

        void popupMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            ((ContextMenuItem)mcae.Item.UserObject).execute();
        }

        void popupMenu_Closed(Widget source, EventArgs e)
        {
            Gui.Instance.destroyWidget(source);
        }
    }
}