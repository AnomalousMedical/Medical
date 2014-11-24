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
        private AnatomyGroupFacetManager systems;
        private AnatomyGroupFacetManager regions;
        private AnatomyGroupFacetManager classifications;
        private AnatomyGroupFacetManager tags;
        private AnatomyGroupFacetManager structures;

        public AnatomyLuceneSearch(AnatomyController anatomyController)
        {
            this.anatomyController = anatomyController;

            systems = new AnatomyGroupFacetManager("Systems", "System", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show System Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, systems.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Region", () => breakdownGroup("{0} of the {1}", group, regions)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Structure", () => breakdownGroup("{0} of the {1}", group, structures)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Classification", () => breakdownGroup("{1} of the {0}", group, classifications)));
            },
            anatomy =>
            {
                String system = anatomy.Systems.FirstOrDefault();
                return buildGroupFromFacets(String.Format("{0} of the {1}", system, anatomy.Region), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("System", system), new AnatomyFacet("Region", anatomy.Region)));
            });

            regions = new AnatomyGroupFacetManager("Regions", "Region", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Region Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, regions.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by System", () => breakdownGroup("{1} of the {0}", group, systems)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Classification", () => breakdownGroup("{1} of the {0}", group, classifications)));
            },
            anatomy =>
            {
                String system = anatomy.Systems.FirstOrDefault();
                return buildGroupFromFacets(String.Format("{0} of the {1}", system, anatomy.Region), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("System", system), new AnatomyFacet("Region", anatomy.Region)));
            });

            classifications = new AnatomyGroupFacetManager("Classifications", "Classification", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, classifications.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Region", () => breakdownGroup("{0} of the {1}", group, regions)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Structure", () => breakdownGroup("{0} of the {1}", group, structures)));
            },
            anatomy =>
            {
                return buildGroupFromFacets(String.Format("{0} of the {1}", anatomy.Classification, anatomy.Region), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("Classification", anatomy.Classification), new AnatomyFacet("Region", anatomy.Region)));
            });

            tags = new AnatomyGroupFacetManager("Tags", "Tag", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, tags.FacetName)));
            },
            anatomy => { throw new NotImplementedException(); });

            structures = new AnatomyGroupFacetManager("Structures", "Structure", group =>
            {
                group.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForFacet(group.AnatomicalName, structures.FacetName)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by System", () => breakdownGroup("{1} of the {0}", group, systems)));
                group.addCommand(new CallbackAnatomyCommand("Breakdown by Classification", () => breakdownGroup("{1} of the {0}", group, classifications)));
            },
            anatomy =>
            {
                String system = anatomy.Systems.FirstOrDefault();
                return buildGroupFromFacets(String.Format("{0} of the {1}", system, anatomy.Structure), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("System", system), new AnatomyFacet("Structure", anatomy.Structure)));
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

        /// <summary>
        /// Given a set of facets, build an AnatomyGroup and return it.
        /// </summary>
        /// <param name="name">The name to give the group.</param>
        /// <param name="facets">The facets to build the group with.</param>
        /// <returns>A new AnatomyGroup based on the search results.</returns>
        public AnatomyGroup buildGroupFromFacets(String name, IEnumerable<AnatomyFacet> facets)
        {
            Query query = buildQuery(null, facets);
            TopDocs results = searcher.Search(query, int.MaxValue);
            AnatomyGroup group = new AnatomyGroup(name);
            foreach (var scoreDoc in results.ScoreDocs)
            {
                var doc = searcher.Doc(scoreDoc.Doc);
                int index = BitConverter.ToInt32(doc.GetBinaryValue("DataIndex"), 0);
                group.addAnatomy(anatomyList[index]);
            }
            return group;
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
                    anatomy.addExternalCommand(
                        new CallbackAnatomyCommand("Show Groups", () => showGroupsForAnatomy(anatomy))
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
                        if (!String.IsNullOrEmpty(anatomy.Classification))
                        {
                            document.Add(new Field(classifications.FacetName, anatomy.Classification, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup classificationGroup = classifications.getOrCreateGroup(anatomy.Classification);
                            classificationGroup.addAnatomy(anatomy);
                        }
                        if (!String.IsNullOrEmpty(anatomy.Region))
                        {
                            document.Add(new Field(regions.FacetName, anatomy.Region, Field.Store.YES, Field.Index.NOT_ANALYZED));
                            AnatomyGroup regionGroup = regions.getOrCreateGroup(anatomy.Region);
                            regionGroup.addAnatomy(anatomy);
                        }
                        if (!String.IsNullOrEmpty(anatomy.Structure))
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

        public bool tryGetGroup(String name, out AnatomyGroup group)
        {
            return tags.tryGetGroup(name, out group)
                || systems.tryGetGroup(name, out group) 
                || regions.tryGetGroup(name, out group) 
                || classifications.tryGetGroup(name, out group) 
                || structures.tryGetGroup(name, out group);
        }

        public IEnumerable<AnatomyFilterEntry> FilterEntries
        {
            get
            {
                yield return systems;
                yield return regions;
                yield return classifications;
                yield return structures;
            }
        }

        /// <summary>
        /// This function provdies an enumeration over all groups related to the given anatomy.
        /// </summary>
        /// <param name="anatomyIdentifier">The AnatomyIdentifier to scan.</param>
        /// <returns>Enumerates over all AnatomyGroups that could be a group selection mode.</returns>
        public IEnumerable<AnatomyGroup> relatedGroupsFor(AnatomyIdentifier anatomyIdentifier)
        {
            AnatomyGroup group;

            foreach (var name in anatomyIdentifier.Tags)
            {
                if (tags.tryGetGroup(name, out group))
                {
                    yield return group;
                }
            }

            foreach (var name in anatomyIdentifier.Systems)
            {
                if (systems.tryGetGroup(name, out group))
                {
                    yield return group;
                }
            }

            if (anatomyIdentifier.Structure != null && structures.tryGetGroup(anatomyIdentifier.Structure, out group))
            {
                yield return group;
            }

            if (anatomyIdentifier.Classification != null && classifications.tryGetGroup(anatomyIdentifier.Classification, out group))
            {
                yield return group;
            }

            if (anatomyIdentifier.Region != null && regions.tryGetGroup(anatomyIdentifier.Region, out group))
            {
                yield return group;
            }
        }

        private Query buildQuery(String searchTerm, IEnumerable<AnatomyFacet> facets)
        {
            Query query;
            if (String.IsNullOrWhiteSpace(searchTerm) || searchTerm == "*")
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

        /// <summary>
        /// Breakdown a group
        /// </summary>
        /// <param name="groupTitleFormat">The format for the group title, {0} is the group name and {1} is the breakdown facet name.</param>
        /// <param name="group">The group to break down.</param>
        /// <param name="breakdownFacet">The facet to breakdown the group by.</param>
        private void breakdownGroup(String groupTitleFormat, AnatomyGroup group, AnatomyGroupFacetManager breakdownFacet)
        {
            anatomyController.displayAnatomy(String.Format("{0} by {1}", group.AnatomicalName, breakdownFacet.FacetName),
                breakdownGroupSearch(groupTitleFormat, group.AnatomicalName, group.Facets, breakdownFacet.FacetName, breakdownFacet.Select(i => i.AnatomicalName)),
                SuggestedDisplaySortMode.Alphabetical);
        }

        private IEnumerable<AnatomyGroup> breakdownGroupSearch(String groupTitleFormat, String groupName, IEnumerable<AnatomyFacet> parentFacets, String breakdownFacet, IEnumerable<String> breakdownValues)
        {
            foreach (var breakdownValue in breakdownValues)
            {
                AnatomyGroup resultGroup = new AnatomyGroup(String.Format(groupTitleFormat, groupName, breakdownValue));
                resultGroup.Facets = parentFacets.AddSingle(new AnatomyFacet(breakdownFacet, breakdownValue));
                resultGroup.addAnatomy(search(null, resultGroup.Facets, int.MaxValue));
                if (resultGroup.Count > 0)
                {
                    resultGroup.addCommand(new CallbackAnatomyCommand("Show Individual Anatomy", () => displayAnatomyForGroup(resultGroup)));
                    yield return resultGroup;
                }
            }
        }

        private void displayAnatomyForGroup(AnatomyGroup group)
        {
            anatomyController.displayAnatomy(String.Format("{0} Anatomy", group.AnatomicalName), group.SelectableAnatomy, SuggestedDisplaySortMode.Alphabetical);
        }

        private void showGroupsForAnatomy(AnatomyIdentifier anatomy)
        {
            anatomyController.displayAnatomy(String.Format("{0} Groups", anatomy.AnatomicalName), relatedGroupsFor(anatomy), SuggestedDisplaySortMode.Alphabetical);
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
