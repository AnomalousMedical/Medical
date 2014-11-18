using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class AnatomyFacet
    {
        public AnatomyFacet(String field, IEnumerable<String> values)
        {
            this.Field = field;
            this.Values = values;
        }

        public AnatomyFacet(String field, String value)
        {
            this.Field = field;
            this.Values = new String[]{ value };
        }

        public String Field { get; set; }

        public IEnumerable<String> Values { get; set; }
    }
}
