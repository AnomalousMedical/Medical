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
        private MultiListBox propList;

        private PropEditController propEditController;

        public OpenPropManager(PropEditController propEditController)
            :base("Medical.GUI.OpenPropManager.OpenPropManager.layout")
        {
            this.propEditController = propEditController;
            propEditController.PropOpened += propEditController_PropOpened;
            propEditController.PropClosed += propEditController_PropClosed;

            propList = (MultiListBox)window.findWidget("propList");
            propList.addColumn("Prop", 50);
            propList.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);

            Button close = (Button)window.findWidget("close");
            close.MouseButtonClick += new MyGUIEvent(close_MouseButtonClick);

            Button closeAll = (Button)window.findWidget("closeAll");
            closeAll.MouseButtonClick += new MyGUIEvent(closeAll_MouseButtonClick);
        }

        public override void Dispose()
        {
            propEditController.PropOpened -= propEditController_PropOpened;
            propEditController.PropClosed -= propEditController_PropClosed;
            base.Dispose();
        }

        void close_MouseButtonClick(Widget source, EventArgs e)
        {
            uint selectedIndex = propList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                propEditController.removeOpenProp((int)selectedIndex);
            }
        }

        void closeAll_MouseButtonClick(Widget source, EventArgs e)
        {
            propEditController.removeAllOpenProps();
        }

        void propEditController_PropOpened(PropEditController source, ShowPropAction arg)
        {
            propList.addItem(arg.PropType, arg);
        }

        void propEditController_PropClosed(PropEditController source, ShowPropAction arg)
        {
            uint itemIndex = propList.findItemWithData(arg);
            if (itemIndex != uint.MaxValue)
            {
                propList.removeItemAt(itemIndex);
            }
        }
    }
}
