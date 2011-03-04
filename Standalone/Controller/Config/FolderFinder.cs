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
        private static String root = String.Format("{0}/Anomalous Medical", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        private static String download = Path.Combine(root, "Download");
        private static String installLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Anomalous Medical");

        private FolderFinder()
        {

        }

        public static String AnomalousMedicalRoot
        {
            get
            {
                return root;
            }
        }

        public static String DownloadFolder
        {
            get
            {
                return download;
            }
        }

        public static String InstallLocation
        {
            get
            {
                return installLocation;
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
