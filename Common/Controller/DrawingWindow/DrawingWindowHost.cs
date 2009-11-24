using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Medical
{
    public interface DrawingWindowHost : IDisposable
    {
        DrawingWindow DrawingWindow { get; }

        String Text { get; set; }

        void ShowWindow();

        void ShowWindow(DrawingWindowHost parent, DrawingWindowPosition position);

        void Close();

        Size Size { get; set; }
    }
}
