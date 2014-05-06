using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;
using System.Drawing;
using Logging;

namespace Medical.GUI
{
    public class BookmarksGUI : AbstractFullscreenGUIPopup
    {
        private static readonly int BookmarkSize = ScaleHelper.Scaled(100);
        private static readonly int BookmarkThumbSize = BookmarkSize * 2;

        BookmarksController bookmarksController;

        NoSelectButtonGrid bookmarksList;
        EditBox bookmarkName;

        IntSize2 widgetSmallSize;
        ImageBox trash;
        Button addButton;

        private ImageBox dragIconPreview;
        private IntVector2 dragMouseStartPosition;
        private ImageBox lockedFeatureImage;

        private ButtonGridLiveThumbnailController<Bookmark> liveThumbController;

        public BookmarksGUI(BookmarksController bookmarksController, GUIManager guiManager, SceneViewController sceneViewController)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout", guiManager)
        {
            this.bookmarksController = bookmarksController;
            bookmarksController.BookmarkAdded += bookmarksController_BookmarkAdded;
            bookmarksController.BookmarkRemoved += bookmarksController_BookmarkRemoved;
            bookmarksController.PremiumBookmarksChanged += bookmarksController_PremiumBookmarksChanged;

            ScrollView bookmarksListScroll = (ScrollView)widget.findWidget("BookmarksList");
            bookmarksList = new NoSelectButtonGrid(bookmarksListScroll);

            addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            bookmarkName = (EditBox)widget.findWidget("BookmarkName");

            widgetSmallSize = new IntSize2(widget.Width, widget.Height - bookmarksListScroll.Height);
            widget.setSize(widgetSmallSize.Width, widgetSmallSize.Height);
            this.Showing += BookmarksGUI_Showing;
            this.Hidden += BookmarksGUI_Hidden;

            trash = (ImageBox)widget.findWidget("TrashPanel");
            trash.Visible = false;

            dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, BookmarkSize, BookmarkSize, Align.Default, "Info", "BookmarksDragIconPreview");
            dragIconPreview.Visible = false;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            toggleAddCustomBookmarks();

            liveThumbController = new ButtonGridLiveThumbnailController<Bookmark>("Bookmarks_", new IntSize2(BookmarkThumbSize, BookmarkThumbSize), sceneViewController, bookmarksList, bookmarksListScroll);
            liveThumbController.AllowThumbUpdate = false;
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
            base.Dispose();
        }

        public void clearBookmarks()
        {
            while(bookmarksList.Count > 0)
            {
                Bookmark bookmark = liveThumbController.getUserObject(bookmarksList.getItem(0));
                bookmarksController.removeBookmark(bookmark);
            }
        }

        protected override void layoutUpdated()
        {
            bookmarksList.resizeAndLayout(widget.Width);
            liveThumbController.determineVisibleHosts();
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
                showNagMessage();
            }
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
                liveThumbController.itemRemoved(item);
                bookmarksList.removeItem(item);
            }
        }

        void item_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            dragMouseStartPosition = arg.Position;
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                trash.Visible = false;
                dragIconPreview.setItemResource(null);
                dragIconPreview.Visible = false;
                IntVector2 mousePos = arg.Position;
                if (trash.contains(mousePos.x, mousePos.y))
                {
                    bookmarksController.removeBookmark(liveThumbController.getUserObject(source));
                }
            }
        }

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            if (bookmarksController.PremiumBookmarks)
            {
                dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
                if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
                {
                    trash.Visible = true;
                    dragIconPreview.Visible = true;
                    dragIconPreview.setImageTexture(liveThumbController.getTextureName(source));
                    dragIconPreview.setImageCoord(liveThumbController.getTextureCoord(source));
                    LayerManager.Instance.upLayerItem(dragIconPreview);
                }
                if (trash.contains(arg.Position.x, arg.Position.y))
                {
                    trash.setItemName("Highlight");
                }
                else
                {
                    trash.setItemName("Normal");
                }
            }
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem listItem = (ButtonGridItem)sender;
            Bookmark bookmark = liveThumbController.getUserObject(listItem);
            bookmarksController.applyBookmark(bookmark);
            this.hide();
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

        private static void showNagMessage()
        {
            MessageBox.show("Placeholder for nag message", "Placeholder", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok);
        }

        void bookmarksController_PremiumBookmarksChanged(BookmarksController obj)
        {
            toggleAddCustomBookmarks();
        }

        void BookmarksGUI_Showing(object sender, EventArgs e)
        {
            liveThumbController.AllowThumbUpdate = true;
        }

        void BookmarksGUI_Hidden(object sender, EventArgs e)
        {
            liveThumbController.AllowThumbUpdate = false;
        }
    }
}
