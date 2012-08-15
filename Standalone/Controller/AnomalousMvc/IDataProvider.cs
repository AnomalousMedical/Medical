using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    public interface IDataProvider
    {
        String getValue(String name);

        IEnumerable<Tuple<String, String>> Iterator
        {
            get;
        }
    }
}
