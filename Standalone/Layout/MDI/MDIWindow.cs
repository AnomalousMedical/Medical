using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.Controller
{
    public class MDIWindow : LayoutContainer, IDisposable
    {
        public event EventHandler Closed;

        private Layout guiLayout;
        private Widget mainWidget;
        private LayoutContainer content;
        private Button captionButton;

        public MDIWindow(String layoutFile, String caption)
        {
            guiLayout = LayoutManager.Instance.loadLayout(layoutFile);
            mainWidget = guiLayout.getWidget(0);

            captionButton = mainWidget.findWidget("CaptionButton") as Button;

            Button closeButton = mainWidget.findWidget("CloseButton") as Button;
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(guiLayout);
        }

        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(mainWidget);
            if (content != null)
            {
                content.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            mainWidget.Alpha = alpha;
            if (content != null)
            {
                content.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            mainWidget.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, mainWidget.getHeight());
            if (content != null)
            {
                content.WorkingSize = new Size2(WorkingSize.Width, WorkingSize.Height - mainWidget.getHeight());
                content.Location = new Vector2(Location.x, Location.y + mainWidget.getHeight());
                content.layout();
            }
        }

        public override Size2 DesiredSize
        {
            get 
            {
                Size2 desiredSize = new Size2(mainWidget.getWidth(), mainWidget.getHeight());
                if (content != null)
                {
                    Size2 contentSize = content.DesiredSize;
                    desiredSize.Width = contentSize.Width;
                    desiredSize.Height += contentSize.Height;
                }
                return desiredSize;
            }
        }

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

        public String Caption
        {
            get
            {
                return captionButton.Caption;
            }
            set
            {
                captionButton.Caption = value;
                captionButton.setSize((int)FontManager.Instance.measureStringWidth(captionButton.Font, value) + 50, captionButton.getHeight());
            }
        }

        /// <summary>
        /// The container this window is currently inside of.
        /// Do not touch unless you are MDILayoutManager.
        /// </summary>
        internal MDILayoutContainer _CurrentContainer { get; set; }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
