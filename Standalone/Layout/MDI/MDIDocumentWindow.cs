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
            if (content != null)
            {
                content.WorkingSize = new IntSize2(WorkingSize.Width, WorkingSize.Height);
                content.Location = new IntVector2(Location.x, Location.y);
                content.layout();
            }
        }

        internal override void restoreToMDILayout(MDIWindow mDIWindow, WindowAlignment windowAlignment)
        {

        }

        protected override void activeStatusChanged(bool active)
        {
            captionButton.Selected = Active;
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
        public override IntSize2 DesiredSize
        {
            get 
            {
                IntSize2 desiredSize = new IntSize2(mainWidget.Width, mainWidget.Height);
                if (content != null)
                {
                    IntSize2 contentSize = content.DesiredSize;
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
    }

}
