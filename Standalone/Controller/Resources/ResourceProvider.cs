using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public interface ResourceProvider
    {
        Stream openFile(String filename);

        Stream openWriteStream(String filename);

        void addFile(String path, string targetDirectory);

        void delete(String filename);

        IEnumerable<String> listFiles(String pattern);

        IEnumerable<String> listFiles(String pattern, String directory, bool recursive);

        IEnumerable<String> listDirectories(String pattern, String directory, bool recursive);

        bool directoryHasEntries(String path);

        bool exists(String path);

        String getFullFilePath(String filename);

        void createDirectory(string path, string directoryName);

        bool isDirectory(String path);

        String BackingLocation { get; }

        void move(string oldPath, string newPath);

        void copy(string from, string to);
    }
}
