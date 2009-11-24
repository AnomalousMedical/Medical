using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public delegate void DockProviderEvent(DockProvider source);

    public interface DockProvider
    {
        event DockProviderEvent ActiveDocumentChanged;

        bool restoreFromString(String persistString, out String name, out Vector3 translation, out Vector3 lookAt, out int bgColor);

        DrawingWindowHost createWindow(String name, DrawingWindowController controller);

        DrawingWindowHost createCloneWindow(String name, DrawingWindowController controller);

        DrawingWindowHost ActiveDocument { get; }
    }
}
