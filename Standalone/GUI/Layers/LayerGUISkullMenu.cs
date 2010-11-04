using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Platform;

namespace Medical.GUI
{
    delegate void ToggleEminanceCallback(bool show);

    class LayerGUISkullMenu : LayerGUIMenu
    {
        public event ToggleEminanceCallback ToggleEminance;

        MenuItem separator;
        MenuItem showEminence;

        public LayerGUISkullMenu(Button mainButton, Button menuButton)
            :base(mainButton, menuButton)
        {
            separator = contextMenu.addItem("", MenuItemType.Separator) as MenuItem;

            showEminence = contextMenu.addItem("Show Eminence", MenuItemType.Normal) as MenuItem;
            showEminence.StateCheck = true;
            showEminence.MouseButtonClick += new MyGUIEvent(showEminence_MouseButtonClick);
        }

        public void createEminanceShortcut(KeyboardButtonCode keyboardButtonCode)
        {
            MessageEvent eminanceShortcut = new MessageEvent(new Object());
            eminanceShortcut.addButton(keyboardButtonCode);
            DefaultEvents.registerDefaultEvent(eminanceShortcut);
            eminanceShortcut.FirstFrameUpEvent += new MessageEventCallback(eminanceShortcut_FirstFrameUpEvent);
        }

        void eminanceShortcut_FirstFrameUpEvent()
        {
            if (AllowShortcuts)
            {
                ShowEminance = !ShowEminance;
            }
        }

        void showEminence_MouseButtonClick(Widget source, EventArgs e)
        {
            ShowEminance = !ShowEminance;
            contextMenu.setVisibleSmooth(false);
        }

        public bool ShowEminance
        {
            get
            {
                return showEminence.StateCheck;
            }
            set
            {
                if (showEminence.StateCheck != value)
                {
                    showEminence.StateCheck = value;
                    toggleShowEminance();
                }
            }
        }

        private void toggleShowEminance()
        {
            if (ToggleEminance != null)
            {
                ToggleEminance.Invoke(showEminence.StateCheck);
            }
        }
    }
}
