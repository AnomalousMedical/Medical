using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    class UploadFileInfo
    {
        public String Key { get; set; }

        public String FileName { get; set; }

        public String ContentType { get; set; }

        public Stream Stream { get; set; }
    }
}
