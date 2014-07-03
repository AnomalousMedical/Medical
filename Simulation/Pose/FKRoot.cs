using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A root of an FK Chain. This class can have no parents and can apply chain states.
    /// </summary>
    public interface FKRoot : FKElement
    {
        String RootName { get; }
    }
}
