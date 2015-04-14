using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Anomalous.OSPlatform;

namespace Medical
{
    /// <summary>
    /// Provides a common shareable way of finding folders.
    /// </summary>
    public class FolderFinder
    {
        private static String userRoot = String.Format("{0}/Anomalous Medical", RuntimePlatformInfo.LocalUserDocumentsFolder);
        private static String localDataFolder = String.Format("{0}/Anomalous Medical", RuntimePlatformInfo.LocalDataFolder);
        private static String localPrivateDataFolder = String.Format("{0}/Anomalous Medical", RuntimePlatformInfo.LocalPrivateDataFolder);
        private static String programFolder = null;

        private FolderFinder()
        {

        }

        /// <summary>
        /// A folder that user documents and settings can be put into.
        /// </summary>
        public static String LocalUserDocumentsFolder
        {
            get
            {
                return userRoot;
            }
        }

        /// <summary>
        /// A non roaming folder that larger data files (like plugins and program downloads) can be put into.
        /// </summary>
        public static String LocalDataFolder
        {
            get
            {
                return localDataFolder;
            }
        }

        /// <summary>
        /// A non roaming folder that data that should be kept private (like license files) can be put into.
        /// </summary>
        public static String LocalPrivateDataFolder
        {
            get
            {
                return localPrivateDataFolder;
            }
        }

        /// <summary>
        /// The folder the current executable is running under.
        /// </summary>
        public static String ExecutableFolder
        {
            get
            {
                if (programFolder == null)
                {
                    programFolder = RuntimePlatformInfo.ExecutablePath;
                }
                return programFolder;
            }
        }
    }
}
