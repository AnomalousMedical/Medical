using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Medical.Utility.LuceneUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class AnatomyLuceneSearch : IDisposable
    {
        private const Lucene.Net.Util.Version LuceneVersion = Lucene.Net.Util.Version.LUCENE_30;

        private Directory directory;
        private IndexSearcher searcher;
        private FacetManager facetManager;
        private AnatomyAnalyzer analyzer = new AnatomyAnalyzer();

        private List<AnatomyIdentifier> anatomyList = new List<AnatomyIdentifier>();

        public AnatomyLuceneSearch()
        {
            
        }

        public void Dispose()
        {
            destroySearcherAndDirectory();
            analyzer.Dispose();
        }

        /// <summary>
        /// Perform a search.
        /// </summary>
        /// <param name="searchTerm">The search term to search product names for. Can be null or empty, which matches everything.</param>
        /// <param name="facets">The facets to apply to the search. Can be null, which matches all facets.</param>
        public IEnumerable<AnatomyIdentifier> search(String searchTerm, IEnumerable<Facet> facets, int maxHits)
        {
            Query query = buildQuery(searchTerm, facets);
            TopDocs results = searcher.Search(query, maxHits);
            foreach(var scoreDoc in results.ScoreDocs)
            {
                var doc = searcher.Doc(scoreDoc.Doc);
                int index = BitConverter.ToInt32(doc.GetBinaryValue("DataIndex"), 0);
                yield return anatomyList[index];
            }
        }

        public void setAnatomy(IEnumerable<AnatomyIdentifier> anatomyIdentifiers)
        {
            destroySearcherAndDirectory();

            directory = new RAMDirectory();

            //Update index
            using (IndexWriter indexWriter = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var anatomy in anatomyIdentifiers)
                {
                    Document document = new Document();
                    int index = anatomyList.Count;
                    anatomyList.Add(anatomy);
                    document.Add(new Field("Id", index.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("DataIndex", BitConverter.GetBytes(index), 0, sizeof(int), Field.Store.YES));
                    document.Add(new Field("Name", anatomy.AnatomicalName, Field.Store.YES, Field.Index.ANALYZED));
                    foreach (String system in anatomy.Systems)
                    {
                        document.Add(new Field("Systems", system, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    if (anatomy.Classification != null)
                    {
                        document.Add(new Field("Classification", anatomy.Classification, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    if (anatomy.Region != null)
                    {
                        document.Add(new Field("Region", anatomy.Region, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }

                    indexWriter.UpdateDocument(new Term("Id", index.ToString()), document);
                }

                indexWriter.Optimize();
            }

            searcher = new IndexSearcher(directory, true);
            facetManager = new FacetManager(searcher.IndexReader, analyzer, getFacets(), LuceneVersion);
        }

        private IEnumerable<String> getFacets()
        {
            yield return "Systems";
            yield return "Connections";
            yield return "Tags";
            yield return "Region";
            yield return "Classification";
        }

        private void destroySearcherAndDirectory()
        {
            if (searcher != null)
            {
                searcher.Dispose();
                searcher = null;
            }

            if (directory != null)
            {
                directory.Dispose();
                directory = null;
            }
        }

        private Query buildQuery(String searchTerm, IEnumerable<Facet> facets)
        {
            Query query;
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                query = new MatchAllDocsQuery();
            }
            else
            {
                if (searchTerm.Length > 2)
                {
                    try
                    {
                        BooleanQuery boolQuery = new BooleanQuery();
                        using (TokenStream source = analyzer.ReusableTokenStream("Name", new System.IO.StringReader(searchTerm)))
                        {
                            var termAtt = source.GetAttribute<ITermAttribute>();
                            bool moreTokens = source.IncrementToken();
                            if (moreTokens)
                            {
                                while (moreTokens)
                                {
                                    var analyzedTerm = termAtt.Term;

                                    boolQuery.Add(new TermQuery(new Term("Name", analyzedTerm)), Occur.SHOULD);
                                    boolQuery.Add(new PrefixQuery(new Term("Name", analyzedTerm)), Occur.SHOULD);

                                    moreTokens = source.IncrementToken();
                                }
                                query = boolQuery;
                            }
                            else
                            {
                                query = buildEmptyQuery();
                            }
                        }
                    }
                    catch(Exception)
                    {
                        query = buildEmptyQuery();
                    }
                }
                else
                {
                    query = new PrefixQuery(new Term("Name", searchTerm.ToLower()));
                }
            }

            if (facets != null && facets.Count() > 0)
            {
                BooleanQuery boolQuery = new BooleanQuery();
                boolQuery.Add(query, Occur.MUST);
                foreach (var facet in facets)
                {
                    //Map the fields back to the case sensitive versions, if the field can't be found it will be skipped.
                    String cleanedField, cleanedValue;
                    if (facetManager.getCaseSensitiveFacet(facet.Field, facet.Value, out cleanedField, out cleanedValue))
                    {
                        boolQuery.Add(new TermQuery(new Term(cleanedField, cleanedValue)), Occur.MUST);
                    }
                    //activeFacets.add(cleanedField, cleanedValue);
                }
                query = boolQuery;
            }
            return query;
        }

        private static Query buildEmptyQuery()
        {
            Query query;
            BooleanQuery noDocsQuery = new BooleanQuery();
            noDocsQuery.Add(new MatchAllDocsQuery(), Occur.MUST_NOT);
            query = noDocsQuery;
            return query;
        }
    }
}
