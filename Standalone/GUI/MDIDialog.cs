﻿using System;
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
        private IntSize2 undockedMinSize;
        private IntSize2 undockedMaxSize;
        private static IntSize2 DOCKED_MIN_SIZE = new IntSize2();
        private static IntSize2 DOCKED_MAX_SIZE = new IntSize2(ScaleHelper.Scaled(3000), ScaleHelper.Scaled(3000));
        private String originalLayer;
        protected IntSize2 dockedSize;

        protected bool hidingWithInterface = false;

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

        /// <summary>
        /// Called when the dialog gets focus
        /// </summary>
        public event EventHandler GotFocus;

        /// <summary>
        /// Called when the dialog looses focus
        /// </summary>
        public event EventHandler LostFocus;

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
            dockedSize = new IntSize2(window.Width, window.Height);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            window.RootKeyChangeFocus += new MyGUIEvent(window_RootKeyChangeFocus);
            window.CaptionWidget.MouseButtonPressed += new MyGUIEvent(window_MouseButtonPressed);
            window.CaptionWidget.MouseButtonReleased += new MyGUIEvent(window_MouseButtonReleased);
            window.CaptionWidget.MouseDrag += new MyGUIEvent(window_MouseDrag); //Wont have to override this in mygui 3.2 as it has all multicast delegates

            if (window.MinSize.Width < DOCKED_MIN_SIZE.Width || window.MinSize.Height < DOCKED_MIN_SIZE.Height)
            {
                window.MinSize = DOCKED_MIN_SIZE;
            }

            if (window.MaxSize.Width > DOCKED_MAX_SIZE.Width || window.MaxSize.Height > DOCKED_MAX_SIZE.Height)
            {
                window.MaxSize = DOCKED_MAX_SIZE;
            }

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
            Serialize = true;
        }

        /// <summary>
        /// Dispose method can be overwritten, but be sure to call base.Dispose();
        /// </summary>
        public override void Dispose()
        {
            window.WindowChangedCoord -= window_WindowChangedCoord;
            window.RootKeyChangeFocus -= window_RootKeyChangeFocus;
            window.CaptionWidget.MouseButtonPressed -= window_MouseButtonPressed;
            window.CaptionWidget.MouseButtonReleased -= window_MouseButtonReleased;
            window.CaptionWidget.MouseDrag -= window_MouseDrag; //Wont have to override this in mygui 3.2 as it has all multicast delegates
            base.Dispose();
            LayoutManager.Instance.unloadLayout(dialogLayout);
        }

        /// <summary>
        /// Determine if the given point is contained in this PopupContainer.
        /// </summary>
        /// <param name="x">The x value to check.</param>
        /// <param name="y">The y valid to check.</param>
        /// <returns>True if the point is contained in this container.</returns>
        public bool contains(int x, int y)
        {
            int left = window.AbsoluteLeft;
            int right = left + window.Width;
            int top = window.AbsoluteTop;
            int bottom = top + window.Height;
            return !(x < left || x > right || y < top || y > bottom);
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
                        if (MDIManager != null)
                        {
                            MDIManager.showWindow(this);
                        }
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

        internal void hidingMainInterface()
        {
            hidingWithInterface = true;
            Visible = false;
            hidingWithInterface = false;
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

        public void serialize(ConfigFile configFile)
        {
            if (Serialize)
            {
                ConfigSection section = configFile.createOrRetrieveConfigSection(persistName);
                section.setValue("Location", desiredLocation.ToString());
                section.setValue("DockLocation", CurrentDockLocation.ToString());
                section.setValue("Scale", Scale);
                section.setValue("DockedSize", dockedSize.ToString());

                customSerialize(section, configFile);
            }
        }

        /// <summary>
        /// Save any custom info for this dialog. The passed section is the section for the persistName for this dialog.
        /// </summary>
        /// <param name="section">The section for this dialog's persist name.</param>
        /// <param name="file">The full config file.</param>
        protected virtual void customSerialize(ConfigSection section, ConfigFile file)
        {

        }

        public void deserialize(ConfigFile configFile)
        {
            ConfigSection section = configFile.createOrRetrieveConfigSection(persistName);
            loadDockProperties(section);
            String location = section.getValue("Location", () => desiredLocation.ToString());
            desiredLocation.fromString(location);
            String dockedSizeStr = section.getValue("DockedSize", () => dockedSize.ToString());
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

            customDeserialize(section, configFile);
        }

        /// <summary>
        /// Load any custom info for this dialog. The passed section is the section for the persistName for this dialog.
        /// </summary>
        /// <param name="section">The section for this dialog's persist name.</param>
        /// <param name="file">The full config file.</param>
        protected virtual void customDeserialize(ConfigSection section, ConfigFile file)
        {

        }

        internal override void restoreToMDILayout(MDIWindow mdiWindow, WindowAlignment windowAlignment)
        {
            if (!window.Visible)
            {
                ensureVisible();
                doChangeVisibility(true);
                onShown(EventArgs.Empty);
                if (MDIManager != null)
                {
                    MDIManager.showWindow(this, mdiWindow, windowAlignment);
                }
            }
        }

        protected void loadDockProperties(ConfigSection section)
        {
            try
            {
                CurrentDockLocation = (DockLocation)Enum.Parse(typeof(DockLocation), section.getValue("DockLocation", () => CurrentDockLocation.ToString()));
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

        /// <summary>
        /// True to serialize the location of this dialog (default)
        /// </summary>
        public bool Serialize { get; set; }

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

        public IntVector2 Position
        {
            get
            {
                return new IntVector2(window.Left, window.Top);
            }
            set
            {
                window.setPosition(value.x, value.y);
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

        public int Left
        {
            get
            {
                return window.Left;
            }
        }

        public int Top
        {
            get
            {
                return window.Top;
            }
        }

        public int AbsoluteLeft
        {
            get
            {
                return window.AbsoluteLeft;
            }
        }

        public int AbsoluteTop
        {
            get
            {
                return window.AbsoluteTop;
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

        public bool Enabled
        {
            get
            {
                return window.ClientWidget.Enabled;
            }
            set
            {
                window.ClientWidget.Enabled = value;
            }
        }

        public override IntSize2 ActualSize
        {
            get
            {
                return Size;
            }
            set
            {
                window.setSize(value.Width, value.Height);
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

                switch(newLocation)
                {
                    case DockLocation.Left:
                    case DockLocation.Right:
                        dockedSize.Width = window.Width;
                        break;

                    case DockLocation.Top:
                    case DockLocation.Bottom:
                        dockedSize.Height = window.Height;
                        break;
                }
                Size = dockedSize;

                window.setActionWidgetsEnabled(false);
            }
        }

        protected virtual void onShown(EventArgs args)
        {
            if (Shown != null)
            {
                Shown.Invoke(this, args);
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
                    dockedSize = new IntSize2(lastWidth, lastHeight);
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

        protected internal override bool isControlWidgetAtPosition(int x, int y)
        {
            return false;
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
            window.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, (int)WorkingSize.Height);
            updateDesiredLocation();
        }

        public override IntSize2 DesiredSize
        {
            get 
            {
                return new IntSize2(window.Width, window.Height);
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

        void window_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            if (((RootFocusEventArgs)e).Focus)
            {
                if (GotFocus != null)
                {
                    GotFocus.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (LostFocus != null)
                {
                    LostFocus.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
