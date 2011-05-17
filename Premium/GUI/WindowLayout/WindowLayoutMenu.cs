using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class WindowLayoutMenu : IDisposable
    {
        private PopupMenu windowMenu;
        private StandaloneController standaloneController;

        public WindowLayoutMenu(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;

            windowMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
            windowMenu.Visible = false;
            foreach (SceneViewWindowPresetSet preset in standaloneController.PresetWindows.PresetSets)
            {
                if (!preset.Hidden)
                {
                    MenuItem item = windowMenu.addItem(preset.Name, MenuItemType.Normal);
                    item.UserObject = preset.Name;
                    item.MouseButtonClick += item_MouseButtonClick;
                }
            }
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(windowMenu);
        }

        public void show(int left, int top)
        {
            windowMenu.setPosition(left, top);
            LayerManager.Instance.upLayerItem(windowMenu);
            windowMenu.setVisibleSmooth(true);
        }

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.SceneViewController.createFromPresets(standaloneController.PresetWindows.getPresetSet(source.UserObject.ToString()));
            windowMenu.setVisibleSmooth(false);
        }
    }
}
