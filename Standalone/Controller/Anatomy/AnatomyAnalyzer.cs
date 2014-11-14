using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A lucene analyzer for anatomy, not using a stop list because
    /// the search phrases are not paragraphs.
    /// </summary>
    class AnatomyAnalyzer : Analyzer
    {
        public AnatomyAnalyzer()
        {
            
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new LowerCaseFilter(new StandardTokenizer(AnatomyLuceneSearch.LuceneVersion, reader));
        }
    }
}
