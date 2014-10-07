using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class SimpleOffsetModifierSequence : OffsetModifierSequence
    {
        public SimpleOffsetModifierSequence()
        {

        }

        public override OffsetModifierKeyframe createKeyframe()
        {
            return new SimpleOffsetModifierKeyframe();
        }

        protected SimpleOffsetModifierSequence(LoadInfo info)
            :base(info)
        {

        }
    }
}
