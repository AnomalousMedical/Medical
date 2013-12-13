using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class AbstractFullscreenGUIPopup : PopupContainer
    {
        private GUIManager guiManager;
        private MyGUILayoutContainer layoutContainer;

        public AbstractFullscreenGUIPopup(String layout, GUIManager guiManager)
            :base(layout)
        {
            this.guiManager = guiManager;
            layoutContainer = new MyGUILayoutContainer(widget);
            layoutContainer.LayoutChanged += new Action(layoutUpdated);

            this.Showing += new EventHandler(ChooseSceneDialog_Showing);
            this.Hidden += new EventHandler(ChooseSceneDialog_Hidden);
        }

        protected virtual void layoutUpdated()
        {
            
        }

        void ChooseSceneDialog_Hidden(object sender, EventArgs e)
        {
            guiManager.removeFullscreenPopup(layoutContainer);
        }

        void ChooseSceneDialog_Showing(object sender, EventArgs e)
        {
            guiManager.addFullscreenPopup(layoutContainer);
        }
    }
}
