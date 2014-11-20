using Lucene.Net.Documents;
using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class AnatomyGroupFacetManager : IEnumerable<AnatomyGroup>
    {
        private Dictionary<String, AnatomyGroup> groups = new Dictionary<String, AnatomyGroup>();
        private Action<AnatomyGroup> setupGroup;

        public AnatomyGroupFacetManager(String facetName, Action<AnatomyGroup> setupGroup)
        {
            this.setupGroup = setupGroup;
            this.FacetName = facetName;
        }

        public void createGroups(IEnumerable<AnatomyTagProperties> properties)
        {
            foreach (var property in properties)
            {
                createGroup(property);
            }
        }

        public void createGroup(AnatomyTagProperties properties)
        {
            AnatomyGroup group = new AnatomyGroup(properties.Name, properties.ShowInBasicVersion, properties.ShowInTextSearch, properties.ShowInClickSearch, properties.ShowInTree);
            setupGroup(group);
            groups.Add(group.AnatomicalName, group);
        }

        public AnatomyGroup getOrCreateGroup(String name)
        {
            AnatomyGroup group;
            if(!groups.TryGetValue(name, out group))
            {
                group = new AnatomyGroup(name);
                setupGroup(group);
                groups.Add(group.AnatomicalName, group);
            }
            return group;
        }

        public bool tryGetGroup(String name, out AnatomyGroup group)
        {
            return groups.TryGetValue(name, out group);
        }

        public void clear()
        {
            groups.Clear();
        }

        public String FacetName { get; private set; }

        public AnatomyGroup this[String key]
        {
            get
            {
                return groups[key];
            }
        }

        public IEnumerator<AnatomyGroup> GetEnumerator()
        {
            return groups.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return groups.Values.GetEnumerator();
        }

        public void setupGroupDocuments(IndexWriter indexWriter, Func<AnatomyGroup, int> addToIndex)
        {
            foreach (var group in groups.Values)
            {
                if (group.ShowInTextSearch)
                {
                    Document document = new Document();
                    int index = addToIndex(group);
                    document.Add(new Field("Id", index.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("DataIndex", BitConverter.GetBytes(index), 0, sizeof(int), Field.Store.YES));
                    document.Add(new Field("Name", group.AnatomicalName, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("AnatomyType", FacetName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    indexWriter.UpdateDocument(new Term("Id", index.ToString()), document);
                }
            }
        }
    }
}
