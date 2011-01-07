using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using System.Reflection;

namespace Medical.GUI
{
    public interface AppMenu : IDisposable
    {
        void show(int x, int y);

        int Width { get; }

        int Height { get; }
    }
}
