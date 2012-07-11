using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Engine;

namespace Medical
{
    public class EmbeddedResourceProvider : ResourceProvider
    {
        private Assembly assembly;
        private String baseResourceString;
        private List<String> fileList = new List<string>();

        public EmbeddedResourceProvider(Assembly assembly, String baseResourceString)
        {
            this.assembly = assembly;
            this.baseResourceString = baseResourceString;

            String[] fileList = assembly.GetManifestResourceNames();
            foreach (String file in fileList)
            {
                if (file.StartsWith(baseResourceString))
                {
                    this.fileList.Add(file.Remove(0, baseResourceString.Length));
                }
            }
        }

        /// <summary>
        /// Sort the files in the resource provider, do this if file name order matters.
        /// </summary>
        /// <param name="fileComparer"></param>
        public void sortFiles(Comparison<String> fileComparer)
        {
            fileList.Sort(fileComparer);
        }

        public Stream openFile(string filename)
        {
            return assembly.GetManifestResourceStream(baseResourceString + convertToNamespacePath(filename));
        }

        public Stream openWriteStream(String filename)
        {
            throw new NotImplementedException("stream writing not supported by this resource provider.");
        }

        public void addFile(String path, string targetDirectory)
        {
            throw new NotImplementedException("addFile not supported by this resource provider.");
        }

        public void delete(String filename)
        {
            throw new NotImplementedException("deleteFile not supported by this resource provider.");
        }

        public string[] listFiles(string pattern)
        {
            Regex r = new Regex(FileUtility.wildcardToRegex(pattern));
            List<String> files = new List<string>(fileList.Count);
            foreach (String file in fileList)
            {
                Match match = r.Match(file);
                if (match.Success)
                {
                    files.Add(convertToDirectoryStyleFileName(file));
                }
            }
            return files.ToArray();
        }

        public String[] listFiles(String pattern, String directory, bool recursive)
        {
            Regex r = new Regex(FileUtility.wildcardToRegex(pattern));
            List<String> files = new List<string>(fileList.Count);
            directory = convertToNamespacePath(directory);
            foreach (String file in fileList)
            {
                if (file.StartsWith(directory))
                {
                    Match match = r.Match(file);
                    if (match.Success)
                    {
                        files.Add(convertToDirectoryStyleFileName(file));
                    }
                }
            }
            return files.ToArray();
        }

        public String[] listDirectories(String pattern, String directory, bool recursive)
        {
            return new String[0];
        }

        public bool directoryHasEntries(String path)
        {
            return listFiles("*", path, true).Length > 0;
        }

        public bool exists(string path)
        {
            return fileList.Contains(convertToNamespacePath(path));
        }

        public String getFullFilePath(String filename)
        {
            return convertToDirectoryStyleFileName(baseResourceString + filename);
        }

        public void createDirectory(string path, string directoryName)
        {
            throw new NotImplementedException("Cannot create directories in the EmbeddedResourceProvider");
        }

        public string BackingLocation
        {
            get
            {
                return baseResourceString;
            }
        }

        private String convertToDirectoryStyleFileName(String filePath)
        {
            int lastDotIndex = filePath.LastIndexOf('.');
            if(lastDotIndex == -1)
            {
                return filePath;
            }
            String pathPart = filePath.Substring(0, lastDotIndex);
            String extension = filePath.Substring(lastDotIndex, filePath.Length - lastDotIndex);
            return pathPart.Replace('.', '/') + extension;
        }

        private String convertToNamespacePath(String directory)
        {
            return directory.Replace('\\', '/').Replace('/', '.');
        }
    }
}
