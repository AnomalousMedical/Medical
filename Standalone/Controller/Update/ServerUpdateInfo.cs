using Mono.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ServerUpdateInfo
    {
        ASN1 asn1;
        AtlasPluginManager pluginManager;

        public ServerUpdateInfo(byte[] bytes)
        {
            asn1 = new ASN1(bytes);
        }

        public Version RemotePlatformVersion
        {
            get
            {
                return Version.Parse(Encoding.ASCII.GetString(asn1.Element(0, 0x13).Value));
            }
        }

        public IEnumerable<PluginUpdateInfo> PluginUpdateInfo
        {
            get
            {
                ASN1 pluginInfos = asn1.Element(1, 0x30);
                for (int i = 0; i < pluginInfos.Count; ++i)
                {
                    ASN1 pluginInfo = pluginInfos[i];
                    yield return new PluginUpdateInfo()
                    {
                        PluginId = BitConverter.ToInt64(pluginInfo[0].Value, 0),
                        Version = Version.Parse(Encoding.ASCII.GetString(pluginInfo.Element(1, 0x13).Value))
                    };
                }
            }
        }
    }
}
