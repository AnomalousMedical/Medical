using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public enum CommonMenuItems
    {
        New,
        Open, 
        Save,
        SaveAs,
        Exit,
        Preferences,
        Help,
        About,
        AutoAssign = -1
    }

    public class Menu
    {
        public MenuEntry Append(CommonMenuItems id, string text, string helpText)
        {
            throw new NotImplementedException();
        }

        public void AppendSeparator()
        {
            throw new NotImplementedException();
        }

        public MenuEntry Append(CommonMenuItems id, string text, Menu subMenu)
        {
            throw new NotImplementedException();
        }

        public void Remove(MenuEntry recentDocMenuItem)
        {
            throw new NotImplementedException();
        }

        public void Insert(int p, MenuEntry recentDocMenuItem)
        {
            throw new NotImplementedException();
        }

        public MenuEntry Insert(int p, int p_2, string p_3)
        {
            throw new NotImplementedException();
        }
    }
}
