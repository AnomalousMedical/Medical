using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public abstract class EditorTypeController
    {
        public EditorTypeController(String extension)
        {
            this.Extension = extension;
        }

        public String Extension { get; private set; }

        public abstract void openFile(String fullPath);

        public virtual void fileChanged(FileSystemEventArgs e, String extension)
        {

        }
    }
}
