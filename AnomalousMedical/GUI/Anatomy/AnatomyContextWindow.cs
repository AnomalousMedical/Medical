using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class AnatomyContextWindow : PopupContainer
    {
        private AnatomyContextWindowManager windowManager;
        private Anatomy anatomy;
        private List<CommandUIElement> dynamicWidgets = new List<CommandUIElement>();
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5.0f, new Vector2(CommandUIElement.SIDE_PADDING / 2, 52.0f));

        private IntSize2 windowStartSize;

        private StaticImage thumbnailImage;
        private StaticText anatomyName;
        private AnatomyTransparencySlider transparencySlider;
        int captionToBorderDelta = 0;
        private IntVector2 mouseOffset;

        private PopupMenu navMenu;
        private ShowMenuButton menuButton;
        private Button pinButton;

        public AnatomyContextWindow(AnatomyContextWindowManager windowManager)
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {
            this.windowManager = windowManager;
            KeepOpen = true;

            widget.MouseDrag += new MyGUIEvent(widget_MouseDrag);
            widget.MouseButtonPressed += new MyGUIEvent(widget_MouseButtonPressed);

            pinButton = (Button)widget.findWidget("PinButton");
            pinButton.MouseButtonClick += new MyGUIEvent(pinButton_MouseButtonClick);

            navMenu = Gui.Instance.createWidgetT("PopupMenu", "PopupMenu", 0, 0, 1000, 1000, Align.Default, "Overlapped", "") as PopupMenu;
            navMenu.Visible = false;
            MenuItem centerMenuItem = navMenu.addItem("Center");
            centerMenuItem.MouseButtonClick += new MyGUIEvent(centerMenuItem_MouseButtonClick);
            MenuItem highlightMenuItem = navMenu.addItem("Highlight");
            highlightMenuItem.MouseButtonClick += new MyGUIEvent(highlightMenuItem_MouseButtonClick);
            if (windowManager.ShowPremiumAnatomy)
            {
                MenuItem showRelated = navMenu.addItem("Search for Related Anatomy");
                showRelated.MouseButtonClick += new MyGUIEvent(showRelated_MouseButtonClick);
            }

            menuButton = new ShowMenuButton((Button)widget.findWidget("MenuButton"), navMenu);

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            thumbnailImage = (StaticImage)widget.findWidget("ThumbnailImage");
            anatomyName = (StaticText)widget.findWidget("AnatomyName");
            captionToBorderDelta = widget.Width - anatomyName.Right;

            transparencySlider = new AnatomyTransparencySlider((HScroll)widget.findWidget("TransparencySlider"));

            windowStartSize = new IntSize2(widget.Width, widget.Height);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(navMenu);
            transparencySlider.Dispose();
            base.Dispose();
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
                anatomyName.Caption = anatomy.AnatomicalName;
                thumbnailImage.setItemResource(windowManager.getThumbnail(anatomy));
                int width = windowStartSize.Width;
                int captionWidth = (int)anatomyName.getTextSize().Width;
                int totalWidth = captionWidth + anatomyName.Left + captionToBorderDelta;
                if (totalWidth > width)
                {
                    width = totalWidth;
                }
                widget.setSize(width, windowStartSize.Height);
                anatomyName.setSize(captionWidth, anatomyName.Height);
                menuButton.Button.setPosition(anatomyName.Right + 1, menuButton.Button.Top);
                transparencySlider.Command = null;
                foreach (AnatomyCommand command in anatomy.Commands)
                {
                    if (command is TransparencyChanger)
                    {
                        transparencySlider.Command = command;
                    }
                    else
                    {
                        CommandUIElement commandUI = null;
                        switch (command.UIType)
                        {
                            case AnatomyCommandUIType.Numeric:
                                commandUI = new CommandHScroll(command, widget);
                                break;
                            case AnatomyCommandUIType.Executable:
                                break;
                            case AnatomyCommandUIType.Boolean:
                                commandUI = new CommandCheckBox(command, widget);
                                break;
                        }
                        if (commandUI != null)
                        {
                            layoutContainer.addChild(commandUI);
                            dynamicWidgets.Add(commandUI);
                        }
                    }
                }
                layoutContainer.SuppressLayout = false;
                layoutContainer.layout();

                Size2 desiredSize = layoutContainer.DesiredSize;
                widget.setSize(width, (int)(desiredSize.Height));
            }
        }

        public void pinOpen()
        {
            windowManager.alertWindowPinned(this);
            pinButton.StateCheck = true;
            this.Hidden += new EventHandler(AnatomyContextWindow_Hidden);
        }

        void pinButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (pinButton.StateCheck)
            {
                this.hide();
            }
            else
            {
                pinOpen();
            }
        }

        void highlightMenuItem_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.highlightAnatomy(this);
            navMenu.setVisibleSmooth(false);
        }

        void centerMenuItem_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.centerAnatomy(this);
            navMenu.setVisibleSmooth(false);
        }

        void showRelated_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.showRelatedAnatomy(anatomy);
            navMenu.setVisibleSmooth(false);
        }

        void widget_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            mouseOffset = new IntVector2(widget.AbsoluteLeft, widget.AbsoluteTop) - me.Position;
        }

        void widget_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            widget.setPosition(me.Position.x + mouseOffset.x, me.Position.y + mouseOffset.y);
        }

        void AnatomyContextWindow_Hidden(object sender, EventArgs e)
        {
            windowManager.alertPinnedWindowClosed(this);
            this.Dispose();
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }
    }
}
