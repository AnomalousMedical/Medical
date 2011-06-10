using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Engine;
using Medical.Controller;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class MDIDialog : MDIWindow, IDisposable
    {
        private Layout dialogLayout;
        protected Window window;
        private Rect desiredLocation;
        private String persistName;
        private int lastWidth = -1;
        private int lastHeight = -1;
        private IntVector2 captionMouseOffset;

        /// <summary>
        /// Called after the dialog opens.
        /// </summary>
        public event EventHandler Shown;

        /// <summary>
        /// Called when the dialog is closing, but is still open.
        /// </summary>
        public event EventHandler<DialogCancelEventArgs> Closing;

        /// <summary>
        /// Called when the dialog is closed.
        /// </summary>
        public event EventHandler Closed;

        public MDIDialog(String layoutFile)
            : this(layoutFile, "")
        {
            Type t = GetType();
            persistName = String.Format("{0}.{1}", t.Namespace, t.Name);
        }

        /// <summary>
        /// Constructor. Takes the layout file to load.
        /// </summary>
        /// <param name="layoutFile">The layout file of the dialog.</param>
        public MDIDialog(String layoutFile, String persistName)
        {
            this.persistName = persistName;
            dialogLayout = LayoutManager.Instance.loadLayout(layoutFile);
            window = dialogLayout.getWidget(0) as Window;
            window.Visible = false;
            window.WindowButtonPressed += new MyGUIEvent(window_WindowButtonPressed);
            SmoothShow = true;
            IgnorePositionChanges = false;
            desiredLocation = new Rect(window.Left, window.Top, window.Width, window.Height);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            window.CaptionWidget.MouseButtonPressed += new MyGUIEvent(window_MouseButtonPressed);
            window.CaptionWidget.MouseButtonReleased += new MyGUIEvent(window_MouseButtonReleased);
            window.CaptionWidget.MouseDrag += new MyGUIEvent(window_MouseDrag); //Wont have to override this in mygui 3.2 as it has all multicast delegates
        }

        /// <summary>
        /// Dispose method can be overwritten, but be sure to call base.Dispose();
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            LayoutManager.Instance.unloadLayout(dialogLayout);
        }

        /// <summary>
        /// Open the dialog with the given modal setting.
        /// </summary>
        /// <param name="modal">True to be modal. False for non modal.</param>
        public void open(bool modal)
        {
            if (Visible == false)
            {
                Visible = true;
            }
        }

        /// <summary>
        /// Close the dialog.
        /// </summary>
        public void close()
        {
            if (Visible == true)
            {
                Visible = false;
            }
        }

        /// <summary>
        /// True if the window is shown, false otherwise. Setting to true will
        /// show the window with the current properties.
        /// </summary>
        public override bool Visible
        {
            get
            {
                return window.Visible;
            }
            set
            {
                if (window.Visible != value)
                {
                    if (value)
                    {
                        ensureVisible();
                        doChangeVisibility(value);
                        onShown(EventArgs.Empty);
                    }
                    else
                    {
                        DialogCancelEventArgs cancelEvent = new DialogCancelEventArgs();
                        onClosing(cancelEvent);
                        if (!cancelEvent.Cancel)
                        {
                            doChangeVisibility(value);
                            onClosed(EventArgs.Empty);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Have the window compute its position to ensure it is visible in the given screen area.
        /// </summary>
        public void ensureVisible()
        {
            //Adjust the position if needed
            int left = (int)desiredLocation.Left;
            int top = (int)desiredLocation.Top;
            int right = (int)(left + desiredLocation.Width);
            int bottom = (int)(top + desiredLocation.Height);

            int guiWidth = Gui.Instance.getViewWidth();
            int guiHeight = Gui.Instance.getViewHeight();

            if (right > guiWidth)
            {
                left -= right - guiWidth;
                if (left < 0)
                {
                    left = 0;
                }
            }

            if (bottom > guiHeight)
            {
                top -= bottom - guiHeight;
                if (top < 0)
                {
                    top = 0;
                }
            }

            IgnorePositionChanges = true;
            window.setPosition(left, top);
            IgnorePositionChanges = false;
        }

        public virtual void serialize(ConfigFile configFile)
        {
            ConfigSection section = configFile.createOrRetrieveConfigSection(persistName);
            section.setValue("Location", desiredLocation.ToString());
        }

        public virtual void deserialize(ConfigFile configFile)
        {
            ConfigSection section = configFile.createOrRetrieveConfigSection(persistName);
            String location = section.getValue("Location", desiredLocation.ToString());
            desiredLocation.fromString(location);
            window.setCoord((int)desiredLocation.Left, (int)desiredLocation.Top, (int)desiredLocation.Width, (int)desiredLocation.Height);
        }

        public void center()
        {
            desiredLocation.Left = Gui.Instance.getViewWidth() / 2 - window.Width / 2;
            desiredLocation.Top = Gui.Instance.getViewHeight() / 2 - window.Height / 2;
            window.setCoord((int)desiredLocation.Left, (int)desiredLocation.Top, (int)desiredLocation.Width, (int)desiredLocation.Height);
        }

        /// <summary>
        /// Acutally change the window visibility, called from Visible.set
        /// </summary>
        /// <param name="value"></param>
        private void doChangeVisibility(bool value)
        {
            if (value)
            {
                LayerManager.Instance.upLayerItem(window);
            }
            if (SmoothShow)
            {
                window.setVisibleSmooth(value);
            }
            else
            {
                window.Visible = value;
            }
        }

        /// <summary>
        /// True to show the window smoothly.
        /// </summary>
        public bool SmoothShow { get; set; }

        /// <summary>
        /// The MDILayoutManager to add this class to.
        /// </summary>
        public MDILayoutManager MDIManager { get; set; }

        public Rect DesiredLocation
        {
            get
            {
                return desiredLocation;
            }
            set
            {
                desiredLocation = value;
            }
        }

        public bool IgnorePositionChanges { get; set; }

        public Vector2 Position
        {
            get
            {
                return new Vector2(window.Left, window.Top);
            }
            set
            {
                window.setPosition((int)value.x, (int)value.y);
                updateDesiredLocation();
            }
        }

        public IntSize2 Size
        {
            get
            {
                return new IntSize2(window.Width, window.Height);
            }
            set
            {
                window.setSize(value.Width, value.Height);
                updateDesiredLocation();
            }
        }

        public int Width
        {
            get
            {
                return window.Width;
            }
        }

        public int Height
        {
            get
            {
                return window.Height;
            }
        }

        protected String PersistName
        {
            get
            {
                return persistName;
            }
        }

        protected virtual void onShown(EventArgs args)
        {
            if (Shown != null)
            {
                Shown.Invoke(this, args);
            }
            if (MDIManager != null)
            {
                MDIManager.showWindow(this);
            }
        }

        protected virtual void onClosing(DialogCancelEventArgs args)
        {
            if (Closing != null)
            {
                Closing.Invoke(this, args);
            }
        }

        protected virtual void onClosed(EventArgs args)
        {
            if (Closed != null)
            {
                Closed.Invoke(this, args);
            }
            if (MDIManager != null)
            {
                MDIManager.closeWindow(this);
            }
        }

        void window_WindowButtonPressed(Widget source, EventArgs e)
        {
            Visible = false;
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            updateDesiredLocation();
            if (Visible && window.Width != lastWidth || window.Height != lastHeight)
            {
                lastWidth = window.Width;
                lastHeight = window.Height;
                invalidate();
            }
        }

        private void updateDesiredLocation()
        {
            if (!IgnorePositionChanges)
            {
                desiredLocation.Left = window.Left;
                desiredLocation.Top = window.Top;
                desiredLocation.Width = window.Width;
                desiredLocation.Height = window.Height;
            }
        }

        protected override void activeStatusChanged(bool active)
        {
            
        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            return this;
        }

        public override void bringToFront()
        {
            LayerManager.Instance.upLayerItem(window);
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            Position = Location;
            Size = (IntSize2)WorkingSize;
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2(window.Width, window.Height);
            }
        }

        void window_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            IntVector2 mousePosition = new IntVector2(me.Position.x, me.Position.y);
            window.setPosition((int)(mousePosition.x - captionMouseOffset.x), (int)(mousePosition.y - captionMouseOffset.y));
            fireMouseDrag((MouseEventArgs)e);
        }

        void window_MouseButtonReleased(Widget source, EventArgs e)
        {
            fireMouseDragFinished((MouseEventArgs)e);
        }

        void window_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            captionMouseOffset = new IntVector2(me.Position.x - window.CaptionWidget.AbsoluteLeft, me.Position.y - window.CaptionWidget.AbsoluteTop);
            layoutManager.ActiveWindow = this;
            fireMouseDragStarted(me);
        }
    }
}
