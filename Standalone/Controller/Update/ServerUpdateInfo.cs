using System;
using System.Collections.Generic;

namespace Medical
{
    public class ServerUpdateInfo
    {
        public ServerUpdateInfo()
        {
            
        }

        public Version RemotePlatformVersion
        {
            get
            {
                return new Version("0.0.0.0");
            }
        }

        public IEnumerable<PluginUpdateInfo> PluginUpdateInfo
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<PluginUpdateInfo> DependencyUpdateInfo
        {
            get
            {
                yield break;
            }
        }
    }
}
