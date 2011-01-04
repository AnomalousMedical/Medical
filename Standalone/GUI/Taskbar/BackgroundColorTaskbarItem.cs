using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    public class BackgroundColorTaskbarItem : TaskbarItem
    {
        private ColorMenu colorMenu;
        private SceneViewController sceneViewController;

        public BackgroundColorTaskbarItem(SceneViewController sceneViewController)
            :base("Background Color", "BackgroundColorIconLarge")
        {
            this.sceneViewController = sceneViewController;

            colorMenu = new ColorMenu();
            colorMenu.ColorChanged += new EventHandler(colorMenu_ColorChanged);
        }

        public override void Dispose()
        {
            colorMenu.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            colorMenu.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        void colorMenu_ColorChanged(object sender, EventArgs e)
        {
            sceneViewController.ActiveWindow.BackColor = colorMenu.SelectedColor;
        }
    }
}
