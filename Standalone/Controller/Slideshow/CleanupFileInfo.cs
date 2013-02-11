using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical
{
    public class CleanupFileInfo
    {
        private List<String> files = new List<string>();

        public void claimFile(String name)
        {
            files.Add(Path.GetFullPath(name));
        }

        public bool isClaimed(String file)
        {
            return files.Contains(Path.GetFullPath(file));
        }
    }
}
