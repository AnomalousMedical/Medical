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
    public class BookmarksWindow : Dialog
    {
        BookmarksController bookmarksController;

        ButtonList bookmarksList;
        ImageAtlas imageAtlas = new ImageAtlas("Bookmarks", new Size2(50, 50), new Size2(512, 512));
        Edit bookmarkName;

        public BookmarksWindow(BookmarksController bookmarksController)
            : base("Medical.GUI.Bookmarks.BookmarksWindow.layout")
        {
            this.bookmarksController = bookmarksController;

            bookmarksList = new ButtonList((ScrollView)window.findWidget("BookmarksList"));
            bookmarksList.SelectedValueChanged += new EventHandler(bookmarksList_SelectedValueChanged);

            Button addButton = (Button)window.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            bookmarkName = (Edit)window.findWidget("BookmarkName");
        }

        void bookmarksList_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonListItem listItem = bookmarksList.SelectedItem;
            Bookmark bookmark = (Bookmark)listItem.UserObject;
            bookmarksController.applyBookmark(bookmark);
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Bookmark bookmark = bookmarksController.createBookmark(bookmarkName.Caption);
            String imageKey;
            using(Bitmap thumbnail = bookmarksController.createThumbnail(bookmark))
            {
                imageKey = imageAtlas.addImage(bookmark, thumbnail);
            }
            ButtonListItem item = bookmarksList.addItem(bookmark.Name, imageKey);
            item.UserObject = bookmark;
        }

        void bookmarksList_ListSelectAccept(Widget source, EventArgs e)
        {
            //Bookmark bookmark = (Bookmark)bookmarksList.getItemDataAt(bookmarksList.getIndexSelected());
            //bookmarksController.applyBookmark(bookmark);
        }
    }
}
