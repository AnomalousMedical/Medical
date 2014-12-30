using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;
using Engine;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class MainWindow : NativeOSWindow
    {
        private static Dictionary<string, CursorType> cursors = new Dictionary<string, CursorType>();

        static MainWindow()
        {
            cursors.Add(PointerManager.ARROW, CursorType.Arrow);
            cursors.Add(PointerManager.BEAM, CursorType.Beam);
            cursors.Add(PointerManager.SIZE_LEFT, CursorType.SizeLeft);
            cursors.Add(PointerManager.SIZE_RIGHT, CursorType.SizeRight);
            cursors.Add(PointerManager.SIZE_HORZ, CursorType.SizeHorz);
            cursors.Add(PointerManager.SIZE_VERT, CursorType.SizeVert);
            cursors.Add(PointerManager.HAND, CursorType.Hand);
            cursors.Add(PointerManager.LINK, CursorType.Link);
        }

        private String windowDefaultText;
        private String windowTitle;

        public static MainWindow Instance { get; private set; }

        public MainWindow(String windowTitle)
            : base(windowTitle, new IntVector2(-1, -1), new IntSize2(MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes))
        {
            Instance = this;
            this.windowTitle = windowTitle;
        }

        public void setPointerManager(PointerManager pointerManager)
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
