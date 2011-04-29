using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;

namespace Medical
{
    public abstract class AbstractBooleanAnatomyCommand : AbstractAnatomyCommand
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

        public override void execute()
        {
            throw new NotSupportedException();
        }
    }
}
