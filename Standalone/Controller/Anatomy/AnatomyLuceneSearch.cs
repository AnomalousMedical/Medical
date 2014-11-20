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
        private FacetGroupManager systems;
        private FacetGroupManager regions;
        private FacetGroupManager classifications;
        private FacetGroupManager tags;
        private FacetGroupManager structures;

        public AnatomyLuceneSearch(AnatomyController anatomyController)
        {
            this.anatomyController = anatomyController;

            systems = new FacetGroupManager("System", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show System Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, systems.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Region", () => breakdownGroup("{0} of the {1}", systems.FacetName, group, regions)));
            });

            regions = new FacetGroupManager("Region", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Region Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, regions.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by System", () => breakdownGroup("{1} of the {0}", regions.FacetName, group, systems)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Classification", () => breakdownGroup("{1} of the {0}", regions.FacetName, group, classifications)));
            });

            classifications = new FacetGroupManager("Classification", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, classifications.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Region", () => breakdownGroup("{0} of the {1}", classifications.FacetName, group, regions)));
            });

            tags = new FacetGroupManager("Tag", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, tags.FacetName)));
            });

            structures = new FacetGroupManager("Structure", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, structures.FacetName)));
            });
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
                tags.createGroups(organizer.TagProperties);
                systems.createGroups(organizer.SystemProperties);
                regions.createGroups(organizer.RegionProperties);
                classifications.createGroups(organizer.ClassificationProperties);
                structures.createGroups(organizer.StructureProperties);
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
                        int index = addAnatomyToIndex(anatomy);
                        document.Add(new Field("Id", index.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("DataIndex", BitConverter.GetBytes(index), 0, sizeof(int), Field.Store.YES));
                        document.Add(new Field("Name", anatomy.AnatomicalName, Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("AnatomyType", "Individual", Field.Store.YES, Field.Index.NOT_ANALYZED));
                        foreach (String system in anatomy.Systems)
                        {
                            document.Add(new Field(systems.FacetName, system, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup systemGroup = systems.getOrCreateGroup(system);
                            systemGroup.addAnatomy(anatomy);
                        }
                        foreach (String tag in anatomy.Tags)
                        {
                            document.Add(new Field(tags.FacetName, tag, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup tagGroup = tags.getOrCreateGroup(tag);
                            tagGroup.addAnatomy(anatomy);
                        }
                        if (anatomy.Classification != null)
                        {
                            document.Add(new Field(classifications.FacetName, anatomy.Classification, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup classificationGroup = classifications.getOrCreateGroup(anatomy.Classification);
                            classificationGroup.addAnatomy(anatomy);
                        }
                        if (anatomy.Region != null)
                        {
                            document.Add(new Field(regions.FacetName, anatomy.Region, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup regionGroup = regions.getOrCreateGroup(anatomy.Region);
                            regionGroup.addAnatomy(anatomy);
                        }
                        if (anatomy.Structure != null)
                        {
                            document.Add(new Field(structures.FacetName, anatomy.Structure, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup structureGroup = structures.getOrCreateGroup(anatomy.Structure);
                            structureGroup.addAnatomy(anatomy);
                        }

                        indexWriter.UpdateDocument(new Term("Id", index.ToString()), document);
                    }
                }

                //Add the groups to the index last so we can be sure they actually have anatomy in them.
                tags.setupGroupDocuments(indexWriter, addAnatomyToIndex);
                systems.setupGroupDocuments(indexWriter, addAnatomyToIndex);
                regions.setupGroupDocuments(indexWriter, addAnatomyToIndex);
                classifications.setupGroupDocuments(indexWriter, addAnatomyToIndex);
                structures.setupGroupDocuments(indexWriter, addAnatomyToIndex);

                indexWriter.Optimize();
            }

            searcher = new IndexSearcher(directory, true);
        }

        public void clear()
        {
            anatomyList.Clear();
            systems.clear();
            regions.clear();
            classifications.clear();
            tags.clear();
            structures.clear();

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
                return systems;
            }
        }

        public bool tryGetSystem(String name, out AnatomyGroup group)
        {
            return systems.tryGetGroup(name, out group);
        }

        public IEnumerable<AnatomyGroup> Tags
        {
            get
            {
                return tags;
            }
        }

        public bool tryGetTag(String name, out AnatomyGroup group)
        {
            return tags.tryGetGroup(name, out group);
        }

        public IEnumerable<AnatomyGroup> Regions
        {
            get
            {
                return regions;
            }
        }

        public bool tryGetRegion(String name, out AnatomyGroup group)
        {
            return regions.tryGetGroup(name, out group);
        }

        public IEnumerable<AnatomyGroup> Classifications
        {
            get
            {
                return classifications;
            }
        }

        public bool tryGetClassification(String name, out AnatomyGroup group)
        {
            return classifications.tryGetGroup(name, out group);
        }

        public IEnumerable<AnatomyGroup> Structures
        {
            get
            {
                return structures;
            }
        }

        public bool tryGetStructure(String name, out AnatomyGroup group)
        {
            return structures.tryGetGroup(name, out group);
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

        private void breakdownGroup(String groupTitleFormat, String mainFacet, AnatomyGroup group, FacetGroupManager breakdownFacet)
        {
            anatomyController.displayAnatomy(String.Format("{0} by {1}", group.AnatomicalName, breakdownFacet.FacetName),
                breakdownGroupSearch(groupTitleFormat, mainFacet, group.AnatomicalName, breakdownFacet.FacetName, breakdownFacet.Select(i => i.AnatomicalName)),
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

        private int addAnatomyToIndex(Anatomy anatomy)
        {
            int index = anatomyList.Count;
            anatomyList.Add(anatomy);
            return index;
        }
    }
}
