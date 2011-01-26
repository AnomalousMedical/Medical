using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public delegate void NativeMenuEvent(NativeMenuItem item);

    public class NativeMenuItem : IDisposable
    {
        public event NativeMenuEvent Select;

        private IntPtr nativeMenuItem;
        private NativeMenu subMenu = null;

        internal NativeMenuItem(IntPtr nativeMenuItem)
        {
            this.nativeMenuItem = nativeMenuItem;
            IntPtr subMenuPtr = NativeMenuItem_getSubMenu(nativeMenuItem);
            if (subMenuPtr != IntPtr.Zero)
            {
                subMenu = new NativeMenu(subMenuPtr);
            }
        }

        public void Dispose()
        {
            NativeMenuItem_delete(nativeMenuItem);
        }

        public bool Enabled
        {
            get
            {
                return NativeMenuItem_getEnabled(nativeMenuItem);
            }
            set
            {
                NativeMenuItem_setEnabled(nativeMenuItem, value);
            }
        }

        public int ID
        {
            get
            {
                return NativeMenuItem_getID(nativeMenuItem);
            }
        }

        public NativeMenu SubMenu
        {
            get
            {
                return subMenu;
            }
        }

        public String Help
        {
            get
            {
                using(NativeString ns = new NativeString(NativeMenuItem_getHelp(nativeMenuItem)))
                {
                    return ns.ToString();
                }
            }
            set
            {
                NativeMenuItem_setHelp(nativeMenuItem, value);
            }
        }

        internal IntPtr _NativePtr
        {
            get
            {
                return nativeMenuItem;
            }
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern void NativeMenuItem_delete(IntPtr item);

        [DllImport("OSHelper")]
        private static extern void NativeMenuItem_setEnabled(IntPtr item, bool value);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool NativeMenuItem_getEnabled(IntPtr item);

        [DllImport("OSHelper")]
        private static extern int NativeMenuItem_getID(IntPtr item);

        [DllImport("OSHelper")]
        private static extern IntPtr NativeMenuItem_getSubMenu(IntPtr item);

        [DllImport("OSHelper")]
        private static extern void NativeMenuItem_setHelp(IntPtr item, String helpText);

        [DllImport("OSHelper")]
        private static extern IntPtr NativeMenuItem_getHelp(IntPtr item);

        #endregion
    }
}
