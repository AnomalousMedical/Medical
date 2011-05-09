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

        IntSize2 widgetSmallSize;

        public BookmarksGUI(BookmarksController bookmarksController)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout")
        {
            this.bookmarksController = bookmarksController;

            ScrollView bookmarksListScroll = (ScrollView)widget.findWidget("BookmarksList");
            bookmarksList = new ButtonGrid(bookmarksListScroll);

            Button addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            bookmarkName = (Edit)widget.findWidget("BookmarkName");

            widgetSmallSize = new IntSize2(widget.Width, widget.Height - bookmarksListScroll.Height);
            widget.setSize(widgetSmallSize.Width, widgetSmallSize.Height);
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

            widget.setSize(widgetSmallSize.Width, widgetSmallSize.Height + (int)bookmarksList.CanvasSize.Height + 9);
            if (widget.Height + widget.Top > Gui.Instance.getViewHeight())
            {
                widget.setSize(widgetSmallSize.Width, Gui.Instance.getViewHeight() - widget.Top);
            }
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem listItem = (ButtonGridItem)sender;
            Bookmark bookmark = (Bookmark)listItem.UserObject;
            bookmarksController.applyBookmark(bookmark);
            this.hide();
        }
    }
}
