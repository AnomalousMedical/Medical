using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    class MultiChildSegment : ProxyChainSegment
    {
        private List<ProxyChainSegment> children = new List<ProxyChainSegment>();

        public void setChildSegment(ProxyChainSegment segment)
        {
            children.Add(segment);
        }

        public void updatePosition()
        {
            foreach(var child in children)
            {
                child.updatePosition();
            }
        }
    }
}
