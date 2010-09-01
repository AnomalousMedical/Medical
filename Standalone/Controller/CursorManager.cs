using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace Medical.Controller
{
    class CursorManager : IDisposable
    {
        private Dictionary<string, wx.Cursor> cursors = new Dictionary<string, wx.Cursor>();
        private wx.Frame frame;

        public const String ARROW = "arrow";
        public const String BEAM = "beam";
        public const String SIZE_LEFT = "size_left";
        public const String SIZE_RIGHT = "size_right";
        public const String SIZE_HORZ = "size_horz";
        public const String SIZE_VERT = "size_vert";
        public const String HAND = "hand";
        public const String LINK = "link";

        public CursorManager(wx.Frame frame)
        {
            this.frame = frame;

            cursors.Add(ARROW, new wx.Cursor(wx.StockCursor.wxCURSOR_ARROW));
            cursors.Add(BEAM, new wx.Cursor(wx.StockCursor.wxCURSOR_IBEAM));
            cursors.Add(SIZE_LEFT, new wx.Cursor(wx.StockCursor.wxCURSOR_SIZENWSE));
            cursors.Add(SIZE_RIGHT, new wx.Cursor(wx.StockCursor.wxCURSOR_SIZENESW));
            cursors.Add(SIZE_HORZ, new wx.Cursor(wx.StockCursor.wxCURSOR_SIZEWE));
            cursors.Add(SIZE_VERT, new wx.Cursor(wx.StockCursor.wxCURSOR_SIZENS));
            cursors.Add(HAND, new wx.Cursor(wx.StockCursor.wxCURSOR_SIZING));
            cursors.Add(LINK, new wx.Cursor(wx.StockCursor.wxCURSOR_HAND));

            PointerManager.Instance.ChangeMousePointer += new MousePointerChanged(Instance_ChangeMousePointer);
        }

        public void Dispose()
        {
            foreach (wx.Cursor cursor in cursors.Values)
            {
                cursor.Dispose();
            }
            cursors.Clear();
        }

        void Instance_ChangeMousePointer(string pointerName)
        {
            wx.Cursor newCursor;
            if (cursors.TryGetValue(pointerName, out newCursor))
            {
                frame.Cursor = newCursor;
            }
            else
            {
                Log.Warning("Could not find a cursor named {0}.", pointerName);
            }
        }
    }
}
