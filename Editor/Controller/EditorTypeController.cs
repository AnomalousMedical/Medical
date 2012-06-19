using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Medical.GUI;

namespace Medical
{
    public abstract class EditorTypeController
    {
        public EditorTypeController(String extension)
        {
            this.Extension = extension;
        }

        public String Extension { get; private set; }

        public abstract void openFile(String file);

        public abstract void closeFile(String file);

        public virtual void fileChanged(FileSystemEventArgs e, String extension)
        {

        }

        public virtual void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            
        }
    }
}
