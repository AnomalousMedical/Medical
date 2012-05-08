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

        void addStream(String filename, MemoryStream memoryStream);

        void addFile(String path);

        void deleteFile(String filename);

        String[] listFiles(String pattern);

        bool exists(String path);

        String getFullFilePath(String filename);

        String BackingLocation { get; }
    }
}
