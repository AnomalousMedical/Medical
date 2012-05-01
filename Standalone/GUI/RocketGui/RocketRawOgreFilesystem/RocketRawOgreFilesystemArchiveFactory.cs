using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical.GUI
{
    class RocketRawOgreFilesystemArchiveFactory : OgreManagedArchiveFactory
    {
        public RocketRawOgreFilesystemArchiveFactory()
            :base(RocketRawOgreFilesystemArchive.ArchiveName)
        {

        }

        protected override OgreManagedArchive doCreateInstance(string name)
        {
            return new RocketRawOgreFilesystemArchive(name, RocketRawOgreFilesystemArchive.ArchiveName);
        }
    }
}
