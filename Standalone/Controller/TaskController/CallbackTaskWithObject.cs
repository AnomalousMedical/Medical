using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// A TaskMenuItem that will fire a delegate.
    /// </summary>
    public class CallbackTaskWithObject<T> : CallbackTask
    {
        public CallbackTaskWithObject(String uniqueName, String name, String iconName, String category)
            : base(uniqueName, name, iconName, category, DEFAULT_WEIGHT, true)
        {

        }

        public T UserObject { get; set; }
    }
}
