using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class LayerGUISkullMenu : LayerGUIMenu
    {
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

        void showEminence_MouseButtonClick(Widget source, EventArgs e)
        {
            ShowEminance = !ShowEminance;
            contextMenu.setVisibleSmooth(false);
        }

        //public void createEminanceShortcut(String name, ShortcutGroup shortcutGroup, Keys key)
        //{
        //    ShortcutEventCommand eminanceShortcut = new ShortcutEventCommand(name, key, false);
        //    eminanceShortcut.Execute += new ShortcutEventCommand.ExecuteEvent(eminanceShortcut_Execute);
        //    shortcutGroup.addShortcut(eminanceShortcut);
        //}

        //void eminanceShortcut_Execute(ShortcutEventCommand shortcut)
        //{
        //    showEminence.Checked = !showEminence.Checked;
        //}

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
            if (showEminence.StateCheck)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(skull.CurrentAlpha);
                rightEminence.smoothBlend(skull.CurrentAlpha);
            }
            else
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(0.0f);
                rightEminence.smoothBlend(0.0f);
            }
        }
    }
}
