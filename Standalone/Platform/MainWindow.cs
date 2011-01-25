using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Medical.GUI
{
    public class MainWindow : IDisposable
    {
        enum CursorType
        {
            Arrow = 0,
            Beam = 1,
            SizeLeft = 2,
            SizeRight = 3,
            SizeHorz = 4,
            SizeVert = 5,
            Hand = 6,
            Link = 7,
        }

        private static Dictionary<string, CursorType> cursors = new Dictionary<string, CursorType>();
        public const String ARROW = "arrow";
        public const String BEAM = "beam";
        public const String SIZE_LEFT = "size_left";
        public const String SIZE_RIGHT = "size_right";
        public const String SIZE_HORZ = "size_horz";
        public const String SIZE_VERT = "size_vert";
        public const String HAND = "hand";
        public const String LINK = "link";

        static MainWindow()
        {
            cursors.Add(ARROW, CursorType.Arrow);
            cursors.Add(BEAM, CursorType.Beam);
            cursors.Add(SIZE_LEFT, CursorType.SizeLeft);
            cursors.Add(SIZE_RIGHT, CursorType.SizeRight);
            cursors.Add(SIZE_HORZ, CursorType.SizeHorz);
            cursors.Add(SIZE_VERT, CursorType.SizeVert);
            cursors.Add(HAND, CursorType.Hand);
            cursors.Add(LINK, CursorType.Link);
        }

        private delegate void DeleteDelegate();
        private DeleteDelegate deleteCB;

        private String windowDefaultText;
        private String windowTitle = "Piper's Joint Based Occlusion";

        public static MainWindow Instance { get; private set; }

        private IntPtr mainWindowPtr;

        public MainWindow()
        {
            deleteCB = new DeleteDelegate(delete);
            mainWindowPtr = MainWindow_create(windowTitle, 800, 600, deleteCB);

            Instance = this;

            RenderWindow = new WxOSWindow(MainWindow_getOSWindow(mainWindowPtr));
            InputWindow = RenderWindow;
        }

        public void Dispose()
        {
            if (mainWindowPtr != IntPtr.Zero)
            {
                MainWindow_destroy(mainWindowPtr);
                mainWindowPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// This is called by the native side if the window is deleted.
        /// </summary>
        private void delete()
        {
            mainWindowPtr = IntPtr.Zero;
        }

        public void setPointerManager(MyGUIPlugin.PointerManager pointerManager)
        {
            pointerManager.ChangeMousePointer += new MyGUIPlugin.MousePointerChanged(pointerManager_ChangeMousePointer);
        }

        /// <summary>
        /// Update the title of the window to reflect a current filename or other info.
        /// </summary>
        /// <param name="subName">A name to place as a secondary name in the title.</param>
        public void updateWindowTitle(String subName)
        {
            if (windowDefaultText == null)
            {
                windowDefaultText = this.Title;
            }

#if WINDOWS
            Title = String.Format("{0} - {1}", windowDefaultText, subName);
#elif MAC_OSX
            Title = subName;
#endif
        }

        /// <summary>
        /// Clear the window title back to the default text.
        /// </summary>
        public void clearWindowTitle()
        {
            if (windowDefaultText != null)
            {
                Title = windowDefaultText;
            }
        }

        public void showFullScreen()
        {
            MainWindow_showFullScreen(mainWindowPtr);
        }

        public void setSize(int width, int height)
        {
            MainWindow_setSize(mainWindowPtr, width, height);
        }

        public void show()
        {
            MainWindow_show(mainWindowPtr);
        }

        public void close()
        {
            MainWindow_close(mainWindowPtr);
        }

        public bool Maximized
        {
            get
            {
                return MainWindow_getMaximized(mainWindowPtr);
            }
            set
            {
                MainWindow_setMaximized(mainWindowPtr, value);
            }
        }

        public bool Active
        {
            get
            {
                return mainWindowPtr != IntPtr.Zero;
            }
        }

        public OSWindow RenderWindow { get; private set; }

        public OSWindow InputWindow { get; private set; }

        public String Title
        {
            get
            {
                return windowTitle;
            }
            set
            {
                windowTitle = value;
                MainWindow_setTitle(mainWindowPtr, value);
            }
        }

        void pointerManager_ChangeMousePointer(string pointerName)
        {
            MainWindow_setCursor(mainWindowPtr, cursors[pointerName]);
        }

#region PInvoke

        [DllImport("OSHelper")]
        private static extern IntPtr MainWindow_create(String caption, int width, int height, DeleteDelegate deleteCB);

        [DllImport("OSHelper")]
        private static extern void MainWindow_destroy(IntPtr mainWindow);

        [DllImport("OSHelper")]
        private static extern void MainWindow_setTitle(IntPtr mainWindow, String title);

        [DllImport("OSHelper")]
        private static extern void MainWindow_setSize(IntPtr mainWindow, int width, int height);

        [DllImport("OSHelper")]
        private static extern void MainWindow_showFullScreen(IntPtr mainWindow);

        [DllImport("OSHelper")]
        private static extern void MainWindow_show(IntPtr mainWindow);

        [DllImport("OSHelper")]
        private static extern void MainWindow_close(IntPtr mainWindow);

        [DllImport("OSHelper")]
        private static extern void MainWindow_setMaximized(IntPtr mainWindow, bool maximize);

        [DllImport("OSHelper")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MainWindow_getMaximized(IntPtr mainWindow);

        [DllImport("OSHelper")]
        private static extern void MainWindow_setCursor(IntPtr mainWindow, CursorType cursor);

        [DllImport("OSHelper")]
        private static extern IntPtr MainWindow_getOSWindow(IntPtr mainWindow);

#endregion

    }
}
