using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;
using System.IO;

namespace Medical.GUI
{
    public class RocketRawOgreFilesystemArchive : OgreManagedArchive
    {
        public const String ArchiveName = "RocketRawOgreFilesystemArchive";

        public RocketRawOgreFilesystemArchive(String name, String archType)
            :base(name, archType)
        {
            
        }

        protected override void load()
        {
            
        }

        protected override void unload()
        {
            
        }

        protected override Stream doOpen(string filename)
        {
            return File.Open(filename, FileMode.Open, FileAccess.Read);
        }

        protected override void doList(bool recursive, bool dirs, IntPtr ogreStringVector)
        {
            
        }

        protected override void doListFileInfo(bool recursive, bool dirs, IntPtr ogreFileList, IntPtr archive)
        {
            
        }

        protected override void dofind(string pattern, bool recursive, bool dirs, IntPtr ogreStringVector)
        {
            
        }

        protected override void dofindFileInfo(string pattern, bool recursive, bool dirs, IntPtr ogreFileList, IntPtr archive)
        {
            
        }

        protected override bool exists(string filename)
        {
            return File.Exists(filename);
        }
    }
}
