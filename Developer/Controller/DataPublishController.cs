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
    /// <summary>
    /// This class can publish data files, either dependency (.ddd) files or plugin (.ddp) files.
    /// </summary>
    class DataPublishController
    {
        private List<FileInfo> files = new List<FileInfo>();

        public DataPublishController()
        {
            Compress = true;
            Obfuscate = true;
        }

        public String SignatureFile { get; set; }

        public String SignatureFilePassword { get; set; }

        public String CounterSignatureFile { get; set; }

        public String CounterSignaturePassword { get; set; }

        public bool Compress { get; set; }

        public bool Obfuscate { get; set; }

        public void publishDataFile(String dataDefinitionPath, String outputPath)
        {
            try
            {
                switch(Path.GetExtension(dataDefinitionPath).ToLowerInvariant())
                { 
                    case ".ddp":
                        DDAtlasPlugin plugin = null;
                        using (Stream stream = File.Open(dataDefinitionPath, FileMode.Open))
                        {
                            plugin = SharedXmlSaver.Load<DDAtlasPlugin>(stream);
                        }

                        if (plugin != null)
                        {
                            findFiles(dataDefinitionPath, "Plugins", outputPath, plugin.PluginNamespace);
                        }
                        break;
                    case ".ddd":
                        DDAtlasDependency dependency = null;
                        using (Stream stream = File.Open(dataDefinitionPath, FileMode.Open))
                        {
                            dependency = SharedXmlSaver.Load<DDAtlasDependency>(stream);
                        }

                        if (dependency != null)
                        {
                            findFiles(dataDefinitionPath, "Dependencies", outputPath, dependency.DependencyNamespace);
                        }
                        break;
                }
            }
            finally
            {
                files.Clear();
            }
        }

        private void findFiles(String dataFileDefinitionPath, String destinationBasePath, String outputPath, String dataNamespace)
        {
            String dataBasePath = Path.GetDirectoryName(dataFileDefinitionPath);

            //Scan files
            String[] pluginFiles = Directory.GetFiles(dataBasePath, "*", SearchOption.AllDirectories);
            foreach (String file in pluginFiles)
            {
                addFile(new FileInfo(file));
            }

            //Copy Files, build data file
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            copyResources(dataNamespace, dataBasePath, destinationBasePath, outputPath);
        }

        private void copyResources(String dataNamespace, String originalBasePath, String destinationBasePath, String outputPath)
        {
            String archiveName = dataNamespace;
            originalBasePath = Path.GetFullPath(originalBasePath) + Path.DirectorySeparatorChar;
            String targetDirectory = outputPath;
            String zipTempDirectory = null;
            if (Compress)
            {
                targetDirectory += "/zipTemp";
                zipTempDirectory = targetDirectory;
                if(Directory.Exists(targetDirectory))
                {
                    Directory.Delete(targetDirectory, true);
                }
            }

            targetDirectory = Path.Combine(targetDirectory, destinationBasePath, dataNamespace);

            Log.Info("Starting file copy to {0}", targetDirectory);
            foreach (FileInfo sourceFile in files)
            {
                String destination = Path.Combine(targetDirectory, sourceFile.FullName.Replace(originalBasePath, ""));
                String directory = Path.GetDirectoryName(destination);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                Log.Info("Copying {0} to {1}.", sourceFile.FullName, destination);
                File.Copy(sourceFile.FullName, destination, true);
            }

            //Compress
            if (Compress)
            {
                String zipFileName = String.Format("{0}\\{1}.zip", outputPath, archiveName);
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

                if (Obfuscate)
                {
                    String obfuscateFileName = String.Format("{0}\\{1}.dat", outputPath, archiveName);
                    obfuscateZipFile(zipFileName, obfuscateFileName);
                    File.Delete(zipFileName);

                    //Sign the file
                    signDataFile(obfuscateFileName);
                }
            }
        }

        private void signDataFile(String dataFile)
        {
            String signedFile = Path.Combine(Path.GetDirectoryName(dataFile), "Signed_" + Path.GetFileName(dataFile));
            SignedDataFile.SignDataFile(dataFile, signedFile, CryptoHelper.LoadPkcs12(SignatureFile, SignatureFilePassword), CryptoHelper.LoadPkcs12(CounterSignatureFile, CounterSignaturePassword));
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
