using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;
using MyGUIPlugin;

namespace Medical
{
    class ChangeWindowLayoutTask : Task, IDisposable
    {
        private PopupMenu windowMenu;
        private StandaloneController standaloneController;

        public ChangeWindowLayoutTask(StandaloneController standaloneController)
            : base("Medical.ChangeWindowLayout", "Window Layout", "WindowLayoutIcon", TaskMenuCategories.System)
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
            windowMenu.Closed += new MyGUIEvent(windowMenu_Closed);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(windowMenu);
        }

        public override void clicked()
        {
            IntVector2 location = findGoodWindowPosition(0, 0);
            windowMenu.setPosition(location.x, location.y);
            LayerManager.Instance.upLayerItem(windowMenu);
            windowMenu.setVisibleSmooth(true);
        }

        public override bool Active
        {
            get { return false; }
        }

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.SceneViewController.createFromPresets(standaloneController.PresetWindows.getPresetSet(source.UserObject.ToString()));
            windowMenu.setVisibleSmooth(false);
            fireItemClosed();
        }

        void windowMenu_Closed(Widget source, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
