using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public interface TimelineResourceProvider : IDisposable
    {
        Stream openFile(String filename);

        void addStream(String filename, MemoryStream memoryStream);

        void addFile(String path);

        String[] listFiles(String pattern);

        bool exists(String path);

        String BackingLocation { get; }

        TimelineResourceProvider clone();
    }
}
