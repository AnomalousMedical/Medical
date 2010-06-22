﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public delegate void ChangeTransparency(float value);

    class LayerGUIMenu : IDisposable
    {
        public event ChangeTransparency TransparencyChanged;

        protected Button mainButton;
        protected Button menuButton;
        protected PopupMenu contextMenu;
        protected MenuItem opaqueButton;
        protected MenuItem transparentButton;
        protected MenuItem hiddenButton;

        private bool allowUpdates = true;

        public LayerGUIMenu(Button mainButton, Button menuButton)
        {
            Gui gui = Gui.Instance;

            contextMenu = gui.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "LayerMenu") as PopupMenu;
            contextMenu.Visible = false;

            opaqueButton = contextMenu.addItem("Opaque", MenuItemType.Normal) as MenuItem;
            opaqueButton.StateCheck = true;
            opaqueButton.MouseButtonClick += new MyGUIEvent(opaqueButton_MouseButtonClick);

            transparentButton = contextMenu.addItem("Transparent", MenuItemType.Normal) as MenuItem;
            transparentButton.MouseButtonClick += new MyGUIEvent(transparentButton_MouseButtonClick);

            hiddenButton = contextMenu.addItem("Hidden", MenuItemType.Normal) as MenuItem;
            hiddenButton.MouseButtonClick += new MyGUIEvent(hiddenButton_MouseButtonClick);

            this.mainButton = mainButton;
            mainButton.MouseButtonClick += new MyGUIEvent(mainButton_MouseButtonClick);

            this.menuButton = menuButton;
            if (menuButton != null)
            {
                menuButton.MouseButtonClick += new MyGUIEvent(menuButton_MouseButtonClick);
            }
        }

        void menuButton_MouseButtonClick(Widget source, EventArgs e)
        {
            contextMenu.setVisibleSmooth(true);
            contextMenu.setPosition(mainButton.getAbsoluteLeft(), mainButton.getAbsoluteTop() + mainButton.getHeight());
        }

        void mainButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (opaqueButton.StateCheck)
            {
                setAlpha(0.7f);
            }
            else if (transparentButton.StateCheck)
            {
                setAlpha(0.0f);
            }
            else if (hiddenButton.StateCheck)
            {
                setAlpha(1.0f);
            }
        }

        public void Dispose()
        {
            //contextMenu.Dispose();
        }

        public void setAlpha(float alpha)
        {
            allowUpdates = false;
            if (alpha >= 1.0f)
            {
                opaqueButton.StateCheck = true;
                hiddenButton.StateCheck = false;
                transparentButton.StateCheck = false;
            }
            else if (alpha <= 0.0f)
            {
                opaqueButton.StateCheck = false;
                hiddenButton.StateCheck = true;
                transparentButton.StateCheck = false;
            }
            else
            {
                opaqueButton.StateCheck = false;
                hiddenButton.StateCheck = false;
                transparentButton.StateCheck = true;
            }
            allowUpdates = true;

            if (TransparencyChanged != null)
            {
                TransparencyChanged(alpha);
            }
        }

        void hiddenButton_MouseButtonClick(Widget source, EventArgs e)
        {
            setAlpha(0.0f);
            contextMenu.setVisibleSmooth(false);
        }

        void transparentButton_MouseButtonClick(Widget source, EventArgs e)
        {
            setAlpha(0.7f);
            contextMenu.setVisibleSmooth(false);
        }

        void opaqueButton_MouseButtonClick(Widget source, EventArgs e)
        {
            setAlpha(1.0f);
            contextMenu.setVisibleSmooth(false);
        }
    }
}
