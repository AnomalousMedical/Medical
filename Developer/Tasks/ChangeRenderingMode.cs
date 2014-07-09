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
    class ChangeRenderingMode : Task, IDisposable
    {
        private PopupMenu modeMenu;
        private SceneViewController sceneViewController;

        public ChangeRenderingMode(SceneViewController sceneViewController)
            : base("Medical.ChangeRenderingMode", "Rendering Mode", "Developer.RenderingMode", "Developer")
        {
            this.sceneViewController = sceneViewController;
        }

        public void Dispose()
        {
            destroyMenu();
        }

        public override void clicked(TaskPositioner positioner)
        {
            createMenu();
            IntVector2 location = positioner.findGoodWindowPosition(0, 0);
            modeMenu.setPosition(location.x, location.y);
            Gui.Instance.keepWidgetOnscreen(modeMenu);
            LayerManager.Instance.upLayerItem(modeMenu);
            modeMenu.setVisibleSmooth(true);
        }

        public void createMenu()
        {
            if (modeMenu == null)
            {
                modeMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
                modeMenu.Visible = false;

                MenuItem item = modeMenu.addItem("Solid", MenuItemType.Normal);
                item.UserObject = RenderingMode.Solid;
                item.MouseButtonClick += item_MouseButtonClick;

                item = modeMenu.addItem("Wireframe", MenuItemType.Normal);
                item.UserObject = RenderingMode.Wireframe;
                item.MouseButtonClick += item_MouseButtonClick;

                item = modeMenu.addItem("Points", MenuItemType.Normal);
                item.UserObject = RenderingMode.Points;
                item.MouseButtonClick += item_MouseButtonClick;
                
                modeMenu.Closed += new MyGUIEvent(windowMenu_Closed);
            }
        }

        public override bool Active
        {
            get { return false; }
        }

        void item_MouseButtonClick(Widget source, EventArgs e)
        {
            SceneViewWindow window = sceneViewController.ActiveWindow;
            if (window != null)
            {
                window.RenderingMode = (RenderingMode)source.UserObject;
            }
            modeMenu.setVisibleSmooth(false);
            fireItemClosed();
        }

        void windowMenu_Closed(Widget source, EventArgs e)
        {
            fireItemClosed();
        }

        private void destroyMenu()
        {
            if (modeMenu != null)
            {
                Gui.Instance.destroyWidget(modeMenu);
            }
        }
    }
}
