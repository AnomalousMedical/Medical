using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class ColorDialog : IDisposable
    {
        public ColorDialog()
        {

        }

        public void Dispose()
        {

        }

        public NativeDialogResult showModal()
        {
            throw new NotImplementedException();
        }

        public Color Color { get; set; }
    }
}
