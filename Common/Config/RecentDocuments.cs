using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public delegate void RecentDocumentEvent(RecentDocuments source, String document);

    public class RecentDocuments : IEnumerable<String>
    {
        private const int MAX_DOCUMENTS = 10;

        public event RecentDocumentEvent DocumentAdded;
        public event RecentDocumentEvent DocumentRemoved;
        public event RecentDocumentEvent DocumentReaccessed;

        private ConfigSection section;
        private List<String> recentDocumentList = new List<string>();

        public RecentDocuments(ConfigFile configFile)
        {
            section = configFile.createOrRetrieveConfigSection("RecentDocuments");
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
