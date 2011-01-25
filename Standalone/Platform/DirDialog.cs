using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class DirDialog : IDisposable
    {
        public DirDialog(String message, String startPath)
        {

        }

        public void Dispose()
        {

        }

        public NativeDialogResult showModal()
        {
            throw new NotImplementedException();
        }

        public String Path { get; set; }
    }
}
