using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine;
using OgreWrapper;

namespace Medical
{
    class OgreResourceProviderArchive : OgreManagedArchive
    {
        private ResourceProvider resourceProvider;

        public OgreResourceProviderArchive(String name, String archType, ResourceProvider resourceProvider)
            :base(name, archType)
        {
            this.resourceProvider = resourceProvider;
        }

        protected override void load()
        {
            
        }

        protected override void unload()
        {
            resourceProvider = null;
        }

        protected override Stream doOpen(string filename)
        {
            return resourceProvider.openFile(filename);
        }

        protected override void doList(bool recursive, bool dirs, IntPtr ogreStringVector)
        {
            //String[] files;
            //if(dirs)
            //{
            //    files = resourceProvider.listDirectories("*", "", recursive);
            //}
            //else
            //{
            //    files = resourceProvider.listFiles("*", "", recursive);
            //}
            //foreach(String file in files)
            //{
            //    OgreStringVector_push_back(ogreStringVector, file);
            //}
        }

        protected override void doListFileInfo(bool recursive, bool dirs, IntPtr ogreFileList, IntPtr archive)
        {
            //String[] files;
            //if(dirs)
            //{
            //    files = vfs.listDirectories(baseName, recursive);
            //    foreach(String file in files)
            //    {
            //        OgreFileInfoList_push_back(ogreFileList, archive, new IntPtr(-1), new IntPtr(-1), Path.GetFileName(file), file, Path.GetDirectoryName(file));
            //    }
            //}
            //else
            //{
            //    files = vfs.listFiles(baseName, recursive);
            //    foreach(String file in files)
            //    {
            //        pushFixedFileInfo(file, ogreFileList, archive);
            //    }
            //}
        }

        protected override void dofind(string pattern, bool recursive, bool dirs, IntPtr ogreStringVector)
        {
            //String[] files;
            //if(dirs)
            //{
            //    files = vfs.listDirectories(baseName, pattern, recursive);
            //}
            //else
            //{
            //    files = vfs.listFiles(baseName, pattern, recursive);
            //}
            //foreach(String file in files)
            //{
            //    OgreStringVector_push_back(ogreStringVector, file);
            //}
        }

        protected override void dofindFileInfo(string pattern, bool recursive, bool dirs, IntPtr ogreFileList, IntPtr archive)
        {
            //String[] files;
            //if (dirs)
            //{
            //    files = vfs.listDirectories(baseName, pattern, recursive);
            //    foreach (String file in files)
            //    {
            //        OgreFileInfoList_push_back(ogreFileList, archive, new IntPtr(-1), new IntPtr(-1), Path.GetFileName(file), file, Path.GetDirectoryName(file));
            //    }
            //}
            //else
            //{
            //    files = vfs.listFiles(baseName, pattern, recursive);
            //    foreach (String file in files)
            //    {
            //        pushFixedFileInfo(file, ogreFileList, archive);
            //    }
            //}
        }

        //protected void pushFixedFileInfo(String file, IntPtr ogreFileList, IntPtr archive)
        //{
        //    VirtualFileInfo archiveInfo = vfs.getFileInfo(file);
        //    String fixedFilename = archiveInfo.FullName.Replace(baseName, "");
        //    if(fixedFilename.StartsWith("/"))
        //    {
        //        fixedFilename = fixedFilename.Substring(1);
        //    }
        //    String fixedPath = archiveInfo.DirectoryName.Replace(baseName, "");
        //    if(fixedPath.StartsWith("/"))
        //    {
        //        fixedPath = fixedPath.Substring(1);
        //    }
        //    OgreFileInfoList_push_back(ogreFileList, archive, new IntPtr(archiveInfo.CompressedSize), new IntPtr(archiveInfo.UncompressedSize), archiveInfo.Name, fixedFilename, fixedPath);
        //}

        protected override bool exists(string filename)
        {
            return !String.IsNullOrEmpty(filename) && resourceProvider.exists(filename);
        }
    }
}
