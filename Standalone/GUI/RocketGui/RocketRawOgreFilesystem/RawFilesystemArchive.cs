using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine;
using System.IO;

namespace Medical.GUI
{
    public class RawFilesystemArchive : OgreManagedArchive
    {
        public RawFilesystemArchive(String name, String archType)
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
            if (File.Exists(filename))
            {
                return File.Open(filename, FileMode.Open, FileAccess.Read);
            }

            if (!String.IsNullOrEmpty(DirectoryHint))
            {
                String combinedPath = Path.Combine(DirectoryHint, filename);
                if (File.Exists(combinedPath))
                {
                    return File.Open(combinedPath, FileMode.Open, FileAccess.Read);
                }
            }
            throw new FileNotFoundException(String.Format("Cannot find file '{0}' or in hint path '{1}'", filename, DirectoryHint), filename);
        }

        protected override void doList(bool recursive, bool dirs, IntPtr ogreStringVector)
        {
            Logging.Log.Debug("doList {0} {1}", recursive, dirs);
        }

        protected override void doListFileInfo(bool recursive, bool dirs, IntPtr ogreFileList, IntPtr archive)
        {
            Logging.Log.Debug("doListFileInfo {0} {1}", recursive, dirs);
        }

        protected override void dofind(string pattern, bool recursive, bool dirs, IntPtr ogreStringVector)
        {
            Logging.Log.Debug("doFind {0} {1} {2}", pattern, recursive, dirs);
        }

        protected override void dofindFileInfo(string pattern, bool recursive, bool dirs, IntPtr ogreFileList, IntPtr archive)
        {
            Logging.Log.Debug("dofindFileInfo {0} {1} {2}", pattern, recursive, dirs);
        }

        protected override bool exists(string filename)
        {
            if (File.Exists(filename))
            {
                return true;
            }
            if(!String.IsNullOrEmpty(DirectoryHint) && File.Exists(Path.Combine(DirectoryHint, filename)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This is a hint of where to look for resources if the initial path does not work.
        /// </summary>
        public static String DirectoryHint { get; set; }
    }
}
