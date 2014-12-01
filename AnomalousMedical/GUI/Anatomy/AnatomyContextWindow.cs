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
        private static readonly int PaddingSize = ScaleHelper.Scaled(4);
        private static readonly int MaxScrollerSize = ScaleHelper.Scaled(300);

        private AnatomyContextWindowManager windowManager;
        private Anatomy anatomy;
        private Dictionary<String, CommandUIElement> dynamicWidgets = new Dictionary<String, CommandUIElement>();
        private StretchLayoutContainer layoutContainer = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, PaddingSize, new IntVector2(0, 0));
        private LayerController layerController;

        private IntSize2 windowStartSize;

        private ImageBox thumbnailImage;
        private Button captionWidget;
        private AnatomyTransparencySlider transparencySlider;
        private int captionToBorderDelta = 0;

        private Button pinButton;
        private Button centerButton;
        private Button featureButton;
        private Button anatomyFinderButton;

        private AnatomyContextWindowLiveThumbHost thumbHost;
        private ScrollView commandScroller;

        public AnatomyContextWindow(AnatomyContextWindowManager windowManager, LayerController layerController)
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {
            this.windowManager = windowManager;
            this.layerController = layerController;

            captionWidget = window.CaptionWidget as Button;

            pinButton = (Button)window.findWidgetChildSkin("PinButton");
            pinButton.MouseButtonClick += new MyGUIEvent(pinButton_MouseButtonClick);

            thumbnailImage = (ImageBox)window.findWidget("ThumbnailImage");
            captionToBorderDelta = window.Width - captionWidget.getTextRegion().width;

            transparencySlider = new AnatomyTransparencySlider((ScrollBar)window.findWidget("TransparencySlider"));
            transparencySlider.RecordUndo += transparencySlider_RecordUndo;

            windowStartSize = new IntSize2(window.Width, window.Height);

            centerButton = (Button)window.findWidget("CenterButton");
            centerButton.MouseButtonClick += new MyGUIEvent(centerMenuItem_MouseButtonClick);

            featureButton = (Button)window.findWidget("FeatureButton");
            featureButton.MouseButtonClick += new MyGUIEvent(featureButton_MouseButtonClick);

            Button hideButton = (Button)window.findWidget("HideButton");
            hideButton.MouseButtonClick += new MyGUIEvent(hideButton_MouseButtonClick);

            Button showButton = (Button)window.findWidget("ShowButton");
            showButton.MouseButtonClick += new MyGUIEvent(showButton_MouseButtonClick);

            anatomyFinderButton = (Button)window.findWidget("AnatomyFinder");
            anatomyFinderButton.MouseButtonClick +=anatomyFinderButton_MouseButtonClick;

            commandScroller = (ScrollView)window.findWidget("CommandScroller");
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
                window.Caption = anatomy.AnatomicalName;
                thumbHost = windowManager.getThumbnail(this);
                int width = windowStartSize.Width;
                int captionWidth = (int)captionWidget.getTextSize().Width;
                int totalWidth = captionWidth + captionToBorderDelta;
                if (totalWidth > width)
                {
                    width = totalWidth;
                }
                window.setSize(width, windowStartSize.Height);
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
                                commandUI = new CommandButton(commandScroller, this);
                                addCommandUI(command.UIText, commandUI);
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

                IntSize2 desiredSize = layoutContainer.DesiredSize;
                int scrollHeight = desiredSize.Height;
                if(scrollHeight > MaxScrollerSize) //Height of controls larger than scroll area
                {
                    scrollHeight = MaxScrollerSize;
                }

                commandScroller.setSize(commandScroller.Width, scrollHeight);
                window.setSize(width, window.ClientCoord.top + commandScroller.Bottom + ScaleHelper.Scaled(3));

                desiredSize.Width = commandScroller.Width;
                commandScroller.CanvasSize = new IntSize2(desiredSize.Width, desiredSize.Height); //Note that the width may have changed.

                var viewCoord = commandScroller.ViewCoord;
                if (viewCoord.width < desiredSize.Width)
                {
                    desiredSize.Width = commandScroller.ViewCoord.width - ScaleHelper.Scaled(2);
                }

                layoutContainer.SuppressLayout = false;
                layoutContainer.WorkingSize = desiredSize;
                layoutContainer.layout();

                //special layout for feature and center buttons. They center between the anatomy finder and the thumbnail
                int centerLeft = thumbnailImage.Right;
                int featureRight = anatomyFinderButton.Left - PaddingSize;
                int buttonWidth = (featureRight - centerLeft - PaddingSize) / 2;
                centerButton.setSize(buttonWidth, centerButton.Height);
                int featureLeft = centerButton.Right + PaddingSize;
                featureButton.setCoord(featureLeft, featureButton.Top, featureRight - featureLeft, featureButton.Height);
            }
        }

        public void pinOpen()
        {
            windowManager.alertWindowPinned(this);
            pinButton.Selected = true;
            this.Closed += AnatomyContextWindow_Hidden;
        }

        internal void showAnatomyFinder()
        {
            windowManager.showAnatomyFinderFromContextDialog(window.AbsoluteLeft + window.Width, window.Top);
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
                this.Visible = false;
            }
            else
            {
                pinOpen();
            }
        }

        void featureButton_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.showOnly(this.anatomy);
        }

        void centerMenuItem_MouseButtonClick(Widget source, EventArgs e)
        {
            windowManager.centerAnatomy(this);
        }

        void showButton_MouseButtonClick(Widget source, EventArgs e)
        {
            LayerState undoState = LayerState.CreateAndCapture();
            anatomy.smoothBlend(1.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
            layerController.pushUndoState(undoState);
        }

        void hideButton_MouseButtonClick(Widget source, EventArgs e)
        {
            LayerState undoState = LayerState.CreateAndCapture();
            anatomy.smoothBlend(0.0f, MedicalConfig.CameraTransitionTime, EasingFunction.EaseOutQuadratic);
            layerController.pushUndoState(undoState);
        }

        void anatomyFinderButton_MouseButtonClick(Widget source, EventArgs e)
        {
            showAnatomyFinder();
        }

        void AnatomyContextWindow_Hidden(object sender, EventArgs e)
        {
            windowManager.alertPinnedWindowClosed(this);
            this.Dispose();
        }

        /// <summary>
        /// Add a command ui to the window.
        /// </summary>
        void addCommandUI(String key, CommandUIElement commandUI)
        {
            layoutContainer.addChild(commandUI);
            dynamicWidgets.Add(key, commandUI);
        }

        void transparencySlider_RecordUndo(LayerState undoState)
        {
            layerController.pushUndoState(undoState);
        }
    }
}
