using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    public interface ProxyChainSegment
    {
        void setChildSegment(ProxyChainSegment segment);

        void updatePosition();
    }
}
