using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    /// <summary>
    /// This class provides a temporary container for native strings that need
    /// to be allocated on the heap in native classes. It should be constructed
    /// with the results of the function that created the temp string on the
    /// native side and disposed when you are done. A using statement is best.
    /// </summary>
    class NativeString : IDisposable
    {
        IntPtr nativeString;

        public NativeString(IntPtr nativeString)
        {
            this.nativeString = nativeString;
        }

        public void Dispose()
        {
            NativeString_delete(nativeString);
        }

        public override string ToString()
        {
            return Marshal.PtrToStringAnsi(NativeString_c_str(nativeString));
        }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern void NativeString_delete(IntPtr nativeString);

        [DllImport("OSHelper")]
        private static extern IntPtr NativeString_c_str(IntPtr nativeString);

        #endregion
    }
}
