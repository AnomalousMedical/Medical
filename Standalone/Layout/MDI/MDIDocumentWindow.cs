using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using SoundPlugin;
using Medical.GUI;
using Engine;

namespace Medical.Controller
{
    public class MDIDocumentWindow : MDIWindow
    {
        public event EventHandler Closed;
        public event EventHandler ActiveStatusChanged;
        
        private Layout guiLayout;
        private Widget mainWidget;
        private LayoutContainer content;
        private Button captionButton;
        private Button closeButton;
        private Widget volumePanel;
        private VScroll volumeSlider;
        private CheckButton volumeSliderButton;

        /// <summary>
        /// Constructor. Can load custom layout files for the header. These files may contain two buttons for custom behavior:
        /// CaptionButton - The button that the text will go on. Must be a button.
        /// CloseButton - The button that is used to close the window. Must be a button.
        /// If these widgets are missing the associated functions will not be avaliable.
        /// </summary>
        /// <param name="layoutFile"></param>
        /// <param name="caption"></param>
        public MDIDocumentWindow(String caption)
            :base(DockLocation.Center)
        {
            guiLayout = LayoutManager.Instance.loadLayout("Medical.Layout.MDI.MDIDocumentWindow.layout");
            mainWidget = guiLayout.getWidget(0);

            volumePanel = guiLayout.getWidget(1);
            volumePanel.Visible = false;

            SoundConfig.MasterVolumeChanged += SoundConfig_MasterVolumeChanged;
            volumeSlider = volumePanel.findWidget("VolumeSlider") as VScroll;
            volumeSlider.ScrollChangePosition += new MyGUIEvent(volumeSlider_ScrollChangePosition);
            SoundConfig_MasterVolumeChanged(null, null);

            volumeSliderButton = new CheckButton(volumePanel.findWidget("VolumeSliderButton") as Button);
            volumeSliderButton.CheckedChanged += new MyGUIEvent(volumeSliderButton_CheckedChanged);
            volumeSliderButton_CheckedChanged(null, null);

            captionButton = mainWidget.findWidget("CaptionButton") as Button;
            if (captionButton != null)
            {
                captionButton.MouseButtonPressed += new MyGUIEvent(captionButton_MouseButtonClick);
                captionButton.MouseDrag += new MyGUIEvent(captionButton_MouseDrag);
                captionButton.MouseButtonReleased += new MyGUIEvent(captionButton_MouseButtonReleased);
                captionButton.Pointer = MainWindow.HAND;
            }

            closeButton = mainWidget.findWidget("CloseButton") as Button;
            if (closeButton != null)
            {
                closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
            }
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        public override void Dispose()
        {
            LayoutManager.Instance.unloadLayout(guiLayout);
            SoundConfig.MasterVolumeChanged -= SoundConfig_MasterVolumeChanged;
        }

        public Widget findChildWidget(String name)
        {
            return mainWidget.findWidget(name);
        }

        public void close()
        {
            layoutManager.closeWindow(this);
            fireClosed();
        }

        /// <summary>
        /// LayoutContainer method.
        /// </summary>
        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(mainWidget);
            if (content != null)
            {
                content.bringToFront();
            }
        }

        /// <summary>
        /// LayoutContainer method.
        /// </summary>
        public override void setAlpha(float alpha)
        {
            mainWidget.Alpha = alpha;
            if (content != null)
            {
                content.setAlpha(alpha);
            }
        }

        /// <summary>
        /// LayoutContainer method.
        /// </summary>
        public override void layout()
        {
            mainWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, mainWidget.Height);
            volumePanel.setCoord((int)(Location.x + WorkingSize.Width - volumePanel.Width), (int)(Location.y + WorkingSize.Height) - volumePanel.Height, volumePanel.Width, volumePanel.Height);
            if (content != null)
            {
                content.WorkingSize = new Size2(WorkingSize.Width, WorkingSize.Height);
                content.Location = new Vector2(Location.x, Location.y);
                content.layout();
            }
        }

        protected override void activeStatusChanged(bool active)
        {
            captionButton.StateCheck = Active;
            if (ActiveStatusChanged != null)
            {
                ActiveStatusChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            return this;
        }

        /// <summary>
        /// LayoutContainer property.
        /// </summary>
        public override Size2 DesiredSize
        {
            get 
            {
                Size2 desiredSize = new Size2(mainWidget.Width, mainWidget.Height);
                if (content != null)
                {
                    Size2 contentSize = content.DesiredSize;
                    desiredSize.Width = contentSize.Width;
                    desiredSize.Height += contentSize.Height;
                }
                return desiredSize;
            }
        }

        /// <summary>
        /// LayoutContainer property.
        /// </summary>
        public override bool Visible
        {
            get
            {
                return mainWidget.Visible;
            }
            set
            {
                mainWidget.Visible = value;
                if (content != null)
                {
                    content.Visible = value;
                }
            }
        }

        public bool AllowClose
        {
            get
            {
                return closeButton.Visible;
            }
            set
            {
                closeButton.Visible = value;
                captionButton.Visible = value;
                volumePanel.Visible = !value;
            }
        }

        /// <summary>
        /// The content LayoutContainer for this window.
        /// </summary>
        public LayoutContainer Content
        {
            get
            {
                return content;
            }
            set
            {
                if (content != null)
                {
                    content._setParent(null);
                }
                content = value;
                content.SuppressLayout = true;
                content._setParent(this);
                content.setAlpha(mainWidget.Alpha);
                content.Visible = mainWidget.Visible;
                content.SuppressLayout = false;
                invalidate();
            }
        }

        /// <summary>
        /// The caption for this window. Will only work if the layout contained
        /// a CaptionButton. Otherwise it is always null.
        /// </summary>
        public String Caption
        {
            get
            {
                if (captionButton != null)
                {
                    return captionButton.Caption;
                }
                else return null;
            }
            set
            {
                if (captionButton != null)
                {
                    captionButton.Caption = value;
                    captionButton.setSize((int)captionButton.getTextSize().Width + 50, captionButton.Height);
                }
            }
        }

        protected void fireClosed()
        {
            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Close button callback.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            close();
        }

        void captionButton_MouseButtonClick(Widget source, EventArgs e)
        {
            layoutManager.ActiveWindow = this;
            MouseEventArgs me = (MouseEventArgs)e;
            fireMouseDragStarted(me);
        }

        void captionButton_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            fireMouseDrag(me);
        }

        void captionButton_MouseButtonReleased(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            fireMouseDragFinished(me);
        }

        bool allowVolumeUpdates = true;

        void volumeSlider_ScrollChangePosition(Widget source, EventArgs e)
        {
            allowVolumeUpdates = false;
            SoundConfig.MasterVolume = (volumeSlider.ScrollRange - volumeSlider.ScrollPosition) / (float)volumeSlider.ScrollRange;
            allowVolumeUpdates = true;
        }

        void SoundConfig_MasterVolumeChanged(object sender, EventArgs e)
        {
            if (allowVolumeUpdates)
            {
                uint scrollPos = (uint)(volumeSlider.ScrollRange - volumeSlider.ScrollRange * SoundConfig.MasterVolume);
                if (scrollPos >= volumeSlider.ScrollRange)
                {
                    --scrollPos;
                }
                volumeSlider.ScrollPosition = scrollPos;
            }
        }

        void volumeSliderButton_CheckedChanged(Widget source, EventArgs e)
        {
            volumeSlider.Visible = volumeSliderButton.Checked;
        }
    }

}
