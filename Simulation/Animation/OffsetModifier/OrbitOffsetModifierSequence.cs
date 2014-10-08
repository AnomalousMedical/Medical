using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class OrbitOffsetModifierSequence : OffsetModifierSequence
    {
        public OrbitOffsetModifierSequence()
        {

        }

        public override OffsetModifierKeyframe createKeyframe()
        {
            return new OrbitOffsetModifierKeyframe();
        }

        protected OrbitOffsetModifierSequence(LoadInfo info)
            :base(info)
        {

        }
    }
}
