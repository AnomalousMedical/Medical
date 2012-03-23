using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using Logging;

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
        private Size2 undockedMinSize;
        private Size2 undockedMaxSize;
        private static Size2 DOCKED_MIN_SIZE = new Size2();
        private static Size2 DOCKED_MAX_SIZE = new Size2(3000, 3000);
        private String originalLayer;
        protected Size2 dockedSize;

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

        /// <summary>
        /// Called when the dialog is resized.
        /// </summary>
        public event EventHandler Resized;

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
            :base(DockLocation.Floating)
        {
            this.persistName = persistName;
            dialogLayout = LayoutManager.Instance.loadLayout(layoutFile);
            window = dialogLayout.getWidget(0) as Window;
            originalLayer = window.getLayerName();
            window.Visible = false;
            window.WindowButtonPressed += new MyGUIEvent(window_WindowButtonPressed);
            SmoothShow = true;
            IgnorePositionChanges = false;
            desiredLocation = new Rect(window.Left, window.Top, window.Width, window.Height);
            dockedSize = new Size2(window.Width, window.Height);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            window.CaptionWidget.MouseButtonPressed += new MyGUIEvent(window_MouseButtonPressed);
            window.CaptionWidget.MouseButtonReleased += new MyGUIEvent(window_MouseButtonReleased);
            window.CaptionWidget.MouseDrag += new MyGUIEvent(window_MouseDrag); //Wont have to override this in mygui 3.2 as it has all multicast delegates

            String currentDockLocProp = window.getUserString("CurrentDockLocation");
            if (!String.IsNullOrEmpty(currentDockLocProp))
            {
                try
                {
                    CurrentDockLocation = (DockLocation)Enum.Parse(typeof(DockLocation), currentDockLocProp, true);
                }
                catch (Exception)
                {
                    Log.Warning("Could not parse CurrentDockLocation for {0}, using default instead.", persistName);
                }
            }

            String allowedDockLocProp = window.getUserString("AllowedDockLocations");
            if (!String.IsNullOrEmpty(allowedDockLocProp))
            {
                AllowedDockLocations = DockLocation.None;
                String[] allowedLocationsSplit = allowedDockLocProp.Split('|');
                foreach (String location in allowedLocationsSplit)
                {
                    try
                    {
                        AllowedDockLocations |= (DockLocation)Enum.Parse(typeof(DockLocation), location.Trim(), true);
                    }
                    catch (Exception)
                    {
                        Log.Warning("Could not parse AllowedDockLocations {0} for window {1}. Ignoring location.", location, persistName);
                    }
                }
            }

            updateUndockedMinMaxSize();
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
            if (CurrentDockLocation == DockLocation.Floating)
            {
                //Adjust the position if needed
                int left = (int)desiredLocation.Left;
                int top = (int)desiredLocation.Top;
                int right = (int)(left + desiredLocation.Width);
                int bottom = (int)(top + desiredLocation.Height);

                int guiWidth = RenderManager.Instance.ViewWidth;
                int guiHeight = RenderManager.Instance.ViewHeight;

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
        }

        public virtual void serialize(ConfigFile configFile)
        {
            ConfigSection section = configFile.createOrRetrieveConfigSection(persistName);
            section.setValue("Location", desiredLocation.ToString());
            section.setValue("DockLocation", CurrentDockLocation.ToString());
            section.setValue("Scale", Scale);
            section.setValue("DockedSize", dockedSize.ToString());
        }

        public virtual void deserialize(ConfigFile configFile)
        {
            ConfigSection section = configFile.createOrRetrieveConfigSection(persistName);
            loadDockProperties(section);
            String location = section.getValue("Location", desiredLocation.ToString());
            desiredLocation.fromString(location);
            String dockedSizeStr = section.getValue("DockedSize", dockedSize.ToString());
            dockedSize.fromString(dockedSizeStr);

            if (CurrentDockLocation == DockLocation.Floating)
            {
                if (desiredLocation.Left < 0)
                {
                    desiredLocation.Left = 0;
                }
                if (desiredLocation.Top < 0)
                {
                    desiredLocation.Top = 0;
                }

                window.setCoord((int)desiredLocation.Left, (int)desiredLocation.Top, (int)desiredLocation.Width, (int)desiredLocation.Height);
                updateDesiredLocation();
            }
            else
            {
                Size = (IntSize2)dockedSize;
            }
        }

        protected void loadDockProperties(ConfigSection section)
        {
            try
            {
                CurrentDockLocation = (DockLocation)Enum.Parse(typeof(DockLocation), section.getValue("DockLocation", CurrentDockLocation.ToString()));
            }
            catch (Exception)
            {
                Log.Warning("Could not load DockLocation for {0}, using default instead.", persistName);
            }
            Scale = section.getValue("Scale", Scale);
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

        protected override void onDockLocationChanged(DockLocation oldLocation, DockLocation newLocation)
        {
            if (newLocation == DockLocation.Floating)
            {
                float normalizedMouseWidthOffset = (float)captionMouseOffset.x / window.CaptionWidget.Width;
                window.MinSize = undockedMinSize;
                window.MaxSize = undockedMaxSize;
                window.setSize((int)desiredLocation.Width, (int)desiredLocation.Height);
                captionMouseOffset.x = (int)(normalizedMouseWidthOffset * window.CaptionWidget.Width);
                IgnorePositionChanges = false;

                window.setActionWidgetsEnabled(true);
            }
            else
            {
                updateUndockedMinMaxSize();
                window.MinSize = DOCKED_MIN_SIZE;
                window.MaxSize = DOCKED_MAX_SIZE;
                Size = (IntSize2)dockedSize;

                window.setActionWidgetsEnabled(false);
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
            try
            {
                if (MDIManager != null)
                {
                    MDIManager.closeWindow(this);
                }
            }
            catch (MDIException e)
            {
                Log.Warning(e.Message);
            }
        }

        protected void updateUndockedMinMaxSize()
        {
            undockedMinSize = window.MinSize;
            undockedMaxSize = window.MaxSize;
        }

        void window_WindowButtonPressed(Widget source, EventArgs e)
        {
            Visible = false;
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            updateDesiredLocation();
            if (CurrentDockLocation != DockLocation.Floating && CurrentDockLocation != DockLocation.None)
            {
                invalidate();
            }
        }

        private void updateDesiredLocation()
        {
            if (CurrentDockLocation == DockLocation.Floating && !IgnorePositionChanges)
            {
                desiredLocation.Left = window.Left;
                desiredLocation.Top = window.Top;
                desiredLocation.Width = window.Width;
                desiredLocation.Height = window.Height;
            }
            if (window.Width != lastWidth || window.Height != lastHeight)
            {
                lastWidth = window.Width;
                lastHeight = window.Height;
                if (Resized != null)
                {
                    Resized.Invoke(this, EventArgs.Empty);
                }
                if (CurrentDockLocation != DockLocation.Floating && CurrentDockLocation != DockLocation.None)
                {
                    dockedSize = new Size2(lastWidth, lastHeight);
                }
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
            int newMouseX = (int)(mousePosition.x - captionMouseOffset.x);
            int newMouseY = (int)(mousePosition.y - captionMouseOffset.y);

            int maxHiddenWidth = -window.Width / 2;

            if (newMouseX < maxHiddenWidth)
            {
                newMouseX = maxHiddenWidth;
            }
            if (newMouseX > RenderManager.Instance.ViewWidth + maxHiddenWidth)
            {
                newMouseX = RenderManager.Instance.ViewWidth + maxHiddenWidth;
            }
            if (newMouseY < 0)
            {
                newMouseY = 0;
            }
            if (newMouseY > RenderManager.Instance.ViewHeight - 25)
            {
                newMouseY = RenderManager.Instance.ViewHeight - 25;
            }
            window.setPosition(newMouseX, newMouseY);
            fireMouseDrag((MouseEventArgs)e);
        }

        void window_MouseButtonReleased(Widget source, EventArgs e)
        {
            fireMouseDragFinished((MouseEventArgs)e);
            updateDesiredLocation();
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
