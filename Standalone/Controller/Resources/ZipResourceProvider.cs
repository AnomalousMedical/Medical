using System;
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
        }

        private String resourceLocation;
        private ZipFile zipFile;
        private HashSet<WriteStream> openWriteStreams = new HashSet<WriteStream>();
        private Ionic.Zip.ZipFile writeStreamFile;

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
            if (writeStreamFile != null)
            {
                writeStreamFile.Dispose();
            }
        }

        public Stream openFile(string filename)
        {
            return zipFile.openFile(filename);
        }

        public Stream openWriteStream(String filename)
        {
            if (writeStreamFile == null)
            {
                zipFile.Dispose();
                writeStreamFile = new Ionic.Zip.ZipFile(resourceLocation);
                zipFile = null;
            }
            WriteStream stream = new WriteStream(new MemoryStream(), this);
            openWriteStreams.Add(stream);
            writeStreamFile.UpdateEntry(filename, stream);
            return stream;
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
            zipFile.Dispose();
            using (Ionic.Zip.ZipFile ionicZip = new Ionic.Zip.ZipFile(resourceLocation))
            {
                if (ionicZip.ContainsEntry(filename))
                {
                    ionicZip.RemoveEntry(filename);
                    ionicZip.Save();
                }
            }
            zipFile = new ZipFile(resourceLocation);
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
                    yield return info.FullName;
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
            openWriteStreams.Remove(writeStream);
            if (openWriteStreams.Count == 0 && writeStreamFile != null)
            {
                writeStreamFile.Save();
                writeStreamFile.Dispose();
                zipFile = new ZipFile(resourceLocation);
                writeStreamFile = null;
            }
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
    }
}
