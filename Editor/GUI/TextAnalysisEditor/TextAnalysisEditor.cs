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
        private AnalysisEditorComponent selectedComponent;
        private SaveableClipboard clipboard;

        private ActionBlockEditor actionBlockEditor;
        private ScrollView scrollView;
        private int windowWidth;
        private MenuItem refreshVariables;
        private MenuItem inject;
        private MenuItem saveItem;
        private MenuItem openItem;
        private MenuItem newItem;
        private MenuItem removeItem;
        private Edit name;

        private VariableChosenCallback variableChosenCallback;

        public TextAnalysisEditor(BrowserWindow browser, TimelinePropertiesController timelinePropertiesController, SaveableClipboard clipboard)
            : base("Medical.GUI.TextAnalysisEditor.TextAnalysisEditor.layout")
        {
            this.clipboard = clipboard;
            this.browserWindow = browser;
            this.timelinePropertiesController = timelinePropertiesController;

            this.Resized += new EventHandler(TextAnalysisEditor_Resized);

            scrollView = (ScrollView)window.findWidget("ScrollView");
            windowWidth = scrollView.ClientCoord.width;

            name = (Edit)window.findWidget("Name");

            actionBlockEditor = new ActionBlockEditor("Document", this);
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

            MenuItem edit = menuBar.addItem("Edit", MenuItemType.Popup);
            MenuCtrl editItem = menuBar.createItemPopupMenuChild(edit);
            editItem.ItemAccept += new MyGUIEvent(editItem_ItemAccept);
            editItem.addItem("Cut", MenuItemType.Normal, "Cut");
            editItem.addItem("Copy", MenuItemType.Normal, "Copy");
            editItem.addItem("Paste", MenuItemType.Normal, "Paste");
            editItem.addItem("Insert Paste", MenuItemType.Normal, "InsertPaste");

            MenuItem addItem = menuBar.addItem("Add", MenuItemType.Popup);
            MenuCtrl addMenuItem = menuBar.createItemPopupMenuChild(addItem);
            addMenuItem.ItemAccept += new MyGUIEvent(addMenuItem_ItemAccept);
            addMenuItem.addItem("Start Paragraph", MenuItemType.Normal, "StartParagraph");
            addMenuItem.addItem("End Paragraph", MenuItemType.Normal, "EndParagraph");
            addMenuItem.addItem("Write", MenuItemType.Normal, "Write");
            addMenuItem.addItem("Test", MenuItemType.Normal, "Test");

            MenuItem insert = menuBar.addItem("Insert", MenuItemType.Popup);
            MenuCtrl insertItem = menuBar.createItemPopupMenuChild(insert);
            insertItem.ItemAccept += new MyGUIEvent(insertMenuItem_ItemAccept);
            insertItem.addItem("Start Paragraph", MenuItemType.Normal, "StartParagraph");
            insertItem.addItem("End Paragraph", MenuItemType.Normal, "EndParagraph");
            insertItem.addItem("Write", MenuItemType.Normal, "Write");
            insertItem.addItem("Test", MenuItemType.Normal, "Test");

            removeItem = menuBar.addItem("Remove", MenuItemType.Normal);
            removeItem.MouseButtonClick += new MyGUIEvent(removeItem_MouseButtonClick);

            SelectedComponent = actionBlockEditor;
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

        public ActionBlockEditor OwnerActionBlockEditor
        {
            get
            {
                return actionBlockEditor;
            }
        }

        public void requestSelected(AnalysisEditorComponent component)
        {
            SelectedComponent = component;
        }

        public AnalysisEditorComponent SelectedComponent
        {
            get
            {
                return selectedComponent;
            }
            set
            {
                if (selectedComponent != null)
                {
                    selectedComponent.Selected = false;
                }
                selectedComponent = value;
                if (selectedComponent != null)
                {
                    selectedComponent.Selected = true;
                    removeItem.Enabled = selectedComponent.Removeable;
                }
                else
                {
                    removeItem.Enabled = false;
                }
            }
        }

        public Widget Widget
        {
            get
            {
                return scrollView;
            }
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

        void addMenuItem_ItemAccept(Widget source, EventArgs e)
        {
            if (SelectedComponent != null)
            {
                ActionBlockEditor actionBlock = SelectedComponent.OwnerActionBlockEditor;
                MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
                switch (mcae.Item.ItemId)
                {
                    case "StartParagraph":
                        actionBlock.addChildEditor(new StartParagraphEditor(actionBlock));
                        break;
                    case "EndParagraph":
                        actionBlock.addChildEditor(new EndParagraphEditor(actionBlock));
                        break;
                    case "Write":
                        actionBlock.addChildEditor(new WriteEditor(actionBlock));
                        break;
                    case "Test":
                        actionBlock.addChildEditor(new TestEditor(actionBlock));
                        break;
                }
            }
        }

        void insertMenuItem_ItemAccept(Widget source, EventArgs e)
        {
            if (SelectedComponent != null)
            {
                ActionBlockEditor actionBlock = SelectedComponent.OwnerActionBlockEditor;
                MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
                switch (mcae.Item.ItemId)
                {
                    case "StartParagraph":
                        actionBlock.insertChildEditor(new StartParagraphEditor(actionBlock), SelectedComponent);
                        break;
                    case "EndParagraph":
                        actionBlock.insertChildEditor(new EndParagraphEditor(actionBlock), SelectedComponent);
                        break;
                    case "Write":
                        actionBlock.insertChildEditor(new WriteEditor(actionBlock), SelectedComponent);
                        break;
                    case "Test":
                        actionBlock.insertChildEditor(new TestEditor(actionBlock), SelectedComponent);
                        break;
                }
            }
        }

        void editItem_ItemAccept(Widget source, EventArgs e)
        {
            if (SelectedComponent != null)
            {
                ActionBlockEditor actionBlock = SelectedComponent.OwnerActionBlockEditor;
                MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
                AnalysisAction action;
                switch (mcae.Item.ItemId)
                {
                    case "Cut":
                        clipboard.copyToSourceObject(SelectedComponent.createAction());
                        if (SelectedComponent != null)
                        {
                            if (SelectedComponent.Removeable)
                            {
                                AnalysisEditorComponent component = SelectedComponent;
                                SelectedComponent = null;
                                component.OwnerActionBlockEditor.removeChildEditor(component);
                                component.Dispose();
                            }
                            else
                            {
                                ActionBlockEditor block = SelectedComponent as ActionBlockEditor;
                                if (block != null)
                                {
                                    block.empty();
                                }
                            }
                        }
                        break;
                    case "Copy":
                        clipboard.copyToSourceObject(SelectedComponent.createAction());
                        break;
                    case "Paste":
                        if (clipboard.HasSourceObject && (action = clipboard.createCopy<AnalysisAction>()) != null)
                        {
                            actionBlock.addFromAction(action);
                        }
                        break;
                    case "InsertPaste":
                        if (clipboard.HasSourceObject && (action = clipboard.createCopy<AnalysisAction>()) != null)
                        {
                            actionBlock.insertFromAction(action, SelectedComponent);
                        }
                        break;
                }
            }
        }

        void removeItem_MouseButtonClick(Widget source, EventArgs e)
        {
            if (SelectedComponent != null && SelectedComponent.Removeable)
            {
                AnalysisEditorComponent component = SelectedComponent;
                SelectedComponent = null;
                component.OwnerActionBlockEditor.removeChildEditor(component);
                component.Dispose();
            }
        }

        private void newAnalysis()
        {
            SelectedComponent = actionBlockEditor;
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
                            SelectedComponent = actionBlockEditor;
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
