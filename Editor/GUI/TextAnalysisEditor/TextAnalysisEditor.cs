using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Editing;

namespace Medical.GUI
{
    class TextAnalysisEditor : MDIDialog, AnalysisEditorComponentParent
    {
        private BrowserWindow browserWindow;
        private Browser browser = new Browser("Variables");
        private TimelinePropertiesController timelinePropertiesController;

        private ActionBlockEditor actionBlockEditor;
        private ScrollView scrollView;
        private int windowWidth;
        private MenuItem refreshVariables;

        private Action<string> variableChosenCallback;

        public TextAnalysisEditor(BrowserWindow browser, TimelinePropertiesController timelinePropertiesController)
            : base("Medical.GUI.TextAnalysisEditor.TextAnalysisEditor.layout")
        {
            this.browserWindow = browser;
            this.timelinePropertiesController = timelinePropertiesController;

            windowWidth = window.Width;
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            scrollView = (ScrollView)window.findWidget("ScrollView");

            actionBlockEditor = new ActionBlockEditor(this);
            actionBlockEditor.Removeable = false;
            layout((int)scrollView.ClientCoord.width);

            MenuBar menuBar = (MenuBar)window.findWidget("Menu");
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuCtrl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            refreshVariables = fileMenu.addItem("Refresh Variables");
        }

        public override void Dispose()
        {
            actionBlockEditor.Dispose();
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            if (window.Width != windowWidth)
            {
                windowWidth = window.Width;
                layout((int)scrollView.ClientCoord.width);
            }
        }

        private void layout(int newWidth)
        {
            actionBlockEditor.layout(0, 0, newWidth);
            scrollView.CanvasSize = new Engine.Size2(newWidth, actionBlockEditor.Height);
        }

        public void requestLayout()
        {
            layout((int)scrollView.ClientCoord.width);
        }

        public AnalysisEditorComponentParent Parent
        {
            get
            {
                return null;
            }
        }

        public Widget Widget
        {
            get
            {
                return scrollView;
            }
        }

        public void removeChildComponent(AnalysisEditorComponent child)
        {
            throw new NotImplementedException();
        }

        public void openVariableBrowser(Action<string> variableChosenCallback)
        {
            browserWindow.setBrowser(browser);
            this.variableChosenCallback = variableChosenCallback;
            browserWindow.ItemSelected += browserWindow_ItemSelected;
            browserWindow.Canceled += browserWindow_Canceled;
            browserWindow.open(true);
        }

        void browserWindow_Canceled(object sender, EventArgs e)
        {
            unsubBrowserWindow();
        }

        void browserWindow_ItemSelected(object sender, EventArgs e)
        {
            variableChosenCallback.Invoke(browserWindow.SelectedValue.ToString());
            unsubBrowserWindow();
        }

        private void unsubBrowserWindow()
        {
            variableChosenCallback = null;
            browserWindow.ItemSelected -= browserWindow_ItemSelected;
            browserWindow.Canceled -= browserWindow_Canceled;
        }

        void fileMenu_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            if (mcae.Item == refreshVariables)
            {
                refreshVariableBrowser();
            }
        }

        private void refreshVariableBrowser()
        {
            Timeline activeTimeline = timelinePropertiesController.CurrentTimeline;
            if (activeTimeline != null)
            {
                DataDrivenTimelineGUIData dataDrivenGUI = DataDrivenTimelineGUIData.FindDataInTimeline(activeTimeline);
                if (dataDrivenGUI != null)
                {
                    browser = new Browser("Variables");
                }
            }
        }
    }
}
