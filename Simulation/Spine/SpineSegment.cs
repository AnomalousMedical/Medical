using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    public interface SpineSegment
    {
        void setChildSegment(SpineSegment segment);

        void updatePosition();
    }
}
