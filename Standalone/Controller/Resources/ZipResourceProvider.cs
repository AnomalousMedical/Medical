﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZipAccess;
using Logging;
using System.Xml;

namespace Medical
{
    /// <summary>
    /// This resource provider will read files from a zipped file. Don't forget to dispose it.
    /// </summary>
    public class ZipResourceProvider : ResourceProvider, IDisposable
    {
        private String resourceLocation;
        private ZipFile zipFile;

        public ZipResourceProvider(String resourceLocation)
        {
            this.resourceLocation = resourceLocation;
            zipFile = new ZipFile(resourceLocation);
        }

        public void Dispose()
        {
            if (zipFile != null)
            {
                zipFile.Dispose();
            }
        }

        public Stream openFile(string filename)
        {
            return zipFile.openFile(filename);
        }

        public Stream openWriteStream(String filename)
        {
            return new WriteStream(new MemoryStream(), this)
            {
                FileName = filename
            };
        }

        public void addFile(String path, string targetDirectory)
        {
            zipFile.Dispose();
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                ionicZip.UpdateFile(path);
                ionicZip.Save();
            }
            zipFile = new ZipFile(resourceLocation);
        }

        public void delete(String filename)
        {
            //Use our zip archive to figure out what to delete. Ours can find directories even if they do not physically exist in the zip file.
            List<String> removeFiles = new List<string>();
            //Figure out what we are dealing with
            var fileInfo = zipFile.getFileInfo(filename);
            if (fileInfo.IsDirectory)
            {
                filename += '/';
                removeFiles.Add(filename);
                removeFiles.AddRange(zipFile.listDirectories(filename, "*", true).Select(e => e.FullName));
                removeFiles.AddRange(zipFile.listFiles(filename, "*", true).Select(e => e.FullName));
            }
            else
            {
                removeFiles.Add(filename);
            }
            if (removeFiles.Count > 0)
            {
                zipFile.Dispose();
                using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
                {
                    foreach (String file in removeFiles)
                    {
                        //Have to check to make sure the file exists or the ionic library throws an exception
                        var entry = ionicZip[file];
                        if (entry != null)
                        {
                            ionicZip.RemoveEntry(entry);
                        }
                    }
                    ionicZip.Save();
                }
                zipFile = new ZipFile(resourceLocation);
            }
        }

        public IEnumerable<String> listFiles(String pattern)
        {
            IEnumerable<ZipFileInfo> zipFiles = null;
            try
            {
                zipFiles = zipFile.listFiles("/", pattern, false);
            }
            catch (Exception ex)
            {
                Log.Error("Could not list files in directory {0}.\nReason: {1}", resourceLocation, ex.Message);
            }
            if (zipFiles != null)
            {
                foreach (ZipFileInfo info in zipFiles)
                {
                    yield return info.FullName;
                }
            }
        }

        public IEnumerable<String> listFiles(String pattern, String directory, bool recursive)
        {
            IEnumerable<ZipFileInfo> zipFiles = null;
            try
            {
                zipFiles = zipFile.listFiles(directory, pattern, recursive);
            }
            catch (Exception ex)
            {
                Log.Error("Could not list files in directory {0}.\nReason: {1}", Path.Combine(resourceLocation, directory), ex.Message);
            }
            if (zipFiles != null)
            {
                foreach (ZipFileInfo info in zipFiles)
                {
                    yield return info.FullName;
                }
            }
        }

        public IEnumerable<String> listDirectories(String pattern, String directory, bool recursive)
        {
            IEnumerable<ZipFileInfo> zipDirs = null;
            try
            {
                zipDirs = zipFile.listDirectories(directory, pattern, recursive);
            }
            catch (Exception ex)
            {
                Log.Error("Could not list directories in directory {0}.\nReason: {1}", Path.Combine(resourceLocation, directory), ex.Message);
            }
            if (zipDirs != null)
            {
                foreach (ZipFileInfo info in zipDirs)
                {
                    String fullName = info.FullName;
                    int len = fullName.Length;
                    if (len > 0) //Zip archive directories always end with /
                    {
                        yield return fullName.Substring(0, len - 1);
                    }
                }
            }
        }

        public bool directoryHasEntries(String path)
        {
            return listFiles("*", path, true).Any() || listDirectories("*", path, true).Any();
        }

        public bool exists(String path)
        {
            return zipFile.exists(path);
        }

        public String getFullFilePath(String filename)
        {
            return Path.Combine(resourceLocation, filename);
        }

        public void createDirectory(string path, string directoryName)
        {
            zipFile.Dispose();
            path = Path.Combine(path, directoryName);
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                if (!ionicZip.ContainsEntry(Path.GetFileName(path)))
                {
                    ionicZip.AddDirectoryByName(path);
                }
                ionicZip.Save();
            }
            zipFile = new ZipFile(resourceLocation);
        }

        public bool isDirectory(String path)
        {
            var info = zipFile.getFileInfo(path);
            return info != null && info.IsDirectory;
        }

        public String BackingLocation
        {
            get
            {
                return resourceLocation;
            }
        }

        private void writeStreamClosed(WriteStream writeStream)
        {
            zipFile.Dispose();
            using (var ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                ionicZip.UpdateEntry(writeStream.FileName, writeStream.BaseStream);
                ionicZip.Save();
            }
            zipFile = new ZipFile(resourceLocation);
        }

        public void move(string oldPath, string newPath)
        {
            zipFile.Dispose();
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                var entry = ionicZip[oldPath];
                if (entry != null)
                {
                    if (entry.IsDirectory)
                    {
                        foreach (var subEntry in ionicZip.EntriesStartingWith(oldPath))
                        {
                            subEntry.FileName = Path.Combine(newPath, Path.GetFileName(subEntry.FileName));
                        }
                    }
                    else
                    {
                        entry.FileName = newPath;
                    }
                    ionicZip.Save();
                }
            }
            zipFile = new ZipFile(resourceLocation);
        }

        public void copy(string from, string to)
        {
            zipFile.Dispose();
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                var entry = ionicZip[from];
                if (entry != null)
                {
                    if (entry.IsDirectory)
                    {
                        foreach (var subEntry in ionicZip.EntriesStartingWith(from))
                        {
                            String toPath = Path.Combine(to, Path.GetFileName(subEntry.FileName));
                            ionicZip.UpdateEntry(toPath, (name, stream) => entry.Extract(stream));
                        }
                    }
                    else
                    {
                        ionicZip.UpdateEntry(to, (name, stream) => entry.Extract(stream));
                    }
                    ionicZip.Save();
                }
            }
            zipFile = new ZipFile(resourceLocation);
        }

        class WriteStream : Stream
        {
            private Stream baseStream;
            private ZipResourceProvider resourceProvider;

            public WriteStream(Stream baseStream, ZipResourceProvider resourceProvider)
            {
                this.baseStream = baseStream;
                this.resourceProvider = resourceProvider;
            }

            public override void Close()
            {
                baseStream.Seek(0, SeekOrigin.Begin);
                resourceProvider.writeStreamClosed(this);
                baseStream.Close();
            }

            public override bool CanRead
            {
                get
                {
                    return baseStream.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return baseStream.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return baseStream.CanWrite;
                }
            }

            public override void Flush()
            {
                baseStream.Flush();
            }

            public override long Length
            {
                get
                {
                    return baseStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return baseStream.Position;
                }
                set
                {
                    baseStream.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return baseStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                baseStream.Write(buffer, offset, count);
            }

            public String FileName { get; set; }

            public Stream BaseStream
            {
                get
                {
                    return baseStream;
                }
            }
        }
    }
}
