using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical.GUI
{
    class RawFilesystemArchiveFactory : OgreManagedArchiveFactory
    {
        public RawFilesystemArchiveFactory()
            :base("RawFilesystemArchive")
        {

        }

        protected override OgreManagedArchive doCreateInstance(string name)
        {
            return new RawFilesystemArchive(name, "RawFilesystemArchive");
        }
    }
}
