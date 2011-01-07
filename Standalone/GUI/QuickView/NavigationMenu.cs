using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class MenuImageIndex
    {
        public MenuImageIndex(String entryName, String layerState)
        {
            this.EntryName = entryName;
            this.LayerState = layerState;
        }

        public String EntryName { get; set; }
        public String LayerState { get; set; }
    }

    class NavigationMenu : PopupContainer
    {
        public event NavigationShortcutEvent ItemActivated;

        private ImageAtlas imageAtlas;
        private ButtonGrid buttonGrid;
        private ScrollView scrollView;

        private MenuImageIndex defaultItem = null;

        public NavigationMenu(ImageAtlas imageAtlas)
            :base(Gui.Instance.createWidgetT("ScrollView", "CustomScrollView2", 0, 0, 300, 300, Align.Left | Align.Top, "Overlapped", ""))
        {
            this.imageAtlas = imageAtlas;

            scrollView = widget as ScrollView;
            scrollView.CanvasAlign = Align.Left | Align.Top;
            scrollView.VisibleHScroll = scrollView.VisibleVScroll = false;
        }

        public override void Dispose()
        {
            imageAtlas.Dispose();
            buttonGrid.Dispose();
            Gui.Instance.destroyWidget(scrollView);
            base.Dispose();
        }

        public void createImageGallerySubMenu(NavigationMenuEntry topEntry)
        {
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
            if (ItemActivated != null)
            {
                ItemActivated.Invoke(index.EntryName, index.LayerState);
            }
            ButtonGrid grid = item.ButtonGrid;
            this.hide();
        }

        public MenuImageIndex DefaultItem
        {
            get
            {
                return defaultItem;
            }
        }
    }
}
