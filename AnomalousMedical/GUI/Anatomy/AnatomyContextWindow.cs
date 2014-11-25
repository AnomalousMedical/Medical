﻿using System;
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
        private StretchLayoutContainer layoutContainer = new StretchLayoutContainer(StretchLayoutContainer.LayoutType.Vertical, 5, new IntVector2(0, 0));
        private LayerController layerController;

        private IntSize2 windowStartSize;

        private ImageBox thumbnailImage;
        private TextBox anatomyName;
        private AnatomyTransparencySlider transparencySlider;
        int captionToBorderDelta = 0;
        private IntVector2 mouseOffset;

        private Button pinButton;

        private AnatomyContextWindowLiveThumbHost thumbHost;
        private ScrollView commandScroller;

        public AnatomyContextWindow(AnatomyContextWindowManager windowManager, LayerController layerController)
            :base("Medical.GUI.Anatomy.AnatomyContextWindow.layout")
        {
            this.windowManager = windowManager;
            this.layerController = layerController;
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
            transparencySlider.RecordUndo += transparencySlider_RecordUndo;

            windowStartSize = new IntSize2(widget.Width, widget.Height);

            Button centerButton = (Button)widget.findWidget("CenterButton");
            centerButton.MouseButtonClick += new MyGUIEvent(centerMenuItem_MouseButtonClick);

            Button featureButton = (Button)widget.findWidget("FeatureButton");
            featureButton.MouseButtonClick += new MyGUIEvent(featureButton_MouseButtonClick);

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
                                commandUI = new CommandButton(commandScroller);
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
                widget.setSize(width, commandScroller.Bottom + ScaleHelper.Scaled(3));

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

        void transparencySlider_RecordUndo(LayerState undoState)
        {
            layerController.pushUndoState(undoState);
        }
    }
}
