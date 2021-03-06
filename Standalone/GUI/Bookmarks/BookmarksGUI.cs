﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;
using Logging;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;

namespace Medical.GUI
{
    public class BookmarksGUI : AbstractFullscreenGUIPopup
    {
        private static readonly int BookmarkSize = ScaleHelper.Scaled(200);
        private static readonly int BookmarkThumbSize = BookmarkSize;

        private BookmarksController bookmarksController;

        private NoSelectButtonGrid bookmarksList;
        private EditBox bookmarkName;

        private Tree folderTree;
        private Dictionary<BookmarkPath, TreeNode> pathNodes = new Dictionary<BookmarkPath, TreeNode>();

        private IntSize2 widgetSmallSize;
        private Button addButton;
        private Button emptyTrashButton;

        private ImageBox dragIconPreview;
        private IntVector2 dragMouseStartPosition;
        private ImageBox lockedFeatureImage;
        private bool wasDragging = false;

        private Button addFolder;
        private Button removeFolder;

        private ButtonGridLiveThumbnailController<Bookmark> liveThumbController;

        public BookmarksGUI(BookmarksController bookmarksController, GUIManager guiManager, SceneViewController sceneViewController)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout", guiManager)
        {
            this.bookmarksController = bookmarksController;
            bookmarksController.BookmarkAdded += bookmarksController_BookmarkAdded;
            bookmarksController.BookmarkRemoved += bookmarksController_BookmarkRemoved;
            bookmarksController.PremiumBookmarksChanged += bookmarksController_PremiumBookmarksChanged;
            bookmarksController.BookmarkPathAdded += bookmarksController_BookmarkPathAdded;
            bookmarksController.BookmarkPathRemoved += bookmarksController_BookmarkPathRemoved;
            bookmarksController.BookmarksCleared += bookmarksController_BookmarksCleared;
            bookmarksController.BookmarkPathsCleared += bookmarksController_BookmarkPathsCleared;
            bookmarksController.CurrentPathChanged += bookmarksController_CurrentPathChanged;

            ScrollView bookmarksListScroll = (ScrollView)widget.findWidget("BookmarksList");
            bookmarksList = new NoSelectButtonGrid(bookmarksListScroll);
            bookmarksList.ItemRemoved += bookmarksList_ItemRemoved;

            folderTree = new Tree(widget.findWidget("FolderTree") as ScrollView);
            folderTree.AfterSelect += folderTree_AfterSelect;

            addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            emptyTrashButton = widget.findWidget("EmptyTrash") as Button;
            emptyTrashButton.MouseButtonClick += emptyTrashButton_MouseButtonClick;
            emptyTrashButton.Visible = false;

            bookmarkName = (EditBox)widget.findWidget("BookmarkName");

            widgetSmallSize = new IntSize2(widget.Width, widget.Height - bookmarksListScroll.Height);
            widget.setSize(widgetSmallSize.Width, widgetSmallSize.Height);
            this.Showing += BookmarksGUI_Showing;

            dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, BookmarkSize, BookmarkSize, Align.Default, "Info", "BookmarksDragIconPreview");
            dragIconPreview.Visible = false;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            toggleAddCustomBookmarks();

            liveThumbController = new ButtonGridLiveThumbnailController<Bookmark>("Bookmarks_", new IntSize2(BookmarkThumbSize, BookmarkThumbSize), sceneViewController, bookmarksList, bookmarksListScroll);

            addFolder = widget.findWidget("AddFolder") as Button;
            addFolder.MouseButtonClick += addFolder_MouseButtonClick;
            removeFolder = widget.findWidget("RemoveFolder") as Button;
            removeFolder.MouseButtonClick += removeFolder_MouseButtonClick;
        }

        public override void Dispose()
        {
            if(lockedFeatureImage != null)
            {
                Gui.Instance.destroyWidget(lockedFeatureImage);
            }
            liveThumbController.Dispose();
            bookmarksController.BookmarkAdded -= bookmarksController_BookmarkAdded;
            bookmarksController.BookmarkRemoved -= bookmarksController_BookmarkRemoved;
            bookmarksController.PremiumBookmarksChanged -= bookmarksController_PremiumBookmarksChanged;
            Gui.Instance.destroyWidget(dragIconPreview);
            folderTree.Dispose();
            base.Dispose();
        }

        protected override void layoutUpdated()
        {
            bookmarksList.resizeAndLayout();
            liveThumbController.determineVisibleHosts();
            folderTree.layout();
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                try
                {
                    bookmarksController.createBookmark(bookmarkName.Caption);
                }
                catch (Exception ex)
                {
                    MessageBox.show(String.Format("There was an error creating this bookmark.\nTry using a different name and do not include special characters such as \\ / : * ? \" < > and |."), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                    Log.Error("Exception saving bookmark. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
                }
            }
            else
            {
                showBuyMessage();
            }
        }

        void bookmarksList_ItemRemoved(ButtonGrid grid, ButtonGridItem item)
        {
            liveThumbController.itemRemoved(item);
        }

        void bookmarksController_BookmarkAdded(Bookmark bookmark)
        {
            ButtonGridItem item = bookmarksList.addItem("User", bookmark.Name);
            item.ItemClicked += new EventHandler(item_ItemClicked);
            item.MouseDrag += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseDrag);
            item.MouseButtonReleased += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonReleased);
            item.MouseButtonPressed += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonPressed);
            item.UserObject = bookmark;
            liveThumbController.itemAdded(item, bookmark.Layers, bookmark.CameraPosition.Translation, bookmark.CameraPosition.LookAt, bookmark);
        }

        void bookmarksController_BookmarkRemoved(Bookmark bookmark)
        {
            ButtonGridItem item = liveThumbController.findItemByUserObject(bookmark);
            if (item != null)
            {
                bookmarksList.removeItem(item);
            }
        }

        void bookmarksController_BookmarksCleared()
        {
            bookmarksList.clear();
        }

        void bookmarksController_BookmarkPathsCleared()
        {
            folderTree.Nodes.clear();
            pathNodes.Clear();
        }

        void bookmarksController_BookmarkPathAdded(BookmarkPath path)
        {
            BookmarksTreeNode bookmarkNode = new BookmarksTreeNode(path.DisplayName, new BookmarksTreeNodeWidget());
            bookmarkNode.UserData = path;
            pathNodes.Add(path, bookmarkNode);
            if (path.Parent != null)
            {
                pathNodes[path.Parent].Children.add(bookmarkNode);
            }
            else
            {
                folderTree.Nodes.add(bookmarkNode);
                bookmarkNode.Expanded = true;
            }
            folderTree.layout();
        }

        void bookmarksController_BookmarkPathRemoved(BookmarkPath path)
        {
            TreeNode bookmarkNode = pathNodes[path];
            pathNodes.Remove(path);
            if (path.Parent != null)
            {
                pathNodes[path.Parent].Children.remove(bookmarkNode);
            }
            else
            {
                folderTree.Nodes.remove(bookmarkNode);
            }
            folderTree.layout();
        }

        void item_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            wasDragging = false;
            dragMouseStartPosition = arg.Position;
            currentDragNode = null;
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                if (dragIconPreview.Visible)
                {
                    wasDragging = true;
                    dragIconPreview.setItemResource(null);
                    dragIconPreview.Visible = false;
                    IntVector2 mousePos = arg.Position;
                    if(currentDragNode != null)
                    {
                        BookmarkPath path = currentDragNode.UserData as BookmarkPath;
                        Bookmark bookmark = liveThumbController.getUserObject(source);
                        currentDragNode.showHover(false);
                        try
                        {
                            bookmarksController.moveBookmark(path, bookmark);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.show(String.Format("There was an error moving this bookmark."), "Move Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                            Log.Error("Exception moving bookmark. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
                        }
                    }
                }
            }
        }

        private BookmarksTreeNode currentDragNode = null;

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
                if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
                {
                    dragIconPreview.Visible = true;
                    dragIconPreview.setImageTexture(liveThumbController.getTextureName(source));
                    dragIconPreview.setImageCoord(liveThumbController.getTextureCoord(source));
                    LayerManager.Instance.upLayerItem(dragIconPreview);
                }

                int x = arg.Position.x;
                int y = arg.Position.y;

                BookmarksTreeNode node = null;
                if(folderTree.contains(x, y))
                {
                    node = folderTree.itemAt(x, y) as BookmarksTreeNode;
                }
                if(node != currentDragNode)
                {
                    if(currentDragNode != null)
                    {
                        currentDragNode.showHover(false);
                    }
                    currentDragNode = node;
                    if(currentDragNode != null)
                    {
                        currentDragNode.showHover(true);
                    }
                }
            }
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            if (!wasDragging)
            {
                ButtonGridItem listItem = (ButtonGridItem)sender;
                Bookmark bookmark = liveThumbController.getUserObject(listItem);
                bookmarksController.applyBookmark(bookmark);
                this.hide();
            }
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        private void toggleAddCustomBookmarks()
        {
            if (bookmarksController.PremiumBookmarks)
            {
                if (lockedFeatureImage != null)
                {
                    Gui.Instance.destroyWidget(lockedFeatureImage);
                    lockedFeatureImage = null;
                }
            }
            else
            {
                if (lockedFeatureImage == null)
                {
                    int lockSize = ScaleHelper.Scaled(18);
                    lockedFeatureImage = (ImageBox)widget.createWidgetT("ImageBox", "ImageBox", addButton.Left, addButton.Top + (addButton.Height - lockSize) / 2, lockSize, lockSize, Align.Left | Align.Top, "LockedFeatureImage");
                    lockedFeatureImage.NeedMouseFocus = false;
                    lockedFeatureImage.setItemResource("LockedFeature");
                }
            }
        }

        public event Action ShowBuyMessage;

        private void showBuyMessage()
        {
            if(ShowBuyMessage != null)
            {
                ShowBuyMessage.Invoke();
            }
        }

        void bookmarksController_PremiumBookmarksChanged(BookmarksController obj)
        {
            toggleAddCustomBookmarks();
        }

        void BookmarksGUI_Showing(object sender, EventArgs e)
        {
            liveThumbController.updateAllThumbs();
        }

        void folderTree_AfterSelect(object sender, TreeEventArgs e)
        {
            bookmarksController.CurrentPath = e.Node.UserData as BookmarkPath;
        }

        void bookmarksController_CurrentPathChanged(BookmarkPath path)
        {
            removeFolder.Enabled = path != null && path.Parent != null && !path.IsTrash;
            addFolder.Enabled = path != null && !path.IsTrash;
            if (path != null)
            {
                TreeNode node = pathNodes[path];
                if (folderTree.SelectedNode != node)
                {
                    folderTree.SelectedNode = pathNodes[path];
                }
                emptyTrashButton.Visible = path.IsTrash;
            }
        }

        void addFolder_MouseButtonClick(Widget source, EventArgs e)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                try
                {
                    bookmarksController.addFolder(bookmarkName.Caption);
                }
                catch (Exception ex)
                {
                    MessageBox.show(String.Format("There was an error creating the folder.\nTry using a different name and do not include special characters such as \\ / : * ? \" < > and |."), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                    Log.Error("Exception creating bookmark folder. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
                }
            }
            else
            {
                showBuyMessage();
            }
        }

        void removeFolder_MouseButtonClick(Widget source, EventArgs e)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                if(bookmarksController.CurrentPath != null)
                {
                    MessageBox.show(String.Format("Are you sure you want to delete the folder {0} and all of its contents?\nThis cannot be undone.", bookmarksController.CurrentPath.DisplayName), "Delete", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                    {
                        if(result == MessageBoxStyle.Yes)
                        {
                            try
                            {
                                bookmarksController.removeFolder(bookmarksController.CurrentPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.show(String.Format("There was an error deleting the folder."), "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                                Log.Error("Exception deleteing bookmark folder. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
                            }
                        }
                    });
                }
            }
            else
            {
                showBuyMessage();
            }
        }

        void emptyTrashButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                if (bookmarksController.CurrentPath != null && bookmarksController.CurrentPath.IsTrash)
                {
                    MessageBox.show(String.Format("Are you sure you want to empty your bookmarks trash?\nThis cannot be undone.", bookmarksController.CurrentPath.DisplayName), "Delete", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, result =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            try
                            {
                                bookmarksController.emptyTrash();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.show(String.Format("There was an error emptying the trash."), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                                Log.Error("Exception deleteing bookmark folder. Type {0}. Message {1}.", ex.GetType().ToString(), ex.Message);
                            }
                        }
                    });
                }
            }
            else
            {
                showBuyMessage();
            }
        }
    }
}
