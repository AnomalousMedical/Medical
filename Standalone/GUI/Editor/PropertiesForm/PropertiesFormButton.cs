﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class PropertiesFormButton : PropertiesFormComponent
    {
        private MyGUILayoutContainer layoutContainer;
        private EditInterfaceCommand command;
        private Button button;
        private MedicalUICallback uiCallback;
        private EditInterface editInterface;

        public PropertiesFormButton(EditInterface editInterface, EditInterfaceCommand command, MedicalUICallback uiCallback, Widget parent)
        {
            this.editInterface = editInterface;
            this.uiCallback = uiCallback;
            this.command = command;
            button = (Button)parent.createWidgetT("Button", "Button", 0, 0, 100, 25, Align.Default, "");
            button.Caption = command.Name;
            button.ForwardMouseWheelToParent = true;
            layoutContainer = new MyGUILayoutContainer(button);
            button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(button);
        }

        public void refreshData()
        {

        }

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }

        public EditableProperty Property
        {
            get
            {
                return null;
            }
        }

        void button_MouseButtonClick(Widget source, EventArgs e)
        {
            uiCallback.SelectedEditInterface = editInterface;
            command.execute(uiCallback);
        }
    }
}
