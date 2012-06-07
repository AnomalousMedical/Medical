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
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ViewHost.Context.runAction(closeAction, ViewHost);
        }
    }
}
