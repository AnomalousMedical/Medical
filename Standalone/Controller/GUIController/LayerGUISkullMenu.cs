using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Platform;

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
            if (showEminence.StateCheck)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(skull.CurrentAlpha, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(skull.CurrentAlpha, MedicalConfig.TransparencyChangeMultiplier);
            }
            else
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
            }
        }
    }
}
