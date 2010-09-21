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

        private Button mainButton;
        private Button menuButton;
        private NavigationMenu navigationMenu;

        public NavigationShortcut(Button mainButton, Button menuButton, ImageAtlas imageAtlas)
        {
            this.mainButton = mainButton;
            this.menuButton = menuButton;
            navigationMenu = new NavigationMenu(imageAtlas);
            navigationMenu.ItemActivated += new NavigationShortcutEvent(navigationMenu_ItemActivated);
        }

        public void Dispose()
        {
            navigationMenu.Dispose();
            Gui.Instance.destroyWidget(mainButton);
            Gui.Instance.destroyWidget(menuButton);
        }

        public void createSubMenu(NavigationMenuEntry topEntry)
        {
            navigationMenu.createImageGallerySubMenu(topEntry);
            menuButton.MouseButtonClick += new MyGUIEvent(menuButton_MouseButtonClick);
            mainButton.MouseButtonClick += new MyGUIEvent(mainButton_MouseButtonClick);
        }

        void mainButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (ShortcutActivated != null)
            {
                ShortcutActivated.Invoke(navigationMenu.DefaultItem.EntryName, navigationMenu.DefaultItem.LayerState);
            }
        }

        void navigationMenu_ItemActivated(string navigationState, string layerState)
        {
            if (ShortcutActivated != null)
            {
                ShortcutActivated.Invoke(navigationState, layerState);
            }
        }

        void menuButton_MouseButtonClick(Widget source, EventArgs e)
        {
            navigationMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
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
