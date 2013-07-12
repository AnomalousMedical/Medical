﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Medical.Controller;
using System.Net;
using System.Globalization;
using Logging;
using Mono.Security;

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
            AutoStartUpdate = false;
        }

        private UpdateController()
        {

        }

        public static Version CurrentVersion { get; set; }

        /// <summary>
        /// If this is false the user will be prompted to start the update. If
        /// this is true it will start automatically when promptForUpdate is
        /// called.
        /// </summary>
        public static bool AutoStartUpdate { get; set; }

        public static Version DownloadedVersion { get; private set; }

        public static String InstallFile { get; private set; }

        public static bool HasUpdate
        {
            get
            {
                return CurrentVersion < DownloadedVersion && !String.IsNullOrEmpty(InstallFile) && File.Exists(InstallFile);
            }
        }

        public static void checkForUpdate(Action<bool> checkCompletedCallback, AtlasPluginManager pluginManager, LicenseManager licenseManager)
        {
            //Check for updates on a background thread
            Thread updateThread = new Thread(delegate()
            {
                bool foundUpdate = false;
                try
                {
                    CredentialServerConnection serverConnection = new CredentialServerConnection(MedicalConfig.UpdateCheckURL, licenseManager.User, licenseManager.MachinePassword);
                    serverConnection.addArgument("OsId", ((int)PlatformConfig.OsId).ToString());
                    serverConnection.makeRequestDownloadResponse(responseStream =>
                        {
                            ASN1 asn1 = new ASN1(responseStream.ToArray());
                            Version remotePlatformVersion = Version.Parse(Encoding.ASCII.GetString(asn1.Element(0, 0x13).Value));
                            if (remotePlatformVersion > CurrentVersion)
                            {
                                foundUpdate = true;
                            }
                            else
                            {
                                ASN1 pluginInfos = asn1.Element(1, 0x30);
                                for (int i = 0; i < pluginInfos.Count; ++i)
                                {
                                    ASN1 pluginInfo = pluginInfos[i];
                                    long pluginId = BitConverter.ToInt64(pluginInfo[0].Value, 0);
                                    Version remotePluginVersion = Version.Parse(Encoding.ASCII.GetString(pluginInfo.Element(1, 0x13).Value));
                                    AtlasPlugin plugin = pluginManager.getPlugin(pluginId);
                                    if (plugin != null && remotePluginVersion > plugin.Version)
                                    {
                                        foundUpdate = true;
                                        break;
                                    }
                                }
                            }
                        });
                }
                catch (Exception e)
                {
                    Log.Error("Could not read update status from the server. Reason:\n{0}", e.Message);
                }

                ThreadManager.invoke(checkCompletedCallback, foundUpdate);
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

        /// <summary>
        /// Prompt the user for an update. Return true if the update was started.
        /// </summary>
        /// <returns></returns>
        public static bool promptForUpdate()
        {
            if (HasUpdate)
            {
                if (AutoStartUpdate || MessageDialog.showQuestionDialog(String.Format("A new version of the Anomalous Medical Platform {0} has been downloaded. Would you like to install it?", DownloadedVersion), "Update?") == NativeDialogResult.YES)
                {
                    try
                    {
                        Process.Start(InstallFile);
                        return true;
                    }
                    catch (Exception e)
                    {
                        MessageDialog.showErrorDialog(String.Format("Could not install update file {0}. Reason:\n{1}", InstallFile, e.Message), "Update error");
                    }
                }
            }
            return false;
        }
    }
}
