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

        String[] listFiles(String pattern);

        String[] listFiles(String pattern, String directory, bool recursive);

        String[] listDirectories(String pattern, String directory, bool recursive);

        bool directoryHasEntries(String path);

        bool exists(String path);

        String getFullFilePath(String filename);

        void createDirectory(string path, string directoryName);

        String BackingLocation { get; }
    }
}
