using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Utility.LuceneUtil
{
    public class Facet
    {
        public Facet(String field, String value)
        {
            this.Field = field;
            this.Value = value;
        }

        public String Field { get; set; }

        public String Value { get; set; }
    }
}
