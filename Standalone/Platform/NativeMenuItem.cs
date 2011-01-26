using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void NativeMenuEvent(NativeMenuItem item);

    public class NativeMenuItem : IDisposable
    {
        public void Dispose()
        {

        }

        public event NativeMenuEvent Select;

        public bool Enabled { get; set; }

        public int ID { get; set; }

        public NativeMenu SubMenu { get; set; }

        public String Help { get; set; }
    }
}
