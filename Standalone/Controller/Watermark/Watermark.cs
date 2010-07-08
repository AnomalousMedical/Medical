using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    public interface Watermark : IDisposable
    {
        bool Visible { get; set; }
    }
}
