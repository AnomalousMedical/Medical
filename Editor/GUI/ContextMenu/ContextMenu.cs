using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class ContextMenu
    {
        private List<ContextMenuItem> menuItems = new List<ContextMenuItem>();

        public void add(ContextMenuItem item)
        {
            menuItems.Add(item);
        }

        public IEnumerable<ContextMenuItem> Items
        {
            get
            {
                return menuItems;
            }
        }
    }
}
