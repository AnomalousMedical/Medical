using Medical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Controller
{
    class StoreManagerResourceProvider : ResourceProvider
    {
        private EmbeddedResourceProvider dllResourceProvider;
        private ResourceProvider backingResourceProvider;

        public StoreManagerResourceProvider(EmbeddedResourceProvider dllResourceProvider, ResourceProvider backingResourceProvider)
        {
            this.dllResourceProvider = dllResourceProvider;
            this.backingResourceProvider = backingResourceProvider;
        }

        public Stream openFile(string filename)
        {
            if (dllResourceProvider.exists(filename))
            {
                return dllResourceProvider.openFile(filename);
            }
            else
            {
                return backingResourceProvider.openFile(filename);
            }
        }

        public Stream openWriteStream(string filename)
        {
            return backingResourceProvider.openWriteStream(filename);
        }

        public void addFile(string path, string targetDirectory)
        {
            backingResourceProvider.addFile(path, targetDirectory);
        }

        public void delete(string filename)
        {
            backingResourceProvider.delete(filename);
        }

        public IEnumerable<string> listFiles(string pattern)
        {
            foreach (String file in dllResourceProvider.listFiles(pattern))
            {
                yield return file;
            }
            foreach (String file in backingResourceProvider.listFiles(pattern))
            {
                yield return file;
            }
        }

        public IEnumerable<string> listFiles(string pattern, string directory, bool recursive)
        {
            foreach (String file in dllResourceProvider.listFiles(pattern, directory, recursive))
            {
                yield return file;
            }
            foreach (String file in backingResourceProvider.listFiles(pattern, directory, recursive))
            {
                yield return file;
            }
        }

        public IEnumerable<string> listDirectories(string pattern, string directory, bool recursive)
        {
            foreach (String file in dllResourceProvider.listDirectories(pattern, directory, recursive))
            {
                yield return file;
            }
            foreach (String file in backingResourceProvider.listDirectories(pattern, directory, recursive))
            {
                yield return file;
            }
        }

        public bool directoryHasEntries(string path)
        {
            return dllResourceProvider.directoryHasEntries(path) || backingResourceProvider.directoryHasEntries(path);
        }

        public bool exists(string path)
        {
            return dllResourceProvider.exists(path) || backingResourceProvider.exists(path);
        }

        public string getFullFilePath(string filename)
        {
            return backingResourceProvider.getFullFilePath(filename);
        }

        public void createDirectory(string path, string directoryName)
        {
            backingResourceProvider.createDirectory(path, directoryName);
        }

        public string BackingLocation
        {
            get
            {
                return backingResourceProvider.BackingLocation;
            }
        }
    }
}
