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

        static StoreManagerConfig()
        {
            StoreListUrl = String.Format("{0}/PluginUpload/GetUserStores", MedicalConfig.WebsiteHostUrl);
            createPluginUrlFormat = String.Format("{0}/PluginUpload/CreateNew/{{0}}", MedicalConfig.WebsiteHostUrl);
            updatePluginUrlFormat = String.Format("{0}/PluginUpload/UpdatePlugin/{{0}}", MedicalConfig.WebsiteHostUrl);
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
    }
}
