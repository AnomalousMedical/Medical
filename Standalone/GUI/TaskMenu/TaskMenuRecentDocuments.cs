using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;
using Engine;

namespace Medical.GUI
{
    class TaskMenuRecentDocuments
    {
        public event EventDelegate DocumentClicked;

        private ButtonGrid documentGrid;
        private ScrollView documentScroller;

        private Dictionary<String, ButtonGridItem> documentsToItems = new Dictionary<String, ButtonGridItem>();
        private DocumentController documentController;

        public TaskMenuRecentDocuments(Widget widget, DocumentController documentController)
        {
            documentScroller = (ScrollView)widget.findWidget("DocumentScroller");
            documentGrid = new ButtonGrid(documentScroller, new ButtonGridTextAdjustedGridLayout());
            documentGrid.HighlightSelectedButton = false;

            this.documentController = documentController;
            documentController.DocumentAdded += new RecentDocumentEvent(documentController_DocumentAdded);
            documentController.DocumentReaccessed += new RecentDocumentEvent(documentController_DocumentReaccessed);
            documentController.DocumentRemoved += new RecentDocumentEvent(documentController_DocumentRemoved);

            foreach (String document in documentController.RecentDocuments)
            {
                addDocument(document);
            }
        }

        public void resizeAndLayout(int newWidth)
        {
            documentGrid.resizeAndLayout(newWidth);
        }

        public bool Visible
        {
            get
            {
                return documentScroller.Visible;
            }
            set
            {
                documentScroller.Visible = value;
            }
        }

        void documentController_DocumentAdded(RecentDocuments source, string document)
        {
            addDocument(document);
        }

        private void addDocument(string document)
        {
            ButtonGridItem item = documentGrid.insertItem(0, documentController.getFileTypePrettyName(document), Path.GetFileNameWithoutExtension(document), documentController.getFileTypeIcon(document));
            item.UserObject = document;
            documentsToItems.Add(document, item);
            item.ItemClicked += new EventHandler(item_ItemClicked);
        }

        private void removeDocument(string document)
        {
            ButtonGridItem item = documentsToItems[document];
            documentGrid.removeItem(item);
            documentsToItems.Remove(document);
        }

        void documentController_DocumentRemoved(RecentDocuments source, string document)
        {
            removeDocument(document);
        }

        void documentController_DocumentReaccessed(RecentDocuments source, string document)
        {
            removeDocument(document);
            addDocument(document);
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            documentController.openFile(documentGrid.SelectedItem.UserObject.ToString());
            if (DocumentClicked != null)
            {
                DocumentClicked.Invoke();
            }
        }
    }
}
