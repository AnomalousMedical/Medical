using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;

namespace Medical
{
    public delegate void RecentDocumentEvent(RecentDocuments source, String document);

    public class RecentDocuments : IEnumerable<String>
    {
        private const int MAX_DOCUMENTS = 9;

        public event RecentDocumentEvent DocumentAdded;
        public event RecentDocumentEvent DocumentRemoved;
        public event RecentDocumentEvent DocumentReaccessed;

        private ConfigSection section;
        private List<String> recentDocumentList = new List<string>();

        public RecentDocuments(ConfigFile configFile)
        {
            section = configFile.createOrRetrieveConfigSection("RecentDocuments");
            section.SectionLoaded += new ConfigEvent(section_SectionLoaded);
            section.SectionSaving += new ConfigEvent(section_SectionSaving);
        }

        void section_SectionSaving(ConfigSection source)
        {
            section.clearValues();
            for(int i = 0; i < recentDocumentList.Count; ++i)
            {
                section.setValue("Document" + i, recentDocumentList[i]);
            }
        }

        void section_SectionLoaded(ConfigSection source)
        {
            recentDocumentList.Clear();
            for (int i = 0; section.hasValue("Document" + i); ++i)
            {
                String doc = section.getValue("Document" + i, "");
                if (File.Exists(doc))
                {
                    recentDocumentList.Add(doc);
                }
            }
        }

        public void addDocument(String file)
        {
            if (recentDocumentList.Contains(file))
            {
                recentDocumentList.Remove(file);
                recentDocumentList.Insert(0, file);
                if (DocumentReaccessed != null)
                {
                    DocumentReaccessed.Invoke(this, file);
                }
            }
            else
            {
                if (recentDocumentList.Count >= MAX_DOCUMENTS)
                {
                    removeDocument(recentDocumentList[recentDocumentList.Count - 1]);
                }
                recentDocumentList.Insert(0, file);
                if (DocumentAdded != null)
                {
                    DocumentAdded.Invoke(this, file);
                }
            }
        }

        public void removeDocument(String file)
        {
            if (recentDocumentList.Remove(file))
            {
                if (DocumentRemoved != null)
                {
                    DocumentRemoved.Invoke(this, file);
                }
            }
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return recentDocumentList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return recentDocumentList.GetEnumerator();
        }

        #endregion
    }
}
