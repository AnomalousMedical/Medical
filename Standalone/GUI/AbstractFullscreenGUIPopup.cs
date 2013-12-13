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
        private LayoutElementName elementName = new LayoutElementName(GUILocationNames.FullscreenPopup);

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
            guiManager.closeElement(elementName, layoutContainer);
        }

        void ChooseSceneDialog_Showing(object sender, EventArgs e)
        {
            guiManager.changeElement(elementName, layoutContainer);
        }
    }
}
