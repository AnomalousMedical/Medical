using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{
    public class NavigationGUIController
    {
        private BasicController basicController;

        private KryptonCommand showNavigationCommand;

        private NavigationController navigationController;
        private KryptonRibbonGroup viewGroup;

        public NavigationGUIController(BasicForm basicForm, BasicController basicController, ShortcutController shortcuts)
        {
            this.basicController = basicController;

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
                    KryptonContextMenu itemMenu = new KryptonContextMenu();
                    KryptonContextMenuItems topItems = new KryptonContextMenuItems();
                    itemMenu.Items.Add(topItems);
                    itemButton.KryptonContextMenu = itemMenu;
                    addSubMenuEntries(topItems, topEntry);
                }
            }
        }

        void addSubMenuEntries(KryptonContextMenuItems menu, NavigationMenuEntry currentEntry)
        {
            if (currentEntry.SubEntries != null)
            {
                foreach (NavigationMenuEntry entry in currentEntry.SubEntries)
                {
                    KryptonContextMenuItem itemEntry = new KryptonContextMenuItem(entry.Text);
                    itemEntry.Image = entry.Thumbnail;
                    menu.Items.Add(itemEntry);
                    KryptonContextMenuItems subItems = new KryptonContextMenuItems();
                    itemEntry.Items.Add(subItems);
                    addSubMenuEntries(subItems, entry);
                }
            }
            if (currentEntry.States != null)
            {
                foreach (NavigationState state in currentEntry.States)
                {
                    KryptonContextMenuItem itemEntry = new KryptonContextMenuItem(state.Name, activateState);
                    itemEntry.Tag = state;
                    menu.Items.Add(itemEntry);
                    itemEntry.Image = state.Thumbnail;
                }
            }
        }

        void activateState(object sender, EventArgs e)
        {
            KryptonContextMenuItem menuItem = sender as KryptonContextMenuItem;
            if (menuItem != null)
            {
                navigationController.setNavigationState(menuItem.Text, basicController.DrawingWindowController.getActiveWindow().DrawingWindow);
            }
        }
    }
}
