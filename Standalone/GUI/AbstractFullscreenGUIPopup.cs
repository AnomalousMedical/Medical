using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Anomalous.GuiFramework;

namespace Medical.GUI
{
    public class AbstractFullscreenGUIPopup : GUIElementPopup
    {
        public AbstractFullscreenGUIPopup(String layout, GUIManager guiManager)
            :base(layout, guiManager, new LayoutElementName(GUILocationNames.FullscreenPopup))
        {
            
        }
    }
}
