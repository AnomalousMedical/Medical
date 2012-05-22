using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    static class FileTreeSorter
    {
        public static int Compare(TreeNode x, TreeNode y)
        {
            if (x is DirectoryNode)
            {
                if (y is DirectoryNode)
                {
                    return NaturalSortAlgorithm.CompareFunc(x.Text, y.Text);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y is DirectoryNode)
                {
                    return 1;
                }
                else
                {
                    return NaturalSortAlgorithm.CompareFunc(x.Text, y.Text);
                }
            }
        }
    }
}
