using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class AnatomyContextWindow : Dialog
    {
        private AnatomyContextWindowManager windowManager;
        private Anatomy anatomy;
        private List<CommandUIElement> dynamicWidgets = new List<CommandUIElement>();
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5.0f, new Vector2(CommandUIElement.SIDE_PADDING / 2, 18.0f));
        private LayerState beforeFocusLayerState = null;

        public AnatomyContextWindow(AnatomyContextWindowManager windowManager)
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {
            this.windowManager = windowManager;

            Button pinButton = (Button)window.findWidget("PinButton");
            pinButton.MouseButtonClick += new MyGUIEvent(pinButton_MouseButtonClick);

            Button focusButton = (Button)window.findWidget("FocusButton");
            focusButton.MouseButtonClick += new MyGUIEvent(focusButton_MouseButtonClick);
        }

        public Anatomy Anatomy
        {
            get
            {
                return anatomy;
            }
            set
            {
                layoutContainer.SuppressLayout = true;
                foreach (CommandUIElement commandUI in dynamicWidgets)
                {
                    commandUI.Dispose();
                }
                dynamicWidgets.Clear();
                layoutContainer.clearChildren();
                this.anatomy = value;
                window.Caption = anatomy.AnatomicalName;
                foreach (AnatomyCommand command in anatomy.Commands)
                {
                    CommandUIElement commandUI = null;
                    switch (command.UIType)
                    {
                        case AnatomyCommandUIType.Numeric:
                            commandUI = new CommandHScroll(command, window);
                            break;
                        case AnatomyCommandUIType.Executable:
                            break;
                        case AnatomyCommandUIType.Boolean:
                            commandUI = new CommandCheckBox(command, window);
                            break;
                    }
                    if (commandUI != null)
                    {
                        layoutContainer.addChild(commandUI);
                        dynamicWidgets.Add(commandUI);
                    }
                }
                layoutContainer.SuppressLayout = false;
                layoutContainer.layout();
                if (beforeFocusLayerState != null)
                {
                    beforeFocusLayerState.Dispose();
                    beforeFocusLayerState = null;
                }
            }
        }

        void pinButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Button pinButton = (Button)source;
            if (pinButton.StateCheck)
            {
                this.Closed += new EventHandler(AnatomyContextWindow_Closed);
                this.close();
            }
            else
            {
                windowManager.alertWindowPinned();
                pinButton.StateCheck = true;
            }
        }

        void focusButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (beforeFocusLayerState == null)
            {
                beforeFocusLayerState = new LayerState("");
                beforeFocusLayerState.captureState();
                TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                anatomy.TransparencyChanger.smoothBlend(1.0f, MedicalConfig.TransparencyChangeMultiplier);
                windowManager.moveActiveSceneView(anatomy.Center);
            }
            else
            {
                beforeFocusLayerState.apply();
                beforeFocusLayerState.Dispose();
                beforeFocusLayerState = null;
            }
        }

        void AnatomyContextWindow_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
