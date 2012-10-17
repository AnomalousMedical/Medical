﻿using System;
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
        private Widget documentInfoPanel;
        private ImageBox documentInfoIcon;
        private TextBox nameLabel;
        private TextBox locationLabel;
        private bool showDocumentInfo = false;

        public TaskMenuRecentDocuments(Widget widget, DocumentController documentController)
        {
            documentScroller = (ScrollView)widget.findWidget("DocumentScroller");
            documentGrid = new ButtonGrid(documentScroller, new ButtonGridTextAdjustedGridLayout());
            documentGrid.HighlightSelectedButton = true;
            documentGrid.ItemActivated += new EventHandler(documentGrid_ItemActivated);
            documentGrid.SelectedValueChanged += new EventHandler(documentGrid_SelectedValueChanged);

            documentInfoPanel = widget.findWidget("DocumentInfoPanel");
            documentInfoPanel.Visible = false;
            documentInfoIcon = (ImageBox)documentInfoPanel.findWidget("DocumentInfoIcon");
            nameLabel = (TextBox)documentInfoPanel.findWidget("NameLabel");
            locationLabel = (TextBox)documentInfoPanel.findWidget("LocationLabel");

            Button openButton = (Button)documentInfoPanel.findWidget("OpenButton");
            openButton.MouseButtonClick += new MyGUIEvent(openButton_MouseButtonClick);
            Button removeButton = (Button)documentInfoPanel.findWidget("RemoveButton");
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);

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
            documentGrid.resizeAndLayout(documentScroller.Width);
        }

        public bool Visible
        {
            get
            {
                return documentScroller.Visible;
            }
            set
            {
                documentInfoPanel.Visible = value && showDocumentInfo;
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

        void documentGrid_ItemActivated(object sender, EventArgs e)
        {
            openSelectedFile();
        }

        void documentGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            if (documentGrid.SelectedItem != null)
            {
                String document = documentGrid.SelectedItem.UserObject.ToString();
                documentInfoIcon.setItemResource(documentController.getFileTypeIcon(document));
                nameLabel.Caption = Path.GetFileName(document);
                locationLabel.Caption = Path.GetDirectoryName(document);
                showDocumentInfo = true;
                if (!documentInfoPanel.Visible)
                {
                    documentInfoPanel.Visible = true;
                }
            }
            else
            {
                showDocumentInfo = false;
                if (documentInfoPanel.Visible)
                {
                    documentInfoPanel.Visible = false;
                }
            }
        }

        private void openSelectedFile()
        {
            if (documentGrid.SelectedItem != null)
            {
                documentController.openFile(documentGrid.SelectedItem.UserObject.ToString());
                if (DocumentClicked != null)
                {
                    DocumentClicked.Invoke();
                }
            }
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (documentGrid.SelectedItem != null)
            {
                String document = documentGrid.SelectedItem.UserObject.ToString();
                documentController.removeFromRecentDocuments(document);
            }
        }

        void openButton_MouseButtonClick(Widget source, EventArgs e)
        {
            openSelectedFile();
        }
    }
}
