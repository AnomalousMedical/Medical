using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public abstract class App : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate bool OnInitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int OnExitDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OnIdleDelegate();

        OnInitDelegate onInitCB;
        OnExitDelegate onExitCB;
        OnIdleDelegate onIdleCB;

        IntPtr appPtr;

        protected bool restartOnShutdown = false;
        protected bool restartAsAdmin = false;

        public App()
        {
            appPtr = App_create();

            onInitCB = new OnInitDelegate(OnInit);
            onExitCB = new OnExitDelegate(OnExit);
            onIdleCB = new OnIdleDelegate(OnIdle);
            App_registerDelegates(appPtr, onInitCB, onExitCB, onIdleCB);
        }

        public virtual void Dispose()
        {
            App_delete(appPtr);
            appPtr = IntPtr.Zero;
        }

        public void run()
        {
            App_run(appPtr);
        }

        public void exit()
        {
            App_exit(appPtr);
        }

        public void restart(bool asAdmin)
        {
            exit();
            restartOnShutdown = true;
            restartAsAdmin = asAdmin;
        }

        public abstract bool OnInit();

        public abstract int OnExit();

        public abstract void OnIdle();

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr App_create();

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void App_delete(IntPtr app);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void App_registerDelegates(IntPtr app, OnInitDelegate onInitCB, OnExitDelegate onExitCB, OnIdleDelegate onIdleCB);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void App_run(IntPtr app);

        [DllImport(NativePlatformPlugin.LibraryName, CallingConvention=CallingConvention.Cdecl)]
        private static extern void App_exit(IntPtr app);
    }
}
