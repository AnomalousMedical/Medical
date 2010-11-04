using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    class QuickViewPopup : IDisposable
    {
        private NavigationController navigationController;
        private SceneViewController sceneViewController;
        private LayerController layerController;

        private List<NavigationShortcut> menuButtons = new List<NavigationShortcut>();
        private FlowLayoutContainer flowLayout;
        private ImageAtlas ribbonMenuIcons = new ImageAtlas("NavigationRibbonMenus", new Size2(32, 32), new Size2(512, 512));
        private ImageAtlas gridItemIcons = new ImageAtlas("NavigationGridItemIcons", new Size2(69, 51), new Size2(512, 512));

        private Layout layout;
        private Widget quickViewPanel;
        private PopupContainer popupContainer;

        public QuickViewPopup(NavigationController navigationController, SceneViewController sceneViewController, LayerController layerController)
        {
            this.navigationController = navigationController;
            this.sceneViewController = sceneViewController;
            this.layerController = layerController;
            navigationController.NavigationStateSetChanged += new NavigationControllerEvent(navigationController_NavigationStateSetChanged);

            layout = LayoutManager.Instance.loadLayout("Medical.GUI.QuickView.QuickViewPopup.layout");
            quickViewPanel = layout.getWidget(0);
            popupContainer = new PopupContainer(quickViewPanel);
            flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Horizontal, 5.0f, new Vector2(2, 2));
            quickViewPanel.Visible = false;
        }

        public void Dispose()
        {
            clearMenuItems();
        }

        public void show(int left, int top)
        {
            popupContainer.show(left, top);
        }

        private void navigationController_NavigationStateSetChanged(NavigationController controller)
        {
            clearMenuItems();
            int menuButtonHeight = 30;
            int mainButtonHeight = 38;
            flowLayout.SuppressLayout = true;
            Button lastButton = null;
            foreach (NavigationMenuEntry topEntry in navigationController.NavigationSet.Menus.ParentEntries)
            {
                Button menuButton = quickViewPanel.createWidgetT("Button", "RibbonSplitButton", 0, 2, 50, menuButtonHeight, Align.Left | Align.Top, "") as Button;
                menuButton.TextAlign = Align.HCenter | Align.Top;
                menuButton.Caption = topEntry.Text;
                int buttonWidth = (int)menuButton.getTextSize().Width + 5;
                if (buttonWidth < 38)
                {
                    buttonWidth = 38;
                }
                menuButton.setSize(buttonWidth, menuButtonHeight);

                Button mainButton = quickViewPanel.createWidgetT("Button", "RibbonButton", 0, 2, 50, mainButtonHeight, Align.Left | Align.Top, "") as Button;
                mainButton.StaticImage.setItemResource(ribbonMenuIcons.addImage(mainButton, topEntry.Thumbnail));
                mainButton.setSize(menuButton.Width, mainButtonHeight);

                NavigationShortcut navShortcut = new NavigationShortcut(mainButton, menuButton, gridItemIcons);
                navShortcut.ShortcutActivated += new NavigationShortcutEvent(navShortcut_ShortcutActivated);

                menuButtons.Add(navShortcut);

                flowLayout.addChild(navShortcut);

                if (topEntry.SubEntries != null)
                {
                    navShortcut.createSubMenu(topEntry);
                }

                lastButton = mainButton;
            }
            flowLayout.SuppressLayout = false;
            flowLayout.invalidate();

            quickViewPanel.setSize(lastButton.Right + 2, quickViewPanel.Height);
        }

        private void navShortcut_ShortcutActivated(string navigationState, string layerState)
        {
            navigationController.setNavigationState(navigationState, sceneViewController.ActiveWindow);
            if (layerState != null)
            {
                layerController.applyLayerState(layerState);
            }
            popupContainer.hide();
        }

        private void clearMenuItems()
        {
            flowLayout.clearChildren();
            foreach (NavigationShortcut button in menuButtons)
            {
                button.Dispose();
            }
            menuButtons.Clear();
            ribbonMenuIcons.clear();
            gridItemIcons.clear();
        }
    }
}
