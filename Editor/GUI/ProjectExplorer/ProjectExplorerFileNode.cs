using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ProjectExplorerFileNode : TreeNode
    {
        public ProjectExplorerFileNode(String file)
        {
            Text = Path.GetFileName(file);
            FilePath = file;
        }

        public String FilePath { get; set; }
    }
}
