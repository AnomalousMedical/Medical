using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    public interface ProxyChainSegment
    {
        void setChildSegment(ProxyChainSegment segment);

        void updatePosition();
    }
}
