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
        private Anatomy anatomy;
        private List<CommandUIElement> dynamicWidgets = new List<CommandUIElement>();
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5.0f, new Vector2(CommandUIElement.SIDE_PADDING / 2, 5.0f));

        public AnatomyContextWindow()
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {

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
                        case AnatomyCommandUIType.Sliding:
                            commandUI = new CommandHScroll(command, window);
                            break;
                        case AnatomyCommandUIType.Button:
                            break;
                        case AnatomyCommandUIType.Toggle:
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
            }
        }
    }
}
