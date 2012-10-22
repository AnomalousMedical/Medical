using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Medical
{
    public static class PathExtensions
    {
        private static String InvalidReStr = String.Format(@"[{0}]+", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
        private static String ReplaceChar = "_";

        public static String RemoveExtension(String path)
        {
            String extension = Path.GetExtension(path);
            if (!String.IsNullOrEmpty(extension))
            {
                path = path.Replace(extension, "");
            }
            return path;
        }

        public static String MakeValidFileName(string name)
        {
            return Regex.Replace(name, InvalidReStr, ReplaceChar);
        }
    }
}
