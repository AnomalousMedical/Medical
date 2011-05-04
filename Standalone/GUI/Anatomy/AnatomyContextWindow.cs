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

        private Size2 windowExtraSize;

        public AnatomyContextWindow(AnatomyContextWindowManager windowManager)
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {
            this.windowManager = windowManager;

            Button pinButton = (Button)window.findWidget("PinButton");
            pinButton.MouseButtonClick += new MyGUIEvent(pinButton_MouseButtonClick);

            Button centerButton = (Button)window.findWidget("CenterButton");
            centerButton.MouseButtonClick += new MyGUIEvent(centerButton_MouseButtonClick);

            Button highlightButton = (Button)window.findWidget("HighlightButton");
            highlightButton.MouseButtonClick += new MyGUIEvent(highlightButton_MouseButtonClick);

            windowExtraSize = new Size2(window.Width - window.ClientCoord.width, window.Height - window.ClientCoord.height);
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

                Size2 desiredSize = layoutContainer.DesiredSize;
                window.setSize((int)(desiredSize.Width + windowExtraSize.Width + CommandUIElement.SIDE_PADDING), (int)(desiredSize.Height + windowExtraSize.Height));
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

        void centerButton_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.centerAnatomy(this);
        }

        void highlightButton_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.highlightAnatomy(this);
        }

        void AnatomyContextWindow_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
