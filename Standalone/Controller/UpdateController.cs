using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Medical.Controller;

namespace Medical
{
    public class UpdateController
    {
        static ConfigFile updateInfo;
        static ConfigSection updateSection;

        static UpdateController()
        {
            String updateIndex = Path.Combine(MedicalConfig.SafeDownloadFolder, "update.ini");
            updateInfo = new ConfigFile(updateIndex);
            updateInfo.loadConfigFile();
            updateSection = updateInfo.createOrRetrieveConfigSection("Update");
            try
            {
                DownloadedVersion = new Version(updateSection.getValue("Version", "0.0.0.0"));
            }
            catch (Exception)
            {
                DownloadedVersion = new Version(0, 0, 0, 0);
            }
            InstallFile = updateSection.getValue("File", "");
        }

        private UpdateController()
        {

        }

        public static Version CurrentVersion { get; set; }

        public static Version DownloadedVersion { get; private set; }

        public static String InstallFile { get; private set; }

        public static void checkForUpdate(Action<bool> checkCompletedCallback)
        {
            Thread updateThread = new Thread(delegate()
            {
                ThreadManager.invoke(checkCompletedCallback, false);
            });
            updateThread.Start();
        }

        public static void writeUpdateIndex(String updateInstallerFile, Version version)
        {
            DownloadedVersion = version;
            InstallFile = updateInstallerFile;
            updateSection.setValue("File", updateInstallerFile);
            updateSection.setValue("Version", version.ToString());
            updateInfo.writeConfigFile();
        }

        public static void promptForUpdate()
        {
            if (CurrentVersion < DownloadedVersion && !String.IsNullOrEmpty(InstallFile) && File.Exists(InstallFile))
            {
                if (MessageDialog.showQuestionDialog(String.Format("A new version of the Anomalous Medical Platform {0} has been downloaded. Would you like to install it?", DownloadedVersion), "Update?") == NativeDialogResult.YES)
                {
                    try
                    {
                        Process.Start(InstallFile);
                    }
                    catch (Exception e)
                    {
                        MessageDialog.showErrorDialog(String.Format("Could not install update file {0}. Reason:\n{1}", InstallFile, e.Message), "Update error");
                    }
                }
            }
        }
    }
}
