using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;
using System.Drawing;

namespace Medical.GUI
{
    public class BookmarksGUI : PopupContainer, FullscreenGUIPopup
    {
        BookmarksController bookmarksController;
        GUIManager guiManager;

        ButtonGrid bookmarksList;
        Edit bookmarkName;

        IntSize2 widgetSmallSize;

        public BookmarksGUI(BookmarksController bookmarksController, GUIManager guiManager)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout")
        {
            this.bookmarksController = bookmarksController;
            bookmarksController.BookmarkAdded += new BookmarkDelegate(bookmarksController_BookmarkAdded);

            this.guiManager = guiManager;

            ScrollView bookmarksListScroll = (ScrollView)widget.findWidget("BookmarksList");
            bookmarksList = new ButtonGrid(bookmarksListScroll);
            bookmarksList.HighlightSelectedButton = false;

            Button addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            bookmarkName = (Edit)widget.findWidget("BookmarkName");

            widgetSmallSize = new IntSize2(widget.Width, widget.Height - bookmarksListScroll.Height);
            widget.setSize(widgetSmallSize.Width, widgetSmallSize.Height);

            this.Showing += new EventHandler(BookmarksGUI_Showing);
            this.Hidden += new EventHandler(BookmarksGUI_Hidden);
        }

        public void setSize(int width, int height)
        {
            widget.setSize(width, height);
            bookmarksList.resizeAndLayout(width);
        }

        public void setPosition(int x, int y)
        {
            widget.setPosition(x, y);
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bookmarksController.createBookmark(bookmarkName.Caption);
        }

        void bookmarksController_BookmarkAdded(Bookmark bookmark)
        {
            String imageKey = bookmarksController.createThumbnail(bookmark);
            ButtonGridItem item = bookmarksList.addItem("User", bookmark.Name, imageKey);
            item.ItemClicked += new EventHandler(item_ItemClicked);
            item.UserObject = bookmark;
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem listItem = (ButtonGridItem)sender;
            Bookmark bookmark = (Bookmark)listItem.UserObject;
            bookmarksController.applyBookmark(bookmark);
            this.hide();
        }

        void BookmarksGUI_Hidden(object sender, EventArgs e)
        {
            guiManager.removeFullscreenPopup(this);
        }

        void BookmarksGUI_Showing(object sender, EventArgs e)
        {
            guiManager.addFullscreenPopup(this);
        }
    }
}
