using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.Drawing;

namespace Medical.GUI
{
    delegate void NavigationShortcutEvent(String navigationState, String layerState);

    class NavigationShortcut : LayoutContainer, IDisposable
    {
        public event NavigationShortcutEvent ShortcutActivated;

        private class MenuImageIndex
        {
            public MenuImageIndex(String entryName, String layerState)
            {
                this.EntryName = entryName;
                this.LayerState = layerState;
            }

            public String EntryName { get; set; }
            public String LayerState { get; set; }
        }

        private ImageAtlas imageAtlas;
        private Button mainButton;
        private Button menuButton;
        private PopupContainer popupMenu;
        private ButtonGrid buttonGrid;
        private ScrollView scrollView;
        private MenuImageIndex defaultItem = null;

        public NavigationShortcut(Button mainButton, Button menuButton, ImageAtlas navigationController)
        {
            this.mainButton = mainButton;
            this.menuButton = menuButton;
            this.imageAtlas = navigationController;
        }

        public void Dispose()
        {
            buttonGrid.Dispose();
            Gui.Instance.destroyWidget(scrollView);
            Gui.Instance.destroyWidget(mainButton);
            Gui.Instance.destroyWidget(menuButton);
        }

        public void createSubMenu(NavigationMenuEntry topEntry)
        {
            createImageGallerySubMenu(topEntry);
            menuButton.MouseButtonClick += new MyGUIEvent(menuButton_MouseButtonClick);
            mainButton.MouseButtonClick += new MyGUIEvent(mainButton_MouseButtonClick);
        }

        void createImageGallerySubMenu(NavigationMenuEntry topEntry)
        {
            scrollView = Gui.Instance.createWidgetT("ScrollView", "CustomScrollView2", 0, 0, 300, 300, Align.Left | Align.Top, "Overlapped", "") as ScrollView;
            scrollView.CanvasAlign = Align.Left | Align.Top;
            scrollView.VisibleHScroll = scrollView.VisibleVScroll = false;

            buttonGrid = new ButtonGrid(scrollView);
            buttonGrid.ItemWidth = 69;
            buttonGrid.ItemHeight = 51;
            buttonGrid.ButtonSkin = "ButtonGridImageButton";
            buttonGrid.GroupSeparatorSkin = "Separator3";
            buttonGrid.SuppressLayout = true;
            int mostElements = 0;
            foreach (NavigationMenuEntry entry in topEntry.SubEntries)
            {
                int numElements = addEntriesAsImages(entry, buttonGrid);
                if (numElements > mostElements)
                {
                    mostElements = numElements;
                }
            }
            buttonGrid.SuppressLayout = false;
            buttonGrid.layoutAndResize(mostElements);
            scrollView.Visible = false;
            popupMenu = new PopupContainer(scrollView);
        }

        int addEntriesAsImages(NavigationMenuEntry currentEntry, ButtonGrid menu)
        {
            int numElements = 0;
            if (currentEntry.SubEntries != null)
            {
                foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
                {
                    ButtonGridItem item = menu.addItem(currentEntry.Text, "", imageAtlas.addImage(entry, entry.Thumbnail));
                    item.ItemClicked += new EventHandler(item_ItemClicked);
                    //Set the parent's layer state if the entry's layer state is null
                    String layerState = entry.LayerState;
                    if (layerState == null)
                    {
                        layerState = currentEntry.LayerState;
                    }
                    MenuImageIndex index = new MenuImageIndex(entry.NavigationState, layerState);
                    item.UserObject = index;
                    ++numElements;
                    if (defaultItem == null)
                    {
                        defaultItem = index;
                    }
                }
            }
            if (currentEntry.SubEntries != null)
            {
                foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
                {
                    addEntriesAsImages(entry, menu);
                }
            }
            if (numElements > 6)
            {
                numElements = 6;
            }
            return numElements;
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem item = sender as ButtonGridItem;
            MenuImageIndex index = item.UserObject as MenuImageIndex;
            if (ShortcutActivated != null)
            {
                ShortcutActivated.Invoke(index.EntryName, index.LayerState);
            }
            ButtonGrid grid = item.ButtonGrid;
            popupMenu.hide();
        }

        void mainButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (ShortcutActivated != null)
            {
                ShortcutActivated.Invoke(defaultItem.EntryName, defaultItem.LayerState);
            }
        }

        void menuButton_MouseButtonClick(Widget source, EventArgs e)
        {
            popupMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        #region LayoutContainer

        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(mainButton);
            LayerManager.Instance.upLayerItem(menuButton);
        }

        public override void setAlpha(float alpha)
        {
            mainButton.Alpha = alpha;
            menuButton.Alpha = alpha;
        }

        public override void layout()
        {
            mainButton.setPosition((int)Location.x, (int)Location.y);
            menuButton.setPosition((int)Location.x, (int)Location.y + mainButton.Height);
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2(mainButton.Width, mainButton.Height + menuButton.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return mainButton.Visible;
            }
            set
            {
                mainButton.Visible = value;
                menuButton.Visible = value;
            }
        }

        #endregion LayoutContainer
    }
}
