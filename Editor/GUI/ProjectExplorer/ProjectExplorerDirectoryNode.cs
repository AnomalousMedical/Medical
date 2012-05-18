using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ProjectExplorerDirectoryNode : TreeNode
    {
        private ProjectExplorer projectExplorer;

        public ProjectExplorerDirectoryNode(String directory, ProjectExplorer projectExplorer)
        {
            this.projectExplorer = projectExplorer;

            Text = Path.GetFileName(directory);
            DirectoryPath = directory;
        }

        public String DirectoryPath { get; set; }

        public override bool HasChildren
        {
            get
            {
                return true;
            }
        }

        protected override void expandedStatusChanged()
        {
            if (Children.Count == 0)
            {
                projectExplorer.createNodesForPath(Children, DirectoryPath);
            }
        }
    }
}
