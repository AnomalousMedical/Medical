using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Medical.GUI;
using MyGUIPlugin;

namespace ExamExample.GUI
{
    /// <summary>
    /// An example TimelineGUI.
    /// </summary>
    class ExampleGUI : MyGUITimelineGUI
    {
        private ShowTimelineGUIAction showTimelineAction;
        private GUIManager guiManager;
        private ExampleGUIData guiData;

        public ExampleGUI()
            :base("ExamExample.GUI.ExampleGUI.layout")
        {
            //Find the button from the widget in the layout.
            Button closeButton = (Button)widget.findWidget("Close");
            //Subscribe to the mouse clicked event for the close button.
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            Button playSomethingButton = (Button)widget.findWidget("PlaySomething");
            playSomethingButton.MouseButtonClick += new MyGUIEvent(playSomethingButton_MouseButtonClick);
        }

        /// <summary>
        /// The initialize method will be called when the action is firing. It
        /// gives the GUI access to the showTimelineAction, which is what it
        /// will use to control the program.
        /// </summary>
        /// <param name="showTimelineAction">The action that created this gui.</param>
        public override void initialize(ShowTimelineGUIAction showTimelineAction)
        {
            this.showTimelineAction = showTimelineAction;
            guiData = (ExampleGUIData)showTimelineAction.GUIData;
        }

        /// <summary>
        /// This method will be called when the GUI is to be shown on the screen.
        /// </summary>
        /// <param name="guiManager"></param>
        public override void show(GUIManager guiManager)
        {
            //Here the left panel is set to be the layoutContainer for this GUI, which puts this gui on the screen.
            guiManager.changeLeftPanel(layoutContainer);
            this.guiManager = guiManager;
        }

        /// <summary>
        /// This method will be called by the timeline if it needs to cancel the
        /// gui for some reason. Normal program flow will not call this method,
        /// but it should correctly hide the GUI and destroy its instance.
        /// </summary>
        /// <param name="guiManager"></param>
        public override void hide(GUIManager guiManager)
        {
            close();
        }

        /// <summary>
        /// Called when the mouse is clicked on the close button.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            close();
            showTimelineAction.stopTimelines();
        }

        /// <summary>
        /// Called when the Play Something button is clicked.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void playSomethingButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //Here the showTimelineAction is being used to play another timeline. 
            //The second argument determines if you want to allow the timeline to 
            //put a stop to all timeline playback or not. If this is true the main interface will
            //show back up and the timeline is considered to no longer be playing. If it is false
            //control will remain with this gui and nothing will show back up from the main UI.
            //Pass false to keep running this gui.
            //Pass true to allow the main interface to come back. You should also close this gui if you do that.
            showTimelineAction.playTimeline(guiData.SecondTimeline, false);
        }

        /// <summary>
        /// Helper method to close the window.
        /// </summary>
        private void close()
        {
            //Here we change the left panel back to null and have an anoymous delegate that calls Dispose when the animation is completed.
            guiManager.changeLeftPanel(null, delegate()
            {
                //The lifecycle is to create a new gui in the prototype so it must be disposed here when we are done with it.
                Dispose();
            });
        }
    }
}
