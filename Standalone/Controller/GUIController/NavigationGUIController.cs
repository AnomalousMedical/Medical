using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Logging;
using Engine;
using System.Drawing;

namespace Medical.GUI
{
    class NavigationGUIController : IDisposable
    {
        private NavigationController navigationController;
        private SceneViewController sceneViewController;
        private LayerController layerController;
        private CheckButton showNavigationButton;

        private List<NavigationShortcut> menuButtons = new List<NavigationShortcut>();
        private FlowLayoutContainer flowLayout;
        private ScrollView navigationTab;
        private ImageAtlas ribbonMenuIcons = new ImageAtlas("NavigationRibbonMenus", new Size2(32, 32), new Size2(512, 512));
        private ImageAtlas gridItemIcons = new ImageAtlas("NavigationGridItemIcons", new Size2(69, 51), new Size2(512, 512));

        private Widget customLayersPanel;
        private Widget displayOptionsPanel;
        private Widget quickViewPanel;

        public NavigationGUIController(Widget ribbonWidget, NavigationController navigationController, SceneViewController sceneViewController, LayerController layerController)
        {
            this.navigationController = navigationController;
            this.sceneViewController = sceneViewController;
            this.layerController = layerController;

            customLayersPanel = ribbonWidget.findWidget("CustomLayersPanel");
            displayOptionsPanel = ribbonWidget.findWidget("View/DisplayOptionsPanel");
            quickViewPanel = ribbonWidget.findWidget("View/QuickViewPanel");

            showNavigationButton = new CheckButton(ribbonWidget.findWidget("Navigation/ShowNavigationButton") as Button);
            showNavigationButton.CheckedChanged += new MyGUIEvent(showNavigationButton_CheckedChanged);

            navigationTab = ribbonWidget.findWidget("NavigationTab/ScrollView") as ScrollView;

            this.navigationController = navigationController;
            navigationController.NavigationStateSetChanged += new NavigationControllerEvent(navigationController_NavigationStateSetChanged);

            flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Horizontal, 5.0f, new Vector2(0, 2));

            navigationController.ShowOverlaysChanged += new NavigationControllerEvent(navigationController_ShowOverlaysChanged);
        }

        public void Dispose()
        {
            clearMenuItems();
        }

        internal String _addImage(object key, Bitmap thumbnail)
        {
            return gridItemIcons.addImage(key, thumbnail);
        }

        private void navigationController_NavigationStateSetChanged(NavigationController controller)
        {
            clearMenuItems();
            int menuButtonHeight = 30;
            int mainButtonHeight = showNavigationButton.Button.Height - menuButtonHeight;
            flowLayout.SuppressLayout = true;
            Button lastButton = showNavigationButton.Button;
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

                NavigationShortcut navShortcut = new NavigationShortcut(mainButton, menuButton, this);
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

            quickViewPanel.setSize(lastButton.Right, quickViewPanel.Height);
            customLayersPanel.setPosition(quickViewPanel.Right, customLayersPanel.Top);

            Size2 scrollSize = navigationTab.CanvasSize;
            scrollSize.Width = customLayersPanel.Right;
            navigationTab.CanvasSize = scrollSize;
        }

        void navShortcut_ShortcutActivated(string navigationState, string layerState)
        {
            navigationController.setNavigationState(navigationState, sceneViewController.ActiveWindow);
            if (layerState != null)
            {
                layerController.applyLayerState(layerState);
            }
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

        private void showNavigationButton_CheckedChanged(Widget source, EventArgs e)
        {
            navigationController.ShowOverlays = showNavigationButton.Checked;
        }

        private void navigationController_ShowOverlaysChanged(NavigationController controller)
        {
            showNavigationButton.Checked = navigationController.ShowOverlays;
        }
    }
}
