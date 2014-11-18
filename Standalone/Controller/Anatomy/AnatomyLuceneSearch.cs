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
        public const Lucene.Net.Util.Version LuceneVersion = Lucene.Net.Util.Version.LUCENE_30;

        private Directory directory;
        private IndexSearcher searcher;
        private FacetManager facetManager;
        private AnatomyAnalyzer analyzer = new AnatomyAnalyzer();

        private List<AnatomyIdentifier> anatomyList = new List<AnatomyIdentifier>();
        private Dictionary<String, AnatomyGroup> systems = new Dictionary<String, AnatomyGroup>();
        private Dictionary<String, AnatomyGroup> regions = new Dictionary<String, AnatomyGroup>();
        private Dictionary<String, AnatomyGroup> classifications = new Dictionary<String, AnatomyGroup>();
        private Dictionary<String, AnatomyGroup> tags = new Dictionary<String, AnatomyGroup>();

        public AnatomyLuceneSearch()
        {
            
        }

        public void Dispose()
        {
            clear();
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

        public void setAnatomyOrganizer(AnatomyOrganizer organizer)
        {
            foreach (AnatomyTagProperties prop in organizer.TagProperties)
            {
                AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                tags.Add(prop.Name, group);
            }

            foreach (AnatomyTagProperties prop in organizer.SystemProperties)
            {
                AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                systems.Add(prop.Name, group);
            }

            foreach (AnatomyTagProperties prop in organizer.RegionProperties)
            {
                AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                regions.Add(prop.Name, group);
            }

            foreach (AnatomyTagProperties prop in organizer.ClassificationProperties)
            {
                AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                classifications.Add(prop.Name, group);
            }
        }

        public void setAnatomy(IEnumerable<AnatomyIdentifier> anatomyIdentifiers)
        {
            directory = new RAMDirectory();

            //Update index
            using (IndexWriter indexWriter = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var anatomy in anatomyIdentifiers)
                {
                    if (anatomy.ShowInTextSearch)
                    {
                        Document document = new Document();
                        int index = anatomyList.Count;
                        anatomyList.Add(anatomy);
                        document.Add(new Field("Id", index.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("DataIndex", BitConverter.GetBytes(index), 0, sizeof(int), Field.Store.YES));
                        document.Add(new Field("Name", anatomy.AnatomicalName, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("AnatomyType", "Individual", Field.Store.YES, Field.Index.NOT_ANALYZED));
                        foreach (String system in anatomy.Systems)
                        {
                            document.Add(new Field("Systems", system, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup systemGroup = getSystemGroup(system);
                            systemGroup.addAnatomy(anatomy);
                        }
                        foreach(String tag in anatomy.Tags)
                        {
                            document.Add(new Field("Tags", tag, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup tagGroup = getTagGroup(tag);
                            tagGroup.addAnatomy(anatomy);
                        }
                        if (anatomy.Classification != null)
                        {
                            document.Add(new Field("Classification", anatomy.Classification, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup classificationGroup = getClassificationGroup(anatomy.Classification);
                            classificationGroup.addAnatomy(anatomy);
                        }
                        if (anatomy.Region != null)
                        {
                            document.Add(new Field("Region", anatomy.Region, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup regionGorup = getRegionGroup(anatomy.Region);
                            regionGorup.addAnatomy(anatomy);
                        }

                        indexWriter.UpdateDocument(new Term("Id", index.ToString()), document);
                    }
                }

                indexWriter.Optimize();
            }

            searcher = new IndexSearcher(directory, true);
            facetManager = new FacetManager(searcher.IndexReader, analyzer, getFacets(), LuceneVersion);
        }

        public void clear()
        {
            systems.Clear();
            regions.Clear();
            classifications.Clear();
            tags.Clear();

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

        public IEnumerable<AnatomyGroup> Systems
        {
            get
            {
                return systems.Values;
            }
        }

        public bool tryGetSystem(String name, out AnatomyGroup group)
        {
            return systems.TryGetValue(name, out group);
        }

        public IEnumerable<AnatomyGroup> Tags
        {
            get
            {
                return tags.Values;
            }
        }

        public bool tryGetTag(String name, out AnatomyGroup group)
        {
            return tags.TryGetValue(name, out group);
        }

        public IEnumerable<AnatomyGroup> Regions
        {
            get
            {
                return regions.Values;
            }
        }

        public bool tryGetRegion(String name, out AnatomyGroup group)
        {
            return regions.TryGetValue(name, out group);
        }

        public IEnumerable<AnatomyGroup> Classifications
        {
            get
            {
                return classifications.Values;
            }
        }

        public bool tryGetClassification(String name, out AnatomyGroup group)
        {
            return classifications.TryGetValue(name, out group);
        }

        private AnatomyGroup getSystemGroup(String system)
        {
            AnatomyGroup group;
            if(!systems.TryGetValue(system, out group))
            {
                group = new AnatomyGroup(system);
                systems.Add(system, group);
            }
            return group;
        }

        private AnatomyGroup getTagGroup(String tag)
        {
            AnatomyGroup group;
            if (!tags.TryGetValue(tag, out group))
            {
                group = new AnatomyGroup(tag);
                tags.Add(tag, group);
            }
            return group;
        }

        private AnatomyGroup getRegionGroup(String region)
        {
            AnatomyGroup group;
            if (!regions.TryGetValue(region, out group))
            {
                group = new AnatomyGroup(region);
                regions.Add(region, group);
            }
            return group;
        }

        private AnatomyGroup getClassificationGroup(String classification)
        {
            AnatomyGroup group;
            if (!classifications.TryGetValue(classification, out group))
            {
                group = new AnatomyGroup(classification);
                classifications.Add(classification, group);
            }
            return group;
        }

        private IEnumerable<String> getFacets()
        {
            yield return "Systems";
            yield return "Connections";
            yield return "Tags";
            yield return "Region";
            yield return "Classification";
            yield return "AnatomyType";
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
                if (searchTerm.Length > 1)
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

            //Logging.Log.Debug(query.ToString());
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
