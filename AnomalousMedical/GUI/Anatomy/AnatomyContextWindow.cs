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
        private static readonly int MaxScrollerSize = ScaleHelper.Scaled(300);

        private AnatomyContextWindowManager windowManager;
        private Anatomy anatomy;
        private Dictionary<String, CommandUIElement> dynamicWidgets = new Dictionary<String, CommandUIElement>();
        private FlowLayoutContainer layoutContainer = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Vertical, 5, new IntVector2(0, 0));

        private IntSize2 windowStartSize;

        private ImageBox thumbnailImage;
        private TextBox anatomyName;
        private AnatomyTransparencySlider transparencySlider;
        int captionToBorderDelta = 0;
        private IntVector2 mouseOffset;

        private Button pinButton;
        private Button relatedAnatomyButton;

        private AnatomyContextWindowLiveThumbHost thumbHost;
        private ScrollView commandScroller;

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

            thumbnailImage = (ImageBox)widget.findWidget("ThumbnailImage");
            anatomyName = (TextBox)widget.findWidget("AnatomyName");
            captionToBorderDelta = widget.Width - anatomyName.Right;

            transparencySlider = new AnatomyTransparencySlider((ScrollBar)widget.findWidget("TransparencySlider"));

            windowStartSize = new IntSize2(widget.Width, widget.Height);

            Button centerButton = (Button)widget.findWidget("CenterButton");
            centerButton.MouseButtonClick += new MyGUIEvent(centerMenuItem_MouseButtonClick);

            Button highlightButton = (Button)widget.findWidget("HighlightButton");
            highlightButton.MouseButtonClick += new MyGUIEvent(highlightMenuItem_MouseButtonClick);

            relatedAnatomyButton = (Button)widget.findWidget("RelatedAnatomyButton");
            relatedAnatomyButton.MouseButtonClick += new MyGUIEvent(showRelated_MouseButtonClick);

            Button hideButton = (Button)widget.findWidget("HideButton");
            hideButton.MouseButtonClick += new MyGUIEvent(hideButton_MouseButtonClick);

            Button showButton = (Button)widget.findWidget("ShowButton");
            showButton.MouseButtonClick += new MyGUIEvent(showButton_MouseButtonClick);

            commandScroller = (ScrollView)widget.findWidget("CommandScroller");
        }

        public override void Dispose()
        {
            windowManager.returnThumbnail(this);
            foreach (CommandUIElement commandUI in dynamicWidgets.Values)
            {
                commandUI.Dispose();
            }
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
                foreach (CommandUIElement commandUI in dynamicWidgets.Values)
                {
                    commandUI.Dispose();
                }
                dynamicWidgets.Clear();
                layoutContainer.clearChildren();
                this.anatomy = value;
                anatomyName.Caption = anatomy.AnatomicalName;
                thumbHost = windowManager.getThumbnail(this);
                int width = windowStartSize.Width;
                int captionWidth = (int)anatomyName.getTextSize().Width;
                int totalWidth = captionWidth + anatomyName.Left + captionToBorderDelta;
                if (totalWidth > width)
                {
                    width = totalWidth;
                }
                widget.setSize(width, windowStartSize.Height);
                anatomyName.setSize(captionWidth, anatomyName.Height);
                transparencySlider.clearCommands();
                var commandPermissions = windowManager.CommandPermissions;
                foreach (AnatomyCommand command in anatomy.Commands.Where(c => c.allowDisplay(commandPermissions)))
                {
                    //Find the command
                    CommandUIElement commandUI = null;
                    if (!dynamicWidgets.TryGetValue(command.UIText, out commandUI))
                    {
                        //The command was not found, create a widget or handle the command otherwise (transparency is just added to the exising slider).
                        switch (command.UIType)
                        {
                            case AnatomyCommandUIType.Numeric:
                                commandUI = new CommandHScroll(commandScroller);
                                addCommandUI(command.UIText, commandUI);
                                break;
                            case AnatomyCommandUIType.Executable:
                                //Need to implement this
                                break;
                            case AnatomyCommandUIType.Boolean:
                                commandUI = new CommandCheckBox(commandScroller);
                                addCommandUI(command.UIText, commandUI);
                                break;
                            case AnatomyCommandUIType.Transparency:
                                transparencySlider.addCommand(command);
                                break;
                        }
                    }
       
                    //If we found something above add the command to it.
                    if (commandUI != null)
                    {
                        commandUI.addCommand(command);
                    }
                }

                layoutContainer.SuppressLayout = false;
                layoutContainer.layout();

                IntSize2 desiredSize = layoutContainer.DesiredSize;

                int scrollHeight = desiredSize.Height;
                if(scrollHeight > MaxScrollerSize)
                {
                    scrollHeight = MaxScrollerSize;
                }
                commandScroller.setSize(commandScroller.Width, scrollHeight);
                commandScroller.CanvasSize = new IntSize2(commandScroller.Width, desiredSize.Height);

                widget.setSize(width, commandScroller.Bottom + 3);
            }
        }

        public void pinOpen()
        {
            windowManager.alertWindowPinned(this);
            pinButton.Selected = true;
            this.Hidden += new EventHandler(AnatomyContextWindow_Hidden);
        }

        internal void setTextureInfo(string name, IntCoord coord)
        {
            thumbnailImage.setImageTexture(name);
            thumbnailImage.setImageCoord(coord);
        }

        internal AnatomyContextWindowLiveThumbHost ThumbHost
        {
            get
            {
                return thumbHost;
            }
        }

        void pinButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (pinButton.Selected)
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
            anatomy.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
        }

        void hideButton_MouseButtonClick(Widget source, EventArgs e)
        {
            anatomy.smoothBlend(0.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
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

        /// <summary>
        /// Add a command ui to the window.
        /// </summary>
        void addCommandUI(String key, CommandUIElement commandUI)
        {
            layoutContainer.addChild(commandUI);
            dynamicWidgets.Add(key, commandUI);
        }
    }
}
