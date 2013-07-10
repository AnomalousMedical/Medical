using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using System.IO;
using Logging;
using Ionic.Zip;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Anomalous.Security;

namespace Developer
{
    class PluginPublishController
    {
        private AtlasPluginManager pluginManager;
        private List<FileInfo> files = new List<FileInfo>();

        public PluginPublishController(AtlasPluginManager pluginManager)
        {
            this.pluginManager = pluginManager;
        }

        public void publishPlugin(String pluginFile, String certFile, String certFilePassword, String counterSignatureFile, String counterSignaturePassword, String outDirectory)
        {
            try
            {
                DDAtlasPlugin plugin = null;
                using (Stream stream = File.Open(pluginFile, FileMode.Open))
                {
                    plugin = SharedXmlSaver.Load<DDAtlasPlugin>(stream);
                }

                if (plugin != null)
                {
                    String pluginPath = Path.GetDirectoryName(pluginFile);

                    //Scan files
                    String[] pluginFiles = Directory.GetFiles(pluginPath, "*", SearchOption.AllDirectories);
                    foreach (String file in pluginFiles)
                    {
                        addFile(new FileInfo(file));
                    }

                    //Copy Files, build plugin
                    if (!Directory.Exists(outDirectory))
                    {
                        Directory.CreateDirectory(outDirectory);
                    }
                    copyResources(plugin, Path.GetDirectoryName(pluginFile), outDirectory, pluginFile, certFile, certFilePassword, counterSignatureFile, counterSignaturePassword, true, true);
                }
            }
            finally
            {
                files.Clear();
            }
        }

        private void copyResources(DDAtlasPlugin plugin, String basePath, String targetDirectory, String pluginFile, String certFile, String certFilePassword, String counterSignatureFile, String counterSignaturePassword, bool compress, bool obfuscate)
        {
            String archiveName = plugin.PluginNamespace;
            basePath = Path.GetFullPath(basePath) + Path.DirectorySeparatorChar;
            String originalDirectory = targetDirectory;
            String zipTempDirectory = null;
            if (compress)
            {
                targetDirectory += "/zipTemp";
                zipTempDirectory = targetDirectory;
            }

            targetDirectory = Path.Combine(targetDirectory, "Plugins", plugin.PluginNamespace);

            Log.Info("Starting file copy to {0}", targetDirectory);
            foreach (FileInfo sourceFile in files)
            {
                String destination = Path.Combine(targetDirectory, sourceFile.FullName.Replace(basePath, ""));
                String directory = Path.GetDirectoryName(destination);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                Log.Info("Copying {0} to {1}.", sourceFile.FullName, destination);
                File.Copy(sourceFile.FullName, destination, true);
            }

            //Compress
            if (compress)
            {
                String zipFileName = originalDirectory + "\\" + archiveName + ".zip";
                Log.Info("Starting compression to {0}", zipFileName);
                if (File.Exists(zipFileName))
                {
                    File.Delete(zipFileName);
                }

                using (ZipFile zipFile = new ZipFile(zipFileName, new ZipStatusTextWriter()))
                {
                    zipFile.AddDirectory(zipTempDirectory, "");
                    zipFile.Save();
                }

                Log.Info("Finished compression to {0}", zipFileName);
                Directory.Delete(zipTempDirectory, true);

                if (obfuscate)
                {
                    String obfuscateFileName = originalDirectory + "\\" + archiveName + ".dat";
                    obfuscateZipFile(zipFileName, obfuscateFileName);
                    File.Delete(zipFileName);

                    //Sign the file
                    signPluginFile(obfuscateFileName, certFile, certFilePassword, counterSignatureFile, counterSignaturePassword);
                }
            }
        }

        private static void signPluginFile(String dataFile, String certFile, String certFilePassword, String counterSignatureFile, String counterSignaturePassword)
        {
            String signedFile = Path.Combine(Path.GetDirectoryName(dataFile), "Signed_" + Path.GetFileName(dataFile));
            SignedDataFile.SignDataFile(dataFile, signedFile, CryptoHelper.LoadPkcs12(certFile, certFilePassword), CryptoHelper.LoadPkcs12(counterSignatureFile, counterSignaturePassword));
            File.Delete(dataFile);
            File.Move(signedFile, dataFile);
        }

        public static void obfuscateZipFile(String zipFileName, String obfuscateFileName)
        {
            Log.Info("Starting obfuscation to {0}", obfuscateFileName);
            if (File.Exists(obfuscateFileName))
            {
                File.Delete(obfuscateFileName);
            }

            byte[] buffer = new byte[4096];
            using (Stream source = File.Open(zipFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                using (Stream dest = File.Open(obfuscateFileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
                {
                    int numRead;
                    while ((numRead = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < numRead; ++i)
                        {
                            buffer[i] ^= 73;
                        }
                        dest.Write(buffer, 0, numRead);
                    }
                }
            }
            Log.Info("Finished obfuscation to {0}", obfuscateFileName);
        }

        private void addFile(FileInfo fileInfo)
        {
            if (!fileInfo.Name.EndsWith("~") && !fileInfo.DirectoryName.Contains(".svn") && ((fileInfo.Attributes & FileAttributes.Hidden) == 0))
            {
                files.Add(fileInfo);
            }
        }
    }
}
