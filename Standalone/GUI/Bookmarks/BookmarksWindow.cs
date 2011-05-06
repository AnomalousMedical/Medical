using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    public class BookmarksWindow : Dialog
    {
        BookmarksController bookmarksController;

        MultiList bookmarksList;

        public BookmarksWindow(BookmarksController bookmarksController)
            : base("Medical.GUI.Bookmarks.BookmarksWindow.layout")
        {
            this.bookmarksController = bookmarksController;

            bookmarksList = (MultiList)window.findWidget("BookmarksList");
            bookmarksList.addColumn("Bookmark", bookmarksList.Width);
            bookmarksList.ListSelectAccept += new MyGUIEvent(bookmarksList_ListSelectAccept);

            Button addButton = (Button)window.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Bookmark bookmark = bookmarksController.createBookmark();
            bookmarksList.addItem("Bookmark", bookmark);
        }

        void bookmarksList_ListSelectAccept(Widget source, EventArgs e)
        {
            Bookmark bookmark = (Bookmark)bookmarksList.getItemDataAt(bookmarksList.getIndexSelected());
            bookmarksController.applyBookmark(bookmark);
        }
    }
}
