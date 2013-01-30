using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI;
using Engine;
using Medical.Controller;

namespace Medical
{
    class AspectRatioTask : Task, IDisposable
    {
        private SceneViewController sceneViewController;

        private MenuItem automatic;
        private MenuItem sixteenNine;
        private MenuItem fourThree;
        private MenuItem oneOne;

        public AspectRatioTask(SceneViewController sceneViewController)
            : base("Editor.AspectRatio", "Aspect Ratio", "EditorIcons.AspectRatioIcon", TaskMenuCategories.Editor)
        {
            this.ShowOnTaskbar = false;
            this.sceneViewController = sceneViewController;
        }

        public void Dispose()
        {
            
        }

        public override void clicked(TaskPositioner positioner)
        {
            PopupMenu menu = (PopupMenu)Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1, 1, Align.Default, "Popup", "");
            menu.Visible = false;
            automatic = menu.addItem("Automatic");
            sixteenNine = menu.addItem("16:9");
            fourThree = menu.addItem("4:3");
            oneOne = menu.addItem("1:1");
            menu.ItemAccept += new MyGUIEvent(menu_ItemAccept);
            menu.Closed += new MyGUIEvent(menu_Closed);

            IntVector2 position = positioner.findGoodWindowPosition(menu.Width, menu.Height);
            menu.setPosition(position.x, position.y);
            menu.setVisibleSmooth(true);
        }

        void menu_Closed(Widget source, EventArgs e)
        {
            Gui.Instance.destroyWidget(source);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        void menu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            if (mcae.Item == automatic)
            {
                sceneViewController.AutoAspectRatio = true;
            }
            if (mcae.Item == sixteenNine)
            {
                sceneViewController.AspectRatio = 16f / 9f;
                sceneViewController.AutoAspectRatio = false;
            }
            if (mcae.Item == fourThree)
            {
                sceneViewController.AspectRatio = 4f / 3f;
                sceneViewController.AutoAspectRatio = false;
            }
            if (mcae.Item == oneOne)
            {
                sceneViewController.AspectRatio = 1f;
                sceneViewController.AutoAspectRatio = false;
            }
        }
    }
}
