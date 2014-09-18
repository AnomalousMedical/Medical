using Engine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public interface BlendDriver
    {
        event Action<BlendDriver> BlendAmountChanged;

        float BlendAmount { get; }
    }
}
