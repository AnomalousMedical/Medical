using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface EditorTypeController
    {
        bool canOpenFile(String extension);

        void openFile(String fullPath);
    }
}
