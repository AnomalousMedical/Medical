using Engine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public abstract class AbstractExecuteAnatomyCommand : AbstractAnatomyCommand
    {
        [DoNotCopy]
        public override float NumericValue
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override float NumericValueMin
        {
            get { throw new NotSupportedException(); }
        }

        public override float NumericValueMax
        {
            get { throw new NotSupportedException(); }
        }

        [DoNotCopy]
        public override bool BooleanValue
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
