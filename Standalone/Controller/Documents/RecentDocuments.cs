using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;
using Logging;

namespace Medical
{
    public delegate void RecentDocumentEvent(RecentDocuments source, String document);

    public class RecentDocuments : IEnumerable<String>
    {
        private const int MAX_DOCUMENTS = 50;

        public event RecentDocumentEvent DocumentAdded;
        public event RecentDocumentEvent DocumentRemoved;
        public event RecentDocumentEvent DocumentReaccessed;

        private ConfigFile configFile;
        private ConfigSection section;
        private List<String> recentDocumentList = new List<string>();

        private DocumentController documentController;

        public RecentDocuments(DocumentController documentController)
        {
            this.documentController = documentController;
        }

        public void load(String backingFile)
        {
            configFile = new ConfigFile(backingFile);
            section = configFile.createOrRetrieveConfigSection("RecentDocuments");
            section.SectionLoaded += new ConfigEvent(section_SectionLoaded);
            section.SectionSaving += new ConfigEvent(section_SectionSaving);

            configFile.loadConfigFile();
            if (DocumentAdded != null)
            {
                for(int i = recentDocumentList.Count - 1; i > -1; --i)//Do list backwards, this will accurately reflect the order the items were accessed
                {
                    DocumentAdded.Invoke(this, recentDocumentList[i]);
                }
            }
        }

        public void save()
        {
            if (configFile != null)
            {
                configFile.writeConfigFile();
            }
            else
            {
                Log.Warning("Could not save recent documents because no config file is loaded.");
            }
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
                String doc = normalizePath(section.getValue("Document" + i, ""));
                if ((File.Exists(doc) || Directory.Exists(doc)) && recentDocumentList.IndexOf(doc) == -1)
                {
                    recentDocumentList.Add(doc);
                }
            }
        }

        public void addDocument(String file)
        {
            file = normalizePath(file);
            int index = recentDocumentList.IndexOf(file);
            if (index != -1)
            {
                recentDocumentList.RemoveAt(index);
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
            file = normalizePath(file);
            if (recentDocumentList.Remove(file))
            {
                if (DocumentRemoved != null)
                {
                    DocumentRemoved.Invoke(this, file);
                }
            }
        }

        private String normalizePath(String file)
        {
            StringBuilder normalized = new StringBuilder(file.Length);
            for (int i = 0; i < file.Length; ++i)
            {
                switch (file[i])
                {
                    case '\\':
                        normalized.Append(Path.DirectorySeparatorChar);
                        break;
                    default:
                        normalized.Append(file[i]);
                        break;
                }
            }
            return normalized.ToString();
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
