using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Editing;
using Medical.Controller.Exam;
using System.Xml;
using Engine.Saving.XMLSaver;

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
        private MenuItem inject;
        private MenuItem saveItem;
        private MenuItem openItem;
        private MenuItem newItem;
        private Edit name;

        private VariableChosenCallback variableChosenCallback;

        public TextAnalysisEditor(BrowserWindow browser, TimelinePropertiesController timelinePropertiesController)
            : base("Medical.GUI.TextAnalysisEditor.TextAnalysisEditor.layout")
        {
            this.browserWindow = browser;
            this.timelinePropertiesController = timelinePropertiesController;

            this.Resized += new EventHandler(TextAnalysisEditor_Resized);

            scrollView = (ScrollView)window.findWidget("ScrollView");
            windowWidth = scrollView.ClientCoord.width;

            name = (Edit)window.findWidget("Name");

            actionBlockEditor = new ActionBlockEditor(this);
            actionBlockEditor.Removeable = false;
            layoutEditor();

            MenuBar menuBar = (MenuBar)window.findWidget("Menu");
            MenuItem fileMenuItem = menuBar.addItem("File", MenuItemType.Popup);
            MenuCtrl fileMenu = menuBar.createItemPopupMenuChild(fileMenuItem);
            fileMenu.ItemAccept += new MyGUIEvent(fileMenu_ItemAccept);
            newItem = fileMenu.addItem("New");
            saveItem = fileMenu.addItem("Save");
            openItem = fileMenu.addItem("Open");
            refreshVariables = fileMenu.addItem("Refresh Variables");
            inject = fileMenu.addItem("Inject");
        }

        public override void Dispose()
        {
            actionBlockEditor.Dispose();
            base.Dispose();
        }

        void TextAnalysisEditor_Resized(object sender, EventArgs e)
        {
            if (scrollView.ClientCoord.width != windowWidth)
            {
                windowWidth = scrollView.ClientCoord.width;
                layoutEditor();
            }
        }

        private void layoutEditor()
        {
            int newWidth = scrollView.ClientCoord.width;
            actionBlockEditor.layout(0, 0, newWidth);
            scrollView.CanvasSize = new Engine.Size2(newWidth, actionBlockEditor.Height);
        }

        public void requestLayout()
        {
            layoutEditor();
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

        public void openVariableBrowser(VariableChosenCallback variableChosenCallback)
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
            variableChosenCallback.Invoke(browserWindow.SelectedValue as DataFieldInfo);
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
            else if (mcae.Item == inject)
            {
                try
                {
                    DataDrivenExamController.Instance.TEMP_InjectedExamAnalyzer = createAnalyzer();
                }
                catch (Exception ex)
                {
                    MessageBox.show("Could not inject exam.\n" + ex.Message, "Injection Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
            }
            else if (mcae.Item == saveItem)
            {
                save();
            }
            else if (mcae.Item == openItem)
            {
                open();
            }
            else if (mcae.Item == newItem)
            {
                newAnalysis();
            }
        }

        private void newAnalysis()
        {
            name.Caption = "";
            actionBlockEditor.empty();
        }

        private void open()
        {
            try
            {
                using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Choose an analysis file to open.", MedicalConfig.UserDocRoot, "", "Analysis File (*.anl)|*.anl", false))
                {
                    if (openDialog.showModal() == NativeDialogResult.OK)
                    {
                        using (XmlTextReader xmlReader = new XmlTextReader(openDialog.Path))
                        {
                            XmlSaver xmlSaver = new XmlSaver();
                            DataDrivenExamTextAnalyzer analyzer = (DataDrivenExamTextAnalyzer)xmlSaver.restoreObject(xmlReader);
                            actionBlockEditor.createFromAnalyzer(analyzer.Analysis);
                            name.Caption = analyzer.Name;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.show("Could not open analysis.\n" + e.Message, "Load Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        private void save()
        {
            try
            {
                using (FileSaveDialog saveDialog = new FileSaveDialog(MainWindow.Instance, "Choose location to save your analysis.", MedicalConfig.UserDocRoot, name.Caption + ".anl", "Analysis File (*.anl)|*.anl"))
                {
                    if (saveDialog.showModal() == NativeDialogResult.OK)
                    {
                        DataDrivenExamTextAnalyzer analyzer = createAnalyzer();
                        using (XmlTextWriter xmlWriter = new XmlTextWriter(saveDialog.Path, Encoding.Default))
                        {
                            xmlWriter.Formatting = Formatting.Indented;
                            XmlSaver xmlSaver = new XmlSaver();
                            xmlSaver.saveObject(analyzer, xmlWriter);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.show("Could not save analysis.\n" + e.Message, "Save Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        private DataDrivenExamTextAnalyzer createAnalyzer()
        {
            ActionBlock actionBlock = (ActionBlock)actionBlockEditor.createAction();
            if (actionBlock != null)
            {
                return new DataDrivenExamTextAnalyzer(name.Caption, actionBlock);
            }
            throw new AnalysisCompilationError("Analysis cannot be blank.");
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
                    BrowserBuilderFactory browserFactory = new BrowserBuilderFactory(browser, timelinePropertiesController.EditorTimelineController);
                    dataDrivenGUI.createControls(browserFactory);
                }
            }
        }
    }
}
