using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

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
        IntPtr nativeMenu;

        internal NativeMenu(IntPtr nativeMenu)
        {
            this.nativeMenu = nativeMenu;
        }

        public NativeMenuItem append(CommonMenuItems id, String text, String helpText)
        {
            return append(id, text, helpText, false);
        }

        public NativeMenuItem append(CommonMenuItems id, String text, String helpText, bool subMenu)
        {
            return new NativeMenuItem(NativeMenu_append(nativeMenu, id, text, helpText, subMenu));
        }

        public void appendSeparator()
        {
            NativeMenu_appendSeparator(nativeMenu);
        }

        public void insert(int index, NativeMenuItem menuItem)
        {
            NativeMenu_insertItem(nativeMenu, index, menuItem._NativePtr);
        }

        public NativeMenuItem insert(int index, CommonMenuItems id, String text, String helpText)
        {
            return insert(index, id, text, helpText, false);
        }

        public NativeMenuItem insert(int index, CommonMenuItems id, String text, String helpText, bool subMenu)
        {
            return new NativeMenuItem(NativeMenu_insert(nativeMenu, index, id, text, helpText, subMenu));
        }

        public void remove(NativeMenuItem menuItem)
        {
            NativeMenu_remove(nativeMenu, menuItem._NativePtr);
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr NativeMenu_append(IntPtr menu, CommonMenuItems id, String text, String helpText, bool subMenu);

        [DllImport("OSHelper")]
        private static extern void NativeMenu_appendSeparator(IntPtr menu);

        [DllImport("OSHelper")]
        private static extern IntPtr NativeMenu_insert(IntPtr menu, int index, CommonMenuItems id, String text, String helpText, bool subMenu);

        [DllImport("OSHelper")]
        private static extern void NativeMenu_insertItem(IntPtr menu, int index, IntPtr menuItem);

        [DllImport("OSHelper")]
        private static extern void NativeMenu_remove(IntPtr menu, IntPtr menuItem);

        #endregion
    }
}
