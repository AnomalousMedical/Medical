using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    static class ZipEntryExtensions
    {
        public static ZipEntry FindEntry(ZipFile file, String name)
        {
            foreach (var entry in file.Entries)
            {
                if (entry.FileName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return entry;
                }
            }
            return null;
        }

        public static IEnumerable<ZipEntry> EntriesStartingWith(ZipFile file, String str)
        {
            foreach (var entry in file.Entries)
            {
                if (entry.FileName.StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return entry;
                }
            }
        }
    }
}
