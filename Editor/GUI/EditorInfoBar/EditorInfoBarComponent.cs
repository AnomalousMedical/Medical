using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;

namespace Medical.GUI
{
    class EditorInfoBarComponent : LayoutComponent
    {
        private String closeAction;

        public EditorInfoBarComponent(MyGUIViewHost viewHost, EditorInfoBarView view)
            :base("Medical.GUI.EditorInfoBar.EditorInfoBarComponent.layout", viewHost)
        {
            TextBox captionText = (TextBox)widget.findWidget("CaptionText");
            captionText.Caption = view.Caption;

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
            closeAction = view.CloseAction;

            MenuBar menuBar = (MenuBar)widget.findWidget("MenuBar");

            //Build the menus
            Dictionary<String, MenuControl> menus = new Dictionary<string, MenuControl>();
            foreach (EditorInfoBarAction action in view.Actions)
            {
                MenuControl menu;
                if (!menus.TryGetValue(action.Category, out menu))
                {
                    MenuItem menuItem = menuBar.addItem(action.Category, MenuItemType.Popup);
                    menu = menuBar.createItemPopupMenuChild(menuItem);
                    menu.ItemAccept += new MyGUIEvent(menu_ItemAccept);
                    menus.Add(action.Category, menu);
                }
                MenuItem item = menu.addItem(action.Name, MenuItemType.Normal, action.Action);
                item.UserObject = action;
            }
        }

        void menu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            ViewHost.Context.runAction(mcae.Item.ItemId, ViewHost);
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ViewHost.Context.runAction(closeAction, ViewHost);
        }
    }
}
