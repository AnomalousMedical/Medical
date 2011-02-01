using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    abstract class PromptTextArea : IDisposable
    {
        protected const int EXTRA_HEIGHT = 15;

        public abstract void Dispose();

        public abstract int Top { get; set; }

        public abstract int Bottom { get; }
    }
}
