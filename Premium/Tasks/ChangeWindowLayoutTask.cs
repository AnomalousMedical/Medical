using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;
using MyGUIPlugin;
using Engine.ObjectManagement;

namespace Medical
{
    class ChangeWindowLayoutTask : Task, IDisposable
    {
        private PopupMenu windowMenu;
        private SceneViewController sceneViewController;
        private SceneViewWindowPresetController presetWindows;

        public ChangeWindowLayoutTask(SceneViewController sceneViewController)
            : base("Medical.ChangeWindowLayout", "Window Layout", "WindowLayoutIcon", TaskMenuCategories.Tools)
        {
            this.sceneViewController = sceneViewController;
            this.ShowOnTimelineTaskbar = true;
        }

        public void Dispose()
        {
            destroyMenu();
        }

        public override void clicked(TaskPositioner positioner)
        {
            IntVector2 location = positioner.findGoodWindowPosition(0, 0);
            windowMenu.setPosition(location.x, location.y);
            Gui.Instance.keepWidgetOnscreen(windowMenu);
            LayerManager.Instance.upLayerItem(windowMenu);
            windowMenu.setVisibleSmooth(true);
        }

        public void sceneLoaded(SimScene scene)
        {
            destroyMenu();

            SimulationScene simScene = scene.getDefaultSubScene().getSimElementManager<SimulationScene>();
            presetWindows = simScene.WindowPresets;

            windowMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
            windowMenu.Visible = false;
            foreach (SceneViewWindowPresetSet preset in presetWindows.PresetSets)
            {
                MenuItem item = windowMenu.addItem(preset.Name, MenuItemType.Normal);
                item.UserObject = preset.Name;
                item.MouseButtonClick += item_MouseButtonClick;
            }
            windowMenu.Closed += new MyGUIEvent(windowMenu_Closed);
        }

        public override bool Active
        {
            get { return false; }
        }

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            sceneViewController.createFromPresets(presetWindows.getPresetSet(source.UserObject.ToString()));
            windowMenu.setVisibleSmooth(false);
            fireItemClosed();
        }

        void windowMenu_Closed(Widget source, EventArgs e)
        {
            fireItemClosed();
        }

        private void destroyMenu()
        {
            if (windowMenu != null)
            {
                Gui.Instance.destroyWidget(windowMenu);
            }
        }
    }
}
