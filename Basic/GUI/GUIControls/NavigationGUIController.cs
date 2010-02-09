using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;
using ComponentFactory.Krypton.Ribbon;
using System.Drawing;

namespace Medical.GUI
{
    public class NavigationGUIController
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

        private BasicController basicController;

        private KryptonCommand showNavigationCommand;

        private NavigationController navigationController;
        private LayerController layerController;
        private KryptonRibbonGroup viewGroup;
        private ImageList menuImageList = new ImageList();
        private List<MenuImageIndex> menuImageListIndex = new List<MenuImageIndex>();

        public NavigationGUIController(BasicForm basicForm, BasicController basicController, ShortcutController shortcuts)
        {
            this.basicController = basicController;

            menuImageList.ColorDepth = ColorDepth.Depth32Bit;
            menuImageList.ImageSize = new Size(51, 38);

            showNavigationCommand = basicForm.showNavigationCommand;
            showNavigationCommand.Execute += new EventHandler(showNavigationCommand_Execute);

            ShortcutGroup shortcutGroup = shortcuts.createOrRetrieveGroup("MainUI");
            ShortcutEventCommand navigationShortcut = new ShortcutEventCommand("Navigation", Keys.Space, false);
            navigationShortcut.Execute += navigationShortcut_Execute;
            shortcutGroup.addShortcut(navigationShortcut);

            this.navigationController = basicController.NavigationController;
            navigationController.NavigationStateSetChanged += new NavigationControllerEvent(navigationController_NavigationStateSetChanged);
            viewGroup = basicForm.viewGroup;

            this.layerController = basicController.LayerController;
        }

        void showNavigationCommand_Execute(object sender, EventArgs e)
        {
            basicController.ShowNavigation = showNavigationCommand.Checked;
        }

        void navigationShortcut_Execute(ShortcutEventCommand shortcut)
        {
            showNavigationCommand.Checked = !showNavigationCommand.Checked;
            showNavigationCommand_Execute(null, null);
        }

        void navigationController_NavigationStateSetChanged(NavigationController controller)
        {
            viewGroup.Items.Clear();
            menuImageList.Images.Clear();
            menuImageListIndex.Clear();
            KryptonRibbonGroupTriple kryptonTriple = null;
            foreach (NavigationMenuEntry topEntry in navigationController.NavigationSet.Menus.ParentEntries)
            {
                if (kryptonTriple == null || kryptonTriple.Items.Count > 2)
                {
                    kryptonTriple = new KryptonRibbonGroupTriple();
                    viewGroup.Items.Add(kryptonTriple);
                }
                KryptonRibbonGroupButton itemButton = new KryptonRibbonGroupButton();
                itemButton.TextLine1 = topEntry.Text;
                itemButton.ImageLarge = topEntry.Thumbnail;
                itemButton.ImageSmall = topEntry.Thumbnail;
                itemButton.ButtonType = GroupButtonType.DropDown;

                kryptonTriple.Items.Add(itemButton);

                if (topEntry.SubEntries != null)
                {
                    itemButton.KryptonContextMenu = createImageGallerySubMenu(topEntry);
                }
            }
        }

        KryptonContextMenu createImageGallerySubMenu(NavigationMenuEntry topEntry)
        {
            KryptonContextMenu itemMenu = new KryptonContextMenu();
            if (topEntry.SubEntries != null)
            {
                foreach (NavigationMenuEntry entry in topEntry.SubEntries)
                {
                    addEntriesAsImages(entry, itemMenu);
                }
            }
            return itemMenu;
        }

        void addEntriesAsImages(NavigationMenuEntry currentEntry, KryptonContextMenu menu)
        {
            if (currentEntry.SubEntries != null)
            {
                KryptonContextMenuHeading heading = new KryptonContextMenuHeading(currentEntry.Text);
                menu.Items.Add(heading);
                KryptonContextMenuImageSelect imageSelect = new KryptonContextMenuImageSelect();
                imageSelect.LineItems = 6;
                imageSelect.ImageList = menuImageList;
                imageSelect.ImageIndexStart = menuImageList.Images.Count;
                imageSelect.SelectedIndexChanged += imageSelect_SelectedIndexChanged;
                menu.Items.Add(imageSelect);
                foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
                {
                    menuImageList.Images.Add(entry.Thumbnail);
                    //Set the parent's layer state if the entry's layer state is null
                    String layerState = entry.LayerState;
                    if (layerState == null)
                    {
                        layerState = currentEntry.LayerState;
                    }
                    menuImageListIndex.Add(new MenuImageIndex(entry.NavigationState, layerState));
                }
                imageSelect.ImageIndexEnd = menuImageList.Images.Count - 1;
            }
            if (currentEntry.SubEntries != null)
            {
                foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
                {
                    addEntriesAsImages(entry, menu);
                }
            }
        }

        void imageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            KryptonContextMenuImageSelect imageSelect = sender as KryptonContextMenuImageSelect;
            if (imageSelect.SelectedIndex != -1)
            {
                MenuImageIndex index = menuImageListIndex[imageSelect.SelectedIndex];
                navigationController.setNavigationState(index.EntryName, basicController.DrawingWindowController.getActiveWindow().DrawingWindow);
                if (index.LayerState != null)
                {
                    layerController.applyLayerState(index.LayerState);
                }
                imageSelect.SelectedIndex = -1;
            }
        }

        //KryptonContextMenu createSubMenuMenu(NavigationMenuEntry topEntry)
        //{
        //    KryptonContextMenu itemMenu = new KryptonContextMenu();
        //    itemMenu.Items.Add(addEntriesAsSubMenus(topEntry));
        //    return itemMenu;
        //}

        //KryptonContextMenuItemBase addEntriesAsSubMenus(NavigationMenuEntry currentEntry)
        //{
        //    KryptonContextMenuItems menuItems = new KryptonContextMenuItems();
        //    if (currentEntry.SubEntries != null)
        //    {
        //        foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
        //        {
        //            KryptonContextMenuItem itemEntry = new KryptonContextMenuItem(entry.Text);
        //            itemEntry.Image = entry.Thumbnail;
        //            menuItems.Items.Add(itemEntry);
        //            itemEntry.Items.Add(addEntriesAsSubMenus(entry));
        //        }
        //    }
        //    if (currentEntry.States != null)
        //    {
        //        foreach (NavigationState state in currentEntry.States)
        //        {
        //            KryptonContextMenuItem itemEntry = new KryptonContextMenuItem(state.Name, activateState);
        //            itemEntry.Tag = state;
        //            menuItems.Items.Add(itemEntry);
        //            itemEntry.Image = state.Thumbnail;
        //        }
        //    }
        //    return menuItems;
        //}

        //void activateState(object sender, EventArgs e)
        //{
        //    KryptonContextMenuItem menuItem = sender as KryptonContextMenuItem;
        //    if (menuItem != null)
        //    {
        //        navigationController.setNavigationState(menuItem.Text, basicController.DrawingWindowController.getActiveWindow().DrawingWindow);
        //    }
        //}
    }
}
