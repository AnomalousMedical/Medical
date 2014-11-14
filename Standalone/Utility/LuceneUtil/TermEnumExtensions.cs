using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Utility.LuceneUtil
{
    public static class TermEnumExtensions
    {
        public static IEnumerable<Term> Iterator(this TermEnum te)
        {
            if (te.Term != null)
            {
                yield return te.Term;
                while (te.Next())
                {
                    yield return te.Term;
                }
            }
            else
            {
                yield break;
            }
        }
    }
}
