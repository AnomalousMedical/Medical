using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Medical.GUI
{
    public enum CursorType
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

    public class MainWindow : NativeOSWindow
    {
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

        private String windowDefaultText;
        private String windowTitle;

        public static MainWindow Instance { get; private set; }

        public MainWindow(String windowTitle)
            :base(windowTitle, new Point(-1, -1), new Size(800, 600))
        {
            Instance = this;
            this.windowTitle = windowTitle;
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

            Title = PlatformConfig.formatTitle(windowDefaultText, subName);
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

        void pointerManager_ChangeMousePointer(string pointerName)
        {
            setCursor(cursors[pointerName]);
        }
    }
}
