using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HiddenAttribute : Attribute
    {
    }
}
