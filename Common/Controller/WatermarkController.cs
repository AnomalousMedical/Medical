using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class WatermarkController
    {
        private DrawingWindowController windowController;
        private Dictionary<DrawingWindow, Watermark> watermarks = new Dictionary<DrawingWindow, Watermark>();
        private Watermark sourceWatermark;

        public WatermarkController(Watermark sourceWatermark, DrawingWindowController windowController)
        {
            this.windowController = windowController;
            this.sourceWatermark = sourceWatermark;
            windowController.WindowCreated += new DrawingWindowEvent(windowController_WindowCreated);
            windowController.WindowDestroyed += new DrawingWindowEvent(windowController_WindowDestroyed);
        }

        void windowController_WindowCreated(DrawingWindow window)
        {
            Watermark watermark = sourceWatermark.clone(window.CameraName);
            watermark.createOverlays();
            window.PreFindVisibleObjects += watermark.preFindVisibleCallback;
            window.Resize += window_Resize;
            watermarks.Add(window, watermark);
        }

        void window_Resize(object sender, EventArgs e)
        {
            DrawingWindow window = sender as DrawingWindow;
            if (window != null)
            {
                watermarks[window].sizeChanged(window.RenderWidth, window.RenderHeight);
            }
        }

        void windowController_WindowDestroyed(DrawingWindow window)
        {
            Watermark watermark = watermarks[window];
            window.PreFindVisibleObjects -= watermark.preFindVisibleCallback;
            window.Resize -= window_Resize;
            watermark.destroyOverlays();
            watermarks.Remove(window);
        }
    }
}
