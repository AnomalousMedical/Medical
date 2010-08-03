using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Logging;
using Engine;

namespace Medical.GUI
{
    class NavigationGUIController : IDisposable
    {
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

        private NavigationController navigationController;
        private SceneViewController sceneViewController;
        private LayerController layerController;
        private CheckButton showNavigationButton;

        private List<Button> menuButtons = new List<Button>();
        private FlowLayoutContainer flowLayout;
        private Widget navigationTab;
        private ImageAtlas ribbonMenuIcons = new ImageAtlas("NavigationRibbonMenus", new Size2(32, 32), new Size2(512, 512));
        private ImageAtlas gridItemIcons = new ImageAtlas("NavigationGridItemIcons", new Size2(69, 51), new Size2(512, 512));

        public NavigationGUIController(Widget ribbonWidget, NavigationController navigationController, SceneViewController sceneViewController, LayerController layerController)
        {
            this.navigationController = navigationController;
            this.sceneViewController = sceneViewController;
            this.layerController = layerController;

            showNavigationButton = new CheckButton(ribbonWidget.findWidget("Navigation/ShowNavigationButton") as Button);
            showNavigationButton.CheckedChanged += new MyGUIEvent(showNavigationButton_CheckedChanged);

            navigationTab = ribbonWidget.findWidget("NavigationTab");

            this.navigationController = navigationController;
            navigationController.NavigationStateSetChanged += new NavigationControllerEvent(navigationController_NavigationStateSetChanged);

            flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Horizontal, 5.0f, new Vector2(showNavigationButton.Button.getRight() + 10.0f, showNavigationButton.Button.getTop()));
        }

        public void Dispose()
        {
            clearMenuItems();
        }

        void navigationController_NavigationStateSetChanged(NavigationController controller)
        {
            clearMenuItems();
            int buttonHeight = showNavigationButton.Button.getHeight();
            flowLayout.SuppressLayout = true;
            foreach (NavigationMenuEntry topEntry in navigationController.NavigationSet.Menus.ParentEntries)
            {
                Button itemButton = navigationTab.createWidgetT("Button", "RibbonButton", 0, 0, 50, buttonHeight, Align.Left | Align.Top, "") as Button;
                itemButton.Caption = topEntry.Text;
                itemButton.setSize((int)itemButton.getTextSize().Width + 35, buttonHeight);
                itemButton.StaticImage.setItemResource(ribbonMenuIcons.addImage(itemButton, topEntry.Thumbnail));

                flowLayout.addChild(new MyGUILayoutContainer(itemButton));

                if (topEntry.SubEntries != null)
                {
                    itemButton.UserObject = createImageGallerySubMenu(topEntry);
                    itemButton.MouseButtonClick += new MyGUIEvent(itemButton_MouseButtonClick);
                }
            }
            flowLayout.SuppressLayout = false;
            flowLayout.invalidate();
        }

        PopupContainer createImageGallerySubMenu(NavigationMenuEntry topEntry)
        {
            ScrollView scrollView = Gui.Instance.createWidgetT("ScrollView", "CustomScrollView2", 0, 0, 300, 300, Align.Left | Align.Top, "Overlapped", "") as ScrollView;
            scrollView.CanvasAlign = Align.Left | Align.Top;
            scrollView.VisibleHScroll = scrollView.VisibleVScroll = false;
            ButtonGrid buttonGrid = new ButtonGrid(scrollView);
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
            PopupContainer popup = new PopupContainer(scrollView);
            popup.UserObject = buttonGrid;
            buttonGrid.UserObject = popup;
            return popup;
        }

        int addEntriesAsImages(NavigationMenuEntry currentEntry, ButtonGrid menu)
        {
            int numElements = 0;
            if (currentEntry.SubEntries != null)
            {
                foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
                {
                    ButtonGridItem item = menu.addItem(currentEntry.Text, "", gridItemIcons.addImage(entry, entry.Thumbnail));
                    item.ItemClicked += new EventHandler(item_ItemClicked);
                    //Set the parent's layer state if the entry's layer state is null
                    String layerState = entry.LayerState;
                    if (layerState == null)
                    {
                        layerState = currentEntry.LayerState;
                    }
                    item.UserObject = new MenuImageIndex(entry.NavigationState, layerState);
                    ++numElements;
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

        void itemButton_MouseButtonClick(Widget source, EventArgs e)
        {
            PopupContainer popup = source.UserObject as PopupContainer;
            popup.show(source.getAbsoluteLeft(), source.getAbsoluteTop() + source.getHeight());
        }

        void clearMenuItems()
        {
            flowLayout.clearChildren();
            foreach (Button button in menuButtons)
            {
                PopupContainer popup = button.UserObject as PopupContainer;
                ButtonGrid buttonGrid = popup.UserObject as ButtonGrid;
                ScrollView scrollView = popup.Widget as ScrollView;
                buttonGrid.Dispose();
                Gui.Instance.destroyWidget(scrollView);
                Gui.Instance.destroyWidget(button);
            }
            menuButtons.Clear();
            ribbonMenuIcons.clear();
            gridItemIcons.clear();
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            ButtonGridItem item = sender as ButtonGridItem;
            MenuImageIndex index = item.UserObject as MenuImageIndex;
            navigationController.setNavigationState(index.EntryName, sceneViewController.ActiveWindow);
            if (index.LayerState != null)
            {
                layerController.applyLayerState(index.LayerState);
            }
            ButtonGrid grid = item.ButtonGrid;
            PopupContainer popup = grid.UserObject as PopupContainer;
            popup.hide();
        }

        void showNavigationButton_CheckedChanged(Widget source, EventArgs e)
        {
            navigationController.ShowOverlays = showNavigationButton.Checked;
        }


    }
}
