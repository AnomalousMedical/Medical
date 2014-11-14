using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Utility.LuceneUtil
{
    public class FacetManager
    {
        class FacetEntry
        {
            public String Field { get; set; }

            public String Value { get; set; }

            public OpenBitSetDISI BitSet { get; set; }
        }

        private List<FacetEntry> groups = new List<FacetEntry>();
        private IndexReader reader;
        private FacetPrettyNameCollection prettyNames = new FacetPrettyNameCollection();
        private InsensitiveSearchMap insensitiveSearchMap = new InsensitiveSearchMap();

        public FacetManager(IndexReader reader, Analyzer analyzer, IEnumerable<String> groupFields, Lucene.Net.Util.Version version)
        {
            this.reader = reader;
            foreach (var field in groupFields)
            {
                TermEnum te = reader.Terms(new Term(field, ""));
                foreach (var term in te.Iterator())
                {
                    if (term.Field != field)
                    {
                        break;
                    }
                    var query = new TermQuery(term);
                    var queryFilter = new QueryWrapperFilter(query);
                    var setIterator = queryFilter.GetDocIdSet(reader).Iterator();
                    var bitSet = new OpenBitSetDISI(setIterator, reader.MaxDoc);
                    groups.Add(new FacetEntry()
                    {
                        Field = term.Field,
                        Value = term.Text,
                        BitSet = bitSet
                    });
                    insensitiveSearchMap.addFacet(term.Field, term.Text);
                }
            }
        }

        public IEnumerable<FacetHit> GetHitCounts(Query query, ActiveFacetCollection activeFacets)
        {
            bool allowEnumeration = true;
            DocIdSet docIdSet = null;
            OpenBitSetDISI searchBitSet = null;
            try
            {
                var searchQueryFilter = new QueryWrapperFilter(query);
                docIdSet = searchQueryFilter.GetDocIdSet(reader);
                searchBitSet = new OpenBitSetDISI(docIdSet.Iterator(), reader.MaxDoc);
            }
            catch (Exception)
            {
                allowEnumeration = false;
            }

            if (allowEnumeration)
            {
                foreach (var entry in groups)
                {
                    long hitCount;
                    if (docIdSet == DocIdBitSet.EMPTY_DOCIDSET)
                    {
                        hitCount = 0;
                    }
                    else
                    {
                        var cloneBitSet = (OpenBitSet)entry.BitSet.Clone();
                        cloneBitSet.And(searchBitSet);
                        hitCount = cloneBitSet.Cardinality();
                    }
                    yield return new FacetHit()
                    {
                        PrettyField = prettyNames.getFieldPrettyName(entry.Field),
                        PrettyValue = prettyNames.getValuePrettyName(entry.Field, entry.Value),
                        Field = entry.Field,
                        Value = entry.Value,
                        HitCount = hitCount,
                        IsActive = activeFacets.isActive(entry.Field, entry.Value),
                    };
                }
            }
        }

        public bool getCaseSensitiveFacet(String field, String value, out String csField, out String csValue)
        {
            return insensitiveSearchMap.getCaseSensitiveFacet(field, value, out csField, out csValue);
        }

        public FacetPrettyNameCollection PrettyNames
        {
            get
            {
                return prettyNames;
            }
        }
    }
}
