using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    /// <summary>
    /// Provides a common shareable way of finding folders.
    /// </summary>
    class FolderFinder
    {
        private static String userRoot = String.Format("{0}/Anomalous Medical", PlatformConfig.DocumentsFolder);
        private static String allUserRoot = String.Format("{0}/Anomalous Medical", PlatformConfig.AllUserDocumentsFolder);

        private FolderFinder()
        {

        }

        public static String AnomalousMedicalUserRoot
        {
            get
            {
                return userRoot;
            }
        }

        public static String AnomalousMedicalAllUserRoot
        {
            get
            {
                return allUserRoot;
            }
        }

        public static String ExecutableFolder
        {
            get
            {
                String[] args = Environment.GetCommandLineArgs();
                if (args.Length > 0)
                {
                    return Path.GetDirectoryName(args[0]);
                }
                else
                {
                    return Path.GetFullPath(".");
                }
            }
        }
    }
}
