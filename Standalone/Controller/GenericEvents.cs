using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void SingleArgumentEvent<SourceType, ArgType>(SourceType source, ArgType arg);
}
