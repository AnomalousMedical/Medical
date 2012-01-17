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
        BookmarksController bookmarksController;

        ButtonGrid bookmarksList;
        Edit bookmarkName;

        IntSize2 widgetSmallSize;
        StaticImage trash;

        private StaticImage dragIconPreview;
        private IntVector2 dragMouseStartPosition;

        public BookmarksGUI(BookmarksController bookmarksController, GUIManager guiManager)
            : base("Medical.GUI.Bookmarks.BookmarksGUI.layout", guiManager)
        {
            this.bookmarksController = bookmarksController;
            bookmarksController.BookmarkAdded += new BookmarkDelegate(bookmarksController_BookmarkAdded);
            bookmarksController.BookmarkRemoved += new BookmarkDelegate(bookmarksController_BookmarkRemoved);

            ScrollView bookmarksListScroll = (ScrollView)widget.findWidget("BookmarksList");
            bookmarksList = new ButtonGrid(bookmarksListScroll);
            bookmarksList.HighlightSelectedButton = false;

            Button addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            bookmarkName = (Edit)widget.findWidget("BookmarkName");

            widgetSmallSize = new IntSize2(widget.Width, widget.Height - bookmarksListScroll.Height);
            widget.setSize(widgetSmallSize.Width, widgetSmallSize.Height);

            trash = (StaticImage)widget.findWidget("TrashPanel");
            trash.Visible = false;

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

        public override void setSize(int width, int height)
        {
            base.setSize(width, height);
            bookmarksList.resizeAndLayout(width);
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
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
            ButtonGridItem item = bookmarksList.findItemByUserObject(bookmark);
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
            trash.Visible = false;
            dragIconPreview.Visible = false;
            IntVector2 mousePos = arg.Position;
            if (trash.contains(mousePos.x, mousePos.y))
            {
                bookmarksController.removeBookmark((Bookmark)source.UserObject);
            }
        }

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
            {
                trash.Visible = true;
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(bookmarksController.createThumbnail((Bookmark)source.UserObject));
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

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem listItem = (ButtonGridItem)sender;
            Bookmark bookmark = (Bookmark)listItem.UserObject;
            bookmarksController.applyBookmark(bookmark);
            this.hide();
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }
    }
}
