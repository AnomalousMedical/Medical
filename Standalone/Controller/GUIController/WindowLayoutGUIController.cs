using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Standalone;
using Medical.Controller;

namespace Medical.GUI
{
    class WindowLayoutGUIController : IDisposable
    {
        private PopupMenu windowMenu;
        private StandaloneController standaloneController;

        public WindowLayoutGUIController(Widget ribbonWidget, StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;

            Button windowLayout = ribbonWidget.findWidget("UtilityTab/WindowLayoutButton") as Button;
            windowLayout.MouseButtonClick += new MyGUIEvent(windowLayout_MouseButtonClick);

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

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.SceneViewController.createFromPresets(standaloneController.PresetWindows.getPresetSet(source.UserObject.ToString()));
            windowMenu.setVisibleSmooth(false);
        }

        void windowLayout_MouseButtonClick(Widget source, EventArgs e)
        {
            LayerManager.Instance.upLayerItem(windowMenu);
            windowMenu.setPosition(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
            windowMenu.setVisibleSmooth(true);
        }
    }
}
