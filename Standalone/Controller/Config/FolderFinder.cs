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
    }
}
