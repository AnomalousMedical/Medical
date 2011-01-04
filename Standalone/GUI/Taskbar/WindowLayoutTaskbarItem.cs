using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    class WindowLayoutTaskbarItem : TaskbarItem
    {
        private PopupMenu windowMenu;
        private StandaloneController standaloneController;

        public WindowLayoutTaskbarItem(StandaloneController standaloneController)
            :base("Window Layout", "WindowLayoutIconLarge")
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

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(windowMenu);
        }

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.SceneViewController.createFromPresets(standaloneController.PresetWindows.getPresetSet(source.UserObject.ToString()));
            windowMenu.setVisibleSmooth(false);
        }

        #region ITaskbarItem Members

        public override void clicked(Widget source, EventArgs e)
        {
            int left = source.AbsoluteLeft;
            int top = source.AbsoluteTop + source.Height;
            int guiWidth = Gui.Instance.getViewWidth();
            int guiHeight = Gui.Instance.getViewHeight();

            int right = left + windowMenu.Width;
            int bottom = top + windowMenu.Height;

            if (right > guiWidth)
            {
                left -= right - guiWidth;
                if (left < 0)
                {
                    left = 0;
                }
            }

            if (bottom > guiHeight)
            {
                top -= bottom - guiHeight;
                if (top < 0)
                {
                    top = 0;
                }
            }

            windowMenu.setPosition(left, top);
            LayerManager.Instance.upLayerItem(windowMenu);
            windowMenu.setVisibleSmooth(true);
        }

        #endregion
    }
}
