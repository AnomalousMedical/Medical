using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    /// <summary>
    /// This class can handle opening documents of many different formats by
    /// farming the work out to DocumentHandlers that do the actual work.
    /// </summary>
    public class DocumentController
    {
        private RecentDocuments recentDocuments;
        private List<DocumentHandler> documentHandlers = new List<DocumentHandler>();

        public DocumentController()
        {
            recentDocuments = new RecentDocuments(this);
        }

        public void addDocumentHandler(DocumentHandler handler)
        {
            documentHandlers.Add(handler);
        }

        public void removeDocumentHandler(DocumentHandler handler)
        {
            documentHandlers.Remove(handler);
        }

        public void loadRecentDocuments(String recentDocsFile)
        {
            recentDocuments.load(recentDocsFile);
        }

        public void saveRecentDocuments()
        {
            recentDocuments.save();
        }

        /// <summary>
        /// Open a file with its FileHandler if one is registered. Will return
        /// true if the file was handled in some way. The file will also be
        /// added to the recent documents list if the DocumentHandler says to do
        /// so.
        /// </summary>
        /// <param name="filename"></param>
        public bool openFile(String filename)
        {
            foreach (DocumentHandler handler in documentHandlers)
            {
                if (handler.canReadFile(filename))
                {
                    if (handler.processFile(filename))
                    {
                        recentDocuments.addDocument(filename);
                    }
                    return true;
                }
            }
            return UnknownDocumentHander != null && UnknownDocumentHander.canReadFile(filename) && UnknownDocumentHander.processFile(filename);
        }

        public String getFileTypePrettyName(String filename)
        {
            foreach (DocumentHandler handler in documentHandlers)
            {
                if (handler.canReadFile(filename))
                {
                    return handler.getPrettyName(filename);
                }
            }
            return "Unknown";
        }

        public String getFileTypeIcon(String filename)
        {
            foreach (DocumentHandler handler in documentHandlers)
            {
                if (handler.canReadFile(filename))
                {
                    return handler.getIcon(filename);
                }
            }
            return "StandaloneIcons/UnknownDocument";
        }

        /// <summary>
        /// Add a document to the recent documents list.
        /// </summary>
        /// <param name="filename">The file to add to the list.</param>
        public void addToRecentDocuments(String filename)
        {
            recentDocuments.addDocument(filename);
        }

        public void removeFromRecentDocuments(String filename)
        {
            recentDocuments.removeDocument(filename);
        }

        public bool canReadFile(String filename)
        {
            foreach (DocumentHandler handler in documentHandlers)
            {
                if (handler.canReadFile(filename))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<String> RecentDocuments
        {
            get
            {
                return recentDocuments;
            }
        }

        public event RecentDocumentEvent DocumentAdded
        {
            add
            {
                recentDocuments.DocumentAdded += value;
            }
            remove
            {
                recentDocuments.DocumentAdded -= value;
            }
        }

        public event RecentDocumentEvent DocumentRemoved
        {
            add
            {
                recentDocuments.DocumentRemoved += value;
            }
            remove
            {
                recentDocuments.DocumentRemoved -= value;
            }
        }

        public event RecentDocumentEvent DocumentReaccessed
        {
            add
            {
                recentDocuments.DocumentReaccessed += value;
            }
            remove
            {
                recentDocuments.DocumentReaccessed -= value;
            }
        }

        /// <summary>
        /// This document handler will be invoked if the other registered document handlers cannot handle
        /// the chosen document.
        /// </summary>
        public DocumentHandler UnknownDocumentHander { get; set; }
    }
}
