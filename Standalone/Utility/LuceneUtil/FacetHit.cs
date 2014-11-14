using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Utility.LuceneUtil
{
    public class FacetHit
    {
        public String Field { get; set; }

        public String Value { get; set; }

        public String PrettyField { get; set; }

        public String PrettyValue { get; set; }

        public long HitCount { get; set; }

        public bool IsActive { get; set; }
    }
}
