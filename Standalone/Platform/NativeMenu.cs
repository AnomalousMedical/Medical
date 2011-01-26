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

    public class NativeMenu
    {
        public NativeMenuItem Append(CommonMenuItems id, string text, string helpText)
        {
            throw new NotImplementedException();
        }

        public void AppendSeparator()
        {
            throw new NotImplementedException();
        }

        public NativeMenuItem Append(CommonMenuItems id, string text, NativeMenu subMenu)
        {
            throw new NotImplementedException();
        }

        public void Remove(NativeMenuItem recentDocMenuItem)
        {
            throw new NotImplementedException();
        }

        public void Insert(int p, NativeMenuItem recentDocMenuItem)
        {
            throw new NotImplementedException();
        }

        public NativeMenuItem Insert(int p, int p_2, string p_3)
        {
            throw new NotImplementedException();
        }
    }
}
