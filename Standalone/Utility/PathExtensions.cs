using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public static class PathExtensions
    {
        public static String RemoveExtension(String path)
        {
            String extension = Path.GetExtension(path);
            if (!String.IsNullOrEmpty(extension))
            {
                path = path.Replace(extension, "");
            }
            return path;
        }
    }
}
