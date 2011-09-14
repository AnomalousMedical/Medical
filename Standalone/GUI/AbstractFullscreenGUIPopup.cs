using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class AbstractFullscreenGUIPopup : PopupContainer, FullscreenGUIPopup
    {
        private GUIManager guiManager;

        public AbstractFullscreenGUIPopup(String layout, GUIManager guiManager)
            :base(layout)
        {
            this.guiManager = guiManager;

            this.Showing += new EventHandler(ChooseSceneDialog_Showing);
            this.Hidden += new EventHandler(ChooseSceneDialog_Hidden);
        }

        public virtual void setSize(int width, int height)
        {
            widget.setSize(width, height);
        }

        public virtual void setPosition(int x, int y)
        {
            widget.setPosition(x, y);
        }

        void ChooseSceneDialog_Hidden(object sender, EventArgs e)
        {
            guiManager.removeFullscreenPopup(this);
        }

        void ChooseSceneDialog_Showing(object sender, EventArgs e)
        {
            guiManager.addFullscreenPopup(this);
        }
    }
}
