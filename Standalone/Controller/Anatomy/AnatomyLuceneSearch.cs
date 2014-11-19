using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
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
        private AnatomyAnalyzer analyzer = new AnatomyAnalyzer();

        private AnatomyController anatomyController;

        private List<Anatomy> anatomyList = new List<Anatomy>();
        private Dictionary<String, AnatomyGroup> systems = new Dictionary<String, AnatomyGroup>();
        private Dictionary<String, AnatomyGroup> regions = new Dictionary<String, AnatomyGroup>();
        private Dictionary<String, AnatomyGroup> classifications = new Dictionary<String, AnatomyGroup>();
        private Dictionary<String, AnatomyGroup> tags = new Dictionary<String, AnatomyGroup>();

        public AnatomyLuceneSearch(AnatomyController anatomyController)
        {
            this.anatomyController = anatomyController;
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
        public IEnumerable<Anatomy> search(String searchTerm, IEnumerable<AnatomyFacet> facets, int maxHits)
        {
            Query query = buildQuery(searchTerm, facets);
            TopDocs results = searcher.Search(query, maxHits);
            foreach (var scoreDoc in results.ScoreDocs)
            {
                var doc = searcher.Doc(scoreDoc.Doc);
                int index = BitConverter.ToInt32(doc.GetBinaryValue("DataIndex"), 0);
                yield return anatomyList[index];
            }
        }

        public void setAnatomy(IEnumerable<AnatomyIdentifier> anatomyIdentifiers, AnatomyOrganizer organizer)
        {
            if (organizer != null)
            {
                foreach (AnatomyTagProperties prop in organizer.TagProperties)
                {
                    AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                    setupTagGroup(group);
                }

                foreach (AnatomyTagProperties prop in organizer.SystemProperties)
                {
                    AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                    setupSystemGroup(group);
                }

                foreach (AnatomyTagProperties prop in organizer.RegionProperties)
                {
                    AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                    setupRegionGroup(group);
                }

                foreach (AnatomyTagProperties prop in organizer.ClassificationProperties)
                {
                    AnatomyGroup group = new AnatomyGroup(prop.Name, prop.ShowInBasicVersion, prop.ShowInTextSearch, prop.ShowInClickSearch, prop.ShowInTree);
                    setupClassificationGroup(group);
                }
            }

            directory = new RAMDirectory();

            //Update index
            using (IndexWriter indexWriter = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var anatomy in anatomyIdentifiers)
                {
                    anatomy.addExternalCommand(new CallbackAnatomyCommand("Show Systems", () =>
                        anatomyController.displayAnatomy(String.Format("{0} Systems", anatomy.AnatomicalName), anatomy.Systems.Select(i => systems[i]), SuggestedDisplaySortMode.Alphabetical))
                        {
                            DisplayInGroup = false
                        });

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
                            document.Add(new Field("System", system, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup systemGroup = getSystemGroup(system);
                            systemGroup.addAnatomy(anatomy);
                        }
                        foreach (String tag in anatomy.Tags)
                        {
                            document.Add(new Field("Tag", tag, Field.Store.YES, Field.Index.NOT_ANALYZED));
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

                //Add the groups to the index last so we can be sure they actually have anatomy in them.
                foreach(var group in tags.Values)
                {
                    setupGroupDocument(indexWriter, group, "Tag");
                }

                foreach (var group in systems.Values)
                {
                    setupGroupDocument(indexWriter, group, "System");
                }

                foreach (var group in regions.Values)
                {
                    setupGroupDocument(indexWriter, group, "Region");
                } 
                
                foreach (var group in classifications.Values)
                {
                    setupGroupDocument(indexWriter, group, "Classification");
                }

                indexWriter.Optimize();
            }

            searcher = new IndexSearcher(directory, true);
        }

        public void clear()
        {
            anatomyList.Clear();
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
            if (!systems.TryGetValue(system, out group))
            {
                group = new AnatomyGroup(system);
                setupSystemGroup(group);
            }
            return group;
        }

        private void setupSystemGroup(AnatomyGroup group)
        {
            group.addCommand(new CallbackAnatomyCommand("Show System Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, "System")));
            group.addCommand(new CallbackAnatomyCommand("Breakdown by Region", () => breakdownGroup("{0} of the {1}", "System", group, "Region", regions.Values)));
            systems.Add(group.AnatomicalName, group);
        }

        private AnatomyGroup getTagGroup(String tag)
        {
            AnatomyGroup group;
            if (!tags.TryGetValue(tag, out group))
            {
                group = new AnatomyGroup(tag);
                setupTagGroup(group);
            }
            return group;
        }

        private void setupTagGroup(AnatomyGroup group)
        {
            group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, "Tag")));
            tags.Add(group.AnatomicalName, group);
        }

        private AnatomyGroup getRegionGroup(String region)
        {
            AnatomyGroup group;
            if (!regions.TryGetValue(region, out group))
            {
                group = new AnatomyGroup(region);
                setupRegionGroup(group);
            }
            return group;
        }

        private void setupRegionGroup(AnatomyGroup group)
        {
            group.addCommand(new CallbackAnatomyCommand("Show Region Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, "Region")));
            group.addCommand(new CallbackAnatomyCommand("Breakdown by System", () => breakdownGroup("{1} of the {0}", "Region", group, "System", systems.Values)));
            regions.Add(group.AnatomicalName, group);
        }

        private AnatomyGroup getClassificationGroup(String classification)
        {
            AnatomyGroup group;
            if (!classifications.TryGetValue(classification, out group))
            {
                group = new AnatomyGroup(classification);
                setupClassificationGroup(group);
            }
            return group;
        }

        private void setupClassificationGroup(AnatomyGroup group)
        {
            group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, "Classification")));
            group.addCommand(new CallbackAnatomyCommand("Breakdown by Region", () => breakdownGroup("{0} of the {1}", "Classification", group, "Region", regions.Values)));
            classifications.Add(group.AnatomicalName, group);
        }

        private Query buildQuery(String searchTerm, IEnumerable<AnatomyFacet> facets)
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
                    catch (Exception)
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
                    bool add = false;
                    BooleanQuery facetQuery = new BooleanQuery();
                    foreach (var value in facet.Values)
                    {
                        facetQuery.Add(new TermQuery(new Term(facet.Field, value)), Occur.SHOULD);
                        add = true;
                    }
                    if (add)
                    {
                        boolQuery.Add(facetQuery, Occur.MUST);
                    }
                }
                query = boolQuery;
            }

            return query;
        }

        private void displayAnatomyForFacet(String groupName, String facet)
        {
            anatomyController.displayAnatomy(String.Format("{0} Anatomy", groupName),
                search("", new AnatomyFacet[] { new AnatomyFacet(facet, groupName) }, int.MaxValue), SuggestedDisplaySortMode.Alphabetical);
        }

        private void breakdownGroup(String groupTitleFormat, String mainFacet, AnatomyGroup group, String breakdownFacet, IEnumerable<AnatomyGroup> breakdownGroups)
        {
            anatomyController.displayAnatomy(String.Format("{0} by {1}", group.AnatomicalName, breakdownFacet),
                breakdownGroupSearch(groupTitleFormat, mainFacet, group.AnatomicalName, breakdownFacet, breakdownGroups.Select(i => i.AnatomicalName)), 
                SuggestedDisplaySortMode.Alphabetical);
        }

        private IEnumerable<AnatomyGroup> breakdownGroupSearch(String groupTitleFormat, String mainFacet, String mainFacetValue, String breakdownFacet, IEnumerable<String> breakdownValues)
        {
            List<AnatomyFacet> facets = new List<AnatomyFacet>();
            facets.Add(new AnatomyFacet(mainFacet, mainFacetValue));
            String[] breakdownValueArray = new String[1];
            facets.Add(new AnatomyFacet(breakdownFacet, breakdownValueArray));

            foreach (var breakdownValue in breakdownValues)
            {
                breakdownValueArray[0] = breakdownValue;
                AnatomyGroup resultGroup = new AnatomyGroup(String.Format(groupTitleFormat, mainFacetValue, breakdownValue));
                resultGroup.addAnatomy(search(null, facets, int.MaxValue));
                if (resultGroup.Count > 0)
                {
                    yield return resultGroup;
                }
            }
        }

        private static Query buildEmptyQuery()
        {
            Query query;
            BooleanQuery noDocsQuery = new BooleanQuery();
            noDocsQuery.Add(new MatchAllDocsQuery(), Occur.MUST_NOT);
            query = noDocsQuery;
            return query;
        }

        private void setupGroupDocument(IndexWriter indexWriter, AnatomyGroup group, String anatomyType)
        {
            if (group.ShowInTextSearch)
            {
                Document document = new Document();
                int index = anatomyList.Count;
                anatomyList.Add(group);
                document.Add(new Field("Id", index.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                document.Add(new Field("DataIndex", BitConverter.GetBytes(index), 0, sizeof(int), Field.Store.YES));
                document.Add(new Field("Name", group.AnatomicalName, Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("AnatomyType", anatomyType, Field.Store.YES, Field.Index.NOT_ANALYZED));
                indexWriter.UpdateDocument(new Term("Id", index.ToString()), document);
            }
        }
    }
}
