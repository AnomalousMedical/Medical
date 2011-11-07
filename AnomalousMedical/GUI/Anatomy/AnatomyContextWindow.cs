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
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5.0f, new Vector2(CommandUIElement.SIDE_PADDING / 2, 84.0f));

        private IntSize2 windowStartSize;

        private StaticImage thumbnailImage;
        private StaticText anatomyName;
        private AnatomyTransparencySlider transparencySlider;
        int captionToBorderDelta = 0;
        private IntVector2 mouseOffset;

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

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            thumbnailImage = (StaticImage)widget.findWidget("ThumbnailImage");
            anatomyName = (StaticText)widget.findWidget("AnatomyName");
            captionToBorderDelta = widget.Width - anatomyName.Right;

            transparencySlider = new AnatomyTransparencySlider((HScroll)widget.findWidget("TransparencySlider"));

            windowStartSize = new IntSize2(widget.Width, widget.Height);

            Button centerButton = (Button)widget.findWidget("CenterButton");
            centerButton.MouseButtonClick += new MyGUIEvent(centerMenuItem_MouseButtonClick);

            Button highlightButton = (Button)widget.findWidget("HighlightButton");
            highlightButton.MouseButtonClick += new MyGUIEvent(highlightMenuItem_MouseButtonClick);

            Button relatedAnatomyButton = (Button)widget.findWidget("RelatedAnatomyButton");
            relatedAnatomyButton.MouseButtonClick += new MyGUIEvent(showRelated_MouseButtonClick);

            Button hideButton = (Button)widget.findWidget("HideButton");
            hideButton.MouseButtonClick += new MyGUIEvent(hideButton_MouseButtonClick);

            Button showButton = (Button)widget.findWidget("ShowButton");
            showButton.MouseButtonClick += new MyGUIEvent(showButton_MouseButtonClick);
        }

        public override void Dispose()
        {
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

                if (windowManager.ShowPremiumAnatomy)
                {
                    layoutContainer.StartLocation = new Vector2(layoutContainer.StartLocation.x, 114);
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
        }

        void centerMenuItem_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.centerAnatomy(this);
        }

        void showRelated_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.showRelatedAnatomy(anatomy);
        }

        void showButton_MouseButtonClick(Widget source, EventArgs e)
        {
            anatomy.TransparencyChanger.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime);
        }

        void hideButton_MouseButtonClick(Widget source, EventArgs e)
        {
            anatomy.TransparencyChanger.smoothBlend(0.0f, MedicalConfig.CameraTransitionTime);
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
