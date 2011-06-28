using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    public abstract class AbstractTimelineGUI : MyGUITimelineGUI
    {
        private ShowTimelineGUIAction showTimelineAction;

        public AbstractTimelineGUI(String layout)
            : base(layout)
        {

        }

        public override void Dispose()
        {
            Logging.Log.Debug("AbstractTimelineGUI {0} destroyed.", GetType().Name);
            base.Dispose();
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

        public override sealed void hide(GUIManager guiManager)
        {
            close();
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

        /// <summary>
        /// Helper to close the window, does not signal timeline playback stop.
        /// </summary>
        private void close()
        {
            AbstractTimelineGUIManager.Instance.requestClose(this);
        }
    }
}
