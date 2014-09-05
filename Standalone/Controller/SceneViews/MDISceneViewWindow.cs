using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using MyGUIPlugin;
using Engine.Renderer;
using Engine.ObjectManagement;
using Engine;
using OgrePlugin;

namespace Medical.Controller
{
    public class MDISceneViewWindow : SceneViewWindow
    {
        enum Events
        {
            PrimarySelect,
            SecondarySelect,
        }

        private static MessageEvent primaryActionSelect;
        private static MessageEvent secondaryActionSelect;

        static MDISceneViewWindow()
        {
            primaryActionSelect = new MessageEvent(Events.PrimarySelect, EventLayers.AfterGui);
            primaryActionSelect.addButton(MouseButtonCode.MB_BUTTON0);
            DefaultEvents.registerDefaultEvent(primaryActionSelect);

            secondaryActionSelect = new MessageEvent(Events.SecondarySelect, EventLayers.AfterGui);
            secondaryActionSelect.addButton(MouseButtonCode.MB_BUTTON1);
            DefaultEvents.registerDefaultEvent(secondaryActionSelect);
        }

        private MDIDocumentWindow mdiWindow;

        public MDISceneViewWindow(RendererWindow rendererWindow, SceneViewController controller, CameraMover cameraMover, String name, BackgroundScene background, int zIndexStart)
            :base(controller, cameraMover, name, background, zIndexStart)
        {
            this.createBackground(((OgreWindow)PluginManager.Instance.RendererPlugin.PrimaryWindow).OgreRenderTarget, false);

            //MDI Window
            mdiWindow = new MDIDocumentWindow(Name);
            mdiWindow.AllowedDockLocations = DockLocation.Center;
            mdiWindow.SuppressLayout = true;
            mdiWindow.Content = this;
            mdiWindow.SuppressLayout = false;
            mdiWindow.Caption = Name;
            mdiWindow.Closed += new EventHandler(mdiWindow_Closed);
            mdiWindow.ActiveStatusChanged += new EventHandler(mdiWindow_ActiveStatusChanged);

            primaryActionSelect.FirstFrameDownEvent += selectEvent;
            secondaryActionSelect.FirstFrameDownEvent += selectEvent;

            this.RendererWindow = rendererWindow;
        }

        public override void Dispose()
        {
            primaryActionSelect.FirstFrameDownEvent -= selectEvent;
            secondaryActionSelect.FirstFrameDownEvent -= selectEvent;
            base.Dispose();
            mdiWindow.Dispose();
        }

        /// <summary>
        /// This can add a single child as the parent of the actual scene view and child of the mdi window.
        /// You can use this to display other info on the SceneViewWindow like a product license or logo.
        /// 
        /// Be sure to dispose or otherwise handle anything you attach this way, this only deals with a layout
        /// container.
        /// </summary>
        /// <param name="layoutContainer"></param>
        public void addChildContainer(SingleChildLayoutContainer layoutContainer)
        {
            var oldContent = mdiWindow.Content;
            mdiWindow.Content = layoutContainer;
            layoutContainer.Child = oldContent;
        }

        public override void close()
        {
            mdiWindow.close();
        }

        public bool AllowClose
        {
            get
            {
                return mdiWindow.AllowClose;
            }
            set
            {
                mdiWindow.AllowClose = value;
            }
        }

        /// <summary>
        /// Get the MDIWindow for this window. Do not touch unless you are SceneViewController.
        /// </summary>
        /// <returns>The MDIWindow for this window.</returns>
        internal MDIWindow _getMDIWindow()
        {
            return mdiWindow;
        }

        protected internal override void _madeActiveSceneView()
        {
            TransparencyController.ActiveTransparencyState = CurrentTransparencyState;
        }

        void mdiWindow_Closed(object sender, EventArgs e)
        {
            controller.destroyWindow(this);
        }

        void mdiWindow_ActiveStatusChanged(object sender, EventArgs e)
        {
            if (mdiWindow.Active)
            {
                TransparencyController.ActiveTransparencyState = CurrentTransparencyState;
            }
        }

        void selectEvent(EventLayer eventLayer)
        {
            if (eventLayer.EventProcessingAllowed)
            {
                var absPos = eventLayer.Mouse.AbsolutePosition;
                if (containsPoint(absPos.x, absPos.y))
                {
                    mdiWindow.Active = true;
                }
            }
        }
    }
}
