using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class OpenPropManager : MDIDialog
    {
        private MultiList propList;

        public event EventDelegate<OpenPropManager, ShowPropAction> PropClosed;

        public OpenPropManager()
            :base("Medical.GUI.Timeline.OpenPropManager.layout")
        {
            propList = (MultiList)window.findWidget("propList");
            propList.addColumn("Prop", 50);
            propList.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);

            Button close = (Button)window.findWidget("close");
            close.MouseButtonClick += new MyGUIEvent(close_MouseButtonClick);

            Button closeAll = (Button)window.findWidget("closeAll");
            closeAll.MouseButtonClick += new MyGUIEvent(closeAll_MouseButtonClick);
        }

        public void addOpenProp(ShowPropAction prop)
        {
            propList.addItem(prop.PropType, prop);
            prop.KeepOpen = true;
        }

        public void removeOpenProp(ShowPropAction prop)
        {
            uint index = propList.findItemWithData(prop);
            removeOpenProp(index);
        }

        public void removeOpenProp(uint index)
        {
            ShowPropAction prop = (ShowPropAction)propList.getItemDataAt(index);
            if (prop != null)
            {
                prop.KeepOpen = false;
                propList.removeItemAt(index);
                if (PropClosed != null)
                {
                    PropClosed.Invoke(this, prop);
                }
            }
        }

        public void removeAllOpenProps()
        {
            while (propList.getItemCount() > 0)
            {
                removeOpenProp(0);
            }
        }

        public void hideOpenProps()
        {
            uint count = propList.getItemCount();
            for (uint i = 0; i < count; ++i)
            {
                ShowPropAction prop = (ShowPropAction)propList.getItemDataAt(i);
                prop.KeepOpen = false;
            }
        }

        public void showOpenProps()
        {
            uint count = propList.getItemCount();
            for (uint i = 0; i < count; ++i)
            {
                ShowPropAction prop = (ShowPropAction)propList.getItemDataAt(i);
                prop.KeepOpen = true;
            }
        }

        void close_MouseButtonClick(Widget source, EventArgs e)
        {
            uint selectedIndex = propList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                removeOpenProp(selectedIndex);
            }
        }

        void closeAll_MouseButtonClick(Widget source, EventArgs e)
        {
            removeAllOpenProps();
        }
    }
}
