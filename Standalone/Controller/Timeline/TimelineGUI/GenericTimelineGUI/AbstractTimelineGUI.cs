using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using Medical.Controller;

namespace Medical
{
    public abstract class AbstractTimelineGUI : MyGUITimelineGUI
    {
        private ShowTimelineGUIAction showTimelineAction;

        public AbstractTimelineGUI(String layout)
            : base(layout)
        {

        }

        public override void initialize(ShowTimelineGUIAction showTimelineAction)
        {
            this.showTimelineAction = showTimelineAction;
        }

        public override sealed void show(GUIManager guiManager)
        {
            AbstractTimelineGUIManager.Instance.requestOpen(this);
            onShown();
        }

        protected virtual void onShown()
        {

        }

        public override sealed void forceClose(GUIManager guiManager)
        {
            close();
        }

        public Stream openFile(String filename)
        {
            return showTimelineAction.openFile(filename);
        }

        public void applyLayers(LayerState layers)
        {
            if (layers != null)
            {
                layers.apply();
            }
        }

        public void applyCameraPosition(CameraPosition cameraPosition)
        {
            SceneViewWindow window = showTimelineAction.SceneViewController.ActiveWindow;
            if (window != null)
            {
                window.setPosition(cameraPosition);
            }
        }

        public void playExampleTimeline(String timeline)
        {
            showTimelineAction.playTimeline(timeline, false);
        }

        public void closeAndPlayTimeline(String timeline)
        {
            close();
            showTimelineAction.playTimeline(timeline);
        }

        public void closeAndReturnToMainGUI()
        {
            close();
            showTimelineAction.stopTimelines();
        }

        public void showNavigationBar()
        {
            AbstractTimelineGUIManager.Instance.showNavigationBar();
        }

        public void hideNavigationBar()
        {
            AbstractTimelineGUIManager.Instance.hideNavigationBar();
        }

        public void addToNavigationBar(String timeline, String text, String imageKey)
        {
            AbstractTimelineGUIManager.Instance.addToNavigationBar(timeline, text, imageKey);
        }

        public void clearNavigationBar()
        {
            AbstractTimelineGUIManager.Instance.clearNavigationBar();
        }

        protected virtual void closing()
        {

        }

        internal void _alertNavigationBarTimelineChange(String timeline)
        {
            navigationBarChangedTimelines(timeline);
        }

        /// <summary>
        /// This is called when the navigation bar is changing timelines.
        /// </summary>
        protected virtual void navigationBarChangedTimelines(String timeline)
        {

        }

        /// <summary>
        /// Used only by AbstractTimelineGUIManager. Do NOT touch.
        /// </summary>
        internal bool _RequestClosed { get; set; }

        /// <summary>
        /// Callback when animation is complete. The lifecycle for these objects
        /// is to new/delete when needed so this will Dispose this gui.
        /// 
        /// Used only by AbstractTimelineGUIManager. Do NOT touch.
        /// </summary>
        /// <param name="oldChild">Required parameter for the callback.</param>
        internal void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
        }

        internal String TimelineFile
        {
            get
            {
                return showTimelineAction.Timeline.SourceFile;
            }
        }

        /// <summary>
        /// Helper to close the window, does not signal timeline playback stop.
        /// </summary>
        private void close()
        {
            closing();
            widget.Enabled = false;
            AbstractTimelineGUIManager.Instance.requestClose(this);
        }
    }
}
