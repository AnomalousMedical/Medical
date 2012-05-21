using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;

namespace Medical.GUI
{
    class DirectoryNode : TreeNode
    {
        private FileBrowserTree projectExplorer;

        public DirectoryNode(String directory, FileBrowserTree projectExplorer)
        {
            this.projectExplorer = projectExplorer;
            ListedChildren = false;
            changePath(directory);
        }

        public void changePath(String directory)
        {
            Text = Path.GetFileName(directory);
            DirectoryPath = directory;
        }

        public String DirectoryPath { get; set; }

        public bool ListedChildren { get; private set; }

        public override bool HasChildren
        {
            get
            {
                return true;
            }
        }

        public void addDirectoryNode(DirectoryNode node)
        {
            //foreach (TreeNode node in Children)
            //{

            //}
            Children.add(node);
        }

        public void addFileNode(FileNode node)
        {
            Children.add(node);
        }

        protected override void expandedStatusChanged(bool expanding)
        {
            if (expanding && !ListedChildren)
            {
                projectExplorer.createNodesForPath(Children, DirectoryPath);
                ListedChildren = true;
            }
        }
    }
}
