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
        NativeOSWindow parentWindow;
        private List<NativeMenuItem> menuItems = new List<NativeMenuItem>();
        private String text;

        internal NativeMenu(NativeOSWindow parentWindow, IntPtr nativeMenu, String text)
        {
            this.nativeMenu = nativeMenu;
            this.parentWindow = parentWindow;
            this.text = text;
        }

        internal void Dispose(bool windowDeleted)
        {
            foreach (NativeMenuItem menuItem in menuItems)
            {
                menuItem.Dispose(windowDeleted);
            }
            menuItems.Clear();
        }

        public NativeMenuItem append(CommonMenuItems id, String text, String helpText)
        {
            return append(id, text, helpText, false);
        }

        public NativeMenuItem append(CommonMenuItems id, String text, String helpText, bool subMenu)
        {
            NativeMenuItem item = new NativeMenuItem(parentWindow, NativeMenu_append(nativeMenu, id, text, helpText, subMenu));
            menuItems.Add(item);
            return item;
        }

        public void appendSeparator()
        {
            NativeMenu_appendSeparator(nativeMenu);
        }

        public void insert(int index, NativeMenuItem menuItem)
        {
            menuItems.Add(menuItem);
            NativeMenu_insertItem(nativeMenu, index, menuItem._NativePtr);
        }

        public NativeMenuItem insert(int index, CommonMenuItems id, String text, String helpText)
        {
            return insert(index, id, text, helpText, false);
        }

        public NativeMenuItem insert(int index, CommonMenuItems id, String text, String helpText, bool subMenu)
        {
            NativeMenuItem item = new NativeMenuItem(parentWindow, NativeMenu_insert(nativeMenu, index, id, text, helpText, subMenu));
            menuItems.Add(item);
            return item;
        }

        public void remove(NativeMenuItem menuItem)
        {
            menuItems.Remove(menuItem);
            NativeMenu_remove(nativeMenu, menuItem._NativePtr);
        }

        public String Text
        {
            get
            {
                return text;
            }
        }

        internal IntPtr _NativePtr
        {
            get
            {
                return nativeMenu;
            }
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
