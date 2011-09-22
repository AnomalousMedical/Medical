using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class NativeStringEnumerator : IEnumerator<String>
    {
        private IntPtr nativeEnum;

        public NativeStringEnumerator()
        {
            nativeEnum = NativeStringEnumerator_new();
        }

        public void Dispose()
        {
            NativeStringEnumerator_delete(nativeEnum);
        }

        public bool MoveNext()
        {
            return NativeStringEnumerator_moveNext(nativeEnum);
        }

        public void Reset()
        {
            NativeStringEnumerator_reset(nativeEnum);
        }

        public string Current
        {
            get
            {
                using (NativeString ns = new NativeString(NativeStringEnumerator_getCurrent(nativeEnum)))
                {
                    return ns.ToString();
                }
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public NativeStringEnumerator GetEnumerator()
        {
            return this;
        }

        internal IntPtr _NativePtr
        {
            get
            {
                return nativeEnum;
            }
        }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeStringEnumerator_new();

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeStringEnumerator_delete(IntPtr enumerator);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr NativeStringEnumerator_getCurrent(IntPtr enumerator);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool NativeStringEnumerator_moveNext(IntPtr enumerator);

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void NativeStringEnumerator_reset(IntPtr enumerator);

        #endregion
    }
}
