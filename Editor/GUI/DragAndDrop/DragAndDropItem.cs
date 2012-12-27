using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class DragAndDropItem
    {
        public DragAndDropItem(String name = null, String icon = null)
        {
            this.Name = name;
            this.Icon = icon;
        }

        public String Name { get; set; }

        public String Icon { get; set; }
    }
}
