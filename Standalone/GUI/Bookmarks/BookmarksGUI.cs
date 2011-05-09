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
    public class BookmarksGUI : PopupContainer
    {
        BookmarksController bookmarksController;

        ButtonGrid bookmarksList;
        ImageAtlas imageAtlas = new ImageAtlas("Bookmarks", new Size2(50, 50), new Size2(512, 512));
        Edit bookmarkName;

        public BookmarksGUI(BookmarksController bookmarksController)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout")
        {
            this.bookmarksController = bookmarksController;

            bookmarksList = new ButtonGrid((ScrollView)widget.findWidget("BookmarksList"));

            Button addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            bookmarkName = (Edit)widget.findWidget("BookmarkName");
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Bookmark bookmark = bookmarksController.createBookmark(bookmarkName.Caption);
            String imageKey;
            using(Bitmap thumbnail = bookmarksController.createThumbnail(bookmark))
            {
                imageKey = imageAtlas.addImage(bookmark, thumbnail);
            }
            ButtonGridItem item = bookmarksList.addItem("User", bookmark.Name, imageKey);
            item.ItemClicked += new EventHandler(item_ItemClicked);
            item.UserObject = bookmark;
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem listItem = (ButtonGridItem)sender;
            Bookmark bookmark = (Bookmark)listItem.UserObject;
            bookmarksController.applyBookmark(bookmark);
            //this.hide();
        }
    }
}
