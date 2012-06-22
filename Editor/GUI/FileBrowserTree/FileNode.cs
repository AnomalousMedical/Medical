using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyGUIPlugin;

namespace Medical.GUI
{
    class FileNode : TreeNode
    {
        public FileNode(String file)
        {
            changePath(file);
            ImageResource = "EditorFileIcon/" + Path.GetExtension(file).ToLowerInvariant();
        }

        public void changePath(String file)
        {
            Text = Path.GetFileName(file);
            FilePath = file;
            ImageResource = "EditorFileIcon/" + Path.GetExtension(file).ToLowerInvariant();
        }

        public String FilePath { get; set; }
    }
}
