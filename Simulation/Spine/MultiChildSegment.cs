using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    class MultiChildSegment : SpineSegment
    {
        private List<SpineSegment> children = new List<SpineSegment>();

        public void setChildSegment(SpineSegment segment)
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
