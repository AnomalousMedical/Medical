using Mono.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
