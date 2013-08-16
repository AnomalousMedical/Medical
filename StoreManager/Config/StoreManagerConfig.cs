using Medical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Config
{
    class StoreManagerConfig
    {
        static String createPluginUrlFormat;
        static String updatePluginUrlFormat;
        static String getPluginsFormat;
        static String storeDashboardPluginsFormat;
        static String beginUploadFormat;
        static String uploadCompletedFormat;
        static String uploadCanceledFormat;

        static StoreManagerConfig()
        {
            StoreListUrl = String.Format("{0}/PluginUpload/GetUserStores", MedicalConfig.WebsiteHostUrl);
            createPluginUrlFormat = String.Format("{0}/PluginUpload/CreateNew/{{0}}", MedicalConfig.WebsiteHostUrl);
            updatePluginUrlFormat = String.Format("{0}/PluginUpload/Update/{{0}}", MedicalConfig.WebsiteHostUrl);
            getPluginsFormat = String.Format("{0}/PluginUpload/GetStorePlugins/{{0}}", MedicalConfig.WebsiteHostUrl);
            storeDashboardPluginsFormat = String.Format("{0}/StoreDashboard/Plugins/{{0}}", MedicalConfig.WebsiteHostUrl);
            beginUploadFormat = String.Format("{0}/PluginUpload/BeginUpload/{{0}}", MedicalConfig.WebsiteHostUrl);
            uploadCompletedFormat = String.Format("{0}/PluginUpload/UploadCompleted/{{0}}", MedicalConfig.WebsiteHostUrl);
            uploadCanceledFormat = String.Format("{0}/PluginUpload/UploadCanceled/{{0}}", MedicalConfig.WebsiteHostUrl);
        }

        public static String StoreListUrl { get; private set; }

        public static String UploadArchiveDirectory
        {
            get
            {
                return Path.Combine(MedicalConfig.UserDocRoot, "UploadArchives");
            }
        }

        public static String CreatePluginUrl(String storeUniqueName)
        {
            return String.Format(createPluginUrlFormat, storeUniqueName);
        }

        public static String UpdatePluginUrl(String storeUniqueName)
        {
            return String.Format(updatePluginUrlFormat, storeUniqueName);
        }

        public static String GetStorePluginsUrl(String storeUniqueName)
        {
            return string.Format(getPluginsFormat, storeUniqueName);
        }

        public static String GetBeginUploadUrl(String storeUniqueName)
        {
            return string.Format(beginUploadFormat, storeUniqueName);
        }

        public static String GetUploadCompleteUrl(String storeUniqueName)
        {
            return string.Format(uploadCompletedFormat, storeUniqueName);
        }

        public static String GetStoreDashboardPluginsUrl(String storeUniqueName)
        {
            return string.Format(storeDashboardPluginsFormat, storeUniqueName);
        }

        internal static string GetCancelUploadUrl(string storeUniqueName)
        {
            return string.Format(uploadCanceledFormat, storeUniqueName);
        }
    }
}
