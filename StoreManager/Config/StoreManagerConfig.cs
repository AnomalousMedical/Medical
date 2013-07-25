using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Config
{
    class StoreManagerConfig
    {
        static String createPluginUrlFormat;
        static String updatePluginUrlFormat;
        static String getPluginsFormat;

        static StoreManagerConfig()
        {
            StoreListUrl = String.Format("{0}/PluginUpload/GetUserStores", MedicalConfig.WebsiteHostUrl);
            createPluginUrlFormat = String.Format("{0}/PluginUpload/CreateNew/{{0}}", MedicalConfig.WebsiteHostUrl);
            updatePluginUrlFormat = String.Format("{0}/PluginUpload/Update/{{0}}", MedicalConfig.WebsiteHostUrl);
            getPluginsFormat = String.Format("{0}/PluginUpload/GetStorePlugins/{{0}}", MedicalConfig.WebsiteHostUrl);
        }

        public static String StoreListUrl { get; private set; }

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
    }
}
