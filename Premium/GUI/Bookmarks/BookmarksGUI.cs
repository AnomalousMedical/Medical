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
        Widget trashPanel;

        private StaticImage dragIconPreview;
        private IntVector2 dragMouseStartPosition;

        public BookmarksGUI(BookmarksController bookmarksController, GUIManager guiManager)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout")
        {
            this.bookmarksController = bookmarksController;
            bookmarksController.BookmarkAdded += new BookmarkDelegate(bookmarksController_BookmarkAdded);
            bookmarksController.BookmarkRemoved += new BookmarkDelegate(bookmarksController_BookmarkRemoved);

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

            trashPanel = widget.findWidget("TrashPanel");
            trashPanel.Visible = false;

            dragIconPreview = (StaticImage)Gui.Instance.createWidgetT("StaticImage", "StaticImage", 0, 0, 100, 100, Align.Default, "Info", "BookmarksDragIconPreview");
            dragIconPreview.Visible = false;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(dragIconPreview);
            base.Dispose();
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
            item.MouseDrag += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseDrag);
            item.MouseButtonReleased += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonReleased);
            item.MouseButtonPressed += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonPressed);
            item.UserObject = bookmark;
        }

        void bookmarksController_BookmarkRemoved(Bookmark bookmark)
        {
            ButtonGridItem item = bookmarksList.findItemByCaption(bookmark.Name);
            if (item != null)
            {
                bookmarksList.removeItem(item);
            }
        }

        void item_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            dragMouseStartPosition = arg.Position;
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            trashPanel.Visible = false;
            dragIconPreview.Visible = false;
            IntVector2 mousePos = arg.Position;
            if (trashPanel.contains(mousePos.x, mousePos.y))
            {
                bookmarksController.removeBookmark((Bookmark)source.UserObject);
            }
        }

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
            {
                trashPanel.Visible = true;
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(bookmarksController.createThumbnail((Bookmark)source.UserObject));
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }
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

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }
    }
}
