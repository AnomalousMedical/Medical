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
        private BasicController basicController;

        private KryptonCommand showNavigationCommand;

        private NavigationController navigationController;
        private KryptonRibbonGroup viewGroup;
        private ImageList menuImageList = new ImageList();
        private List<String> menuImageListIndex = new List<string>();

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
            if (currentEntry.States != null)
            {
                KryptonContextMenuHeading heading = new KryptonContextMenuHeading(currentEntry.Text);
                menu.Items.Add(heading);
                KryptonContextMenuImageSelect imageSelect = new KryptonContextMenuImageSelect();
                imageSelect.LineItems = 6;
                imageSelect.ImageList = menuImageList;
                imageSelect.ImageIndexStart = menuImageList.Images.Count;
                imageSelect.SelectedIndexChanged += imageSelect_SelectedIndexChanged;
                menu.Items.Add(imageSelect);
                foreach (NavigationState state in currentEntry.States)
                {
                    menuImageList.Images.Add(state.Thumbnail);
                    menuImageListIndex.Add(state.Name);
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
                navigationController.setNavigationState(menuImageListIndex[imageSelect.SelectedIndex], basicController.DrawingWindowController.getActiveWindow().DrawingWindow);
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
