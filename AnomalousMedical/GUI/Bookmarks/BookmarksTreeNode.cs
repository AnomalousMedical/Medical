using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class BookmarksTreeNode : TreeNode
    {
        private BookmarksTreeNodeWidget bmkWidget;

        public BookmarksTreeNode(String text, BookmarksTreeNodeWidget widget)
            :base(text, widget)
        {
            this.bmkWidget = widget;
        }

        public void showHover(bool hovered)
        {
            bmkWidget.showHover(hovered, Selected);
        }
    }
}
