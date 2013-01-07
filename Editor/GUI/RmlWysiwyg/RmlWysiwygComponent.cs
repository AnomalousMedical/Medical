using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;
using Medical.GUI.AnomalousMvc;
using System.Xml;
using System.IO;
using Engine.Editing;
using Engine;
using Medical.GUI.RmlWysiwyg.Elements;

namespace Medical.GUI
{
    public class RmlWysiwygComponent : LayoutComponent
    {
        private static ElementStrategyManager elementStrategyManager = new ElementStrategyManager();

        static RmlWysiwygComponent()
        {
            elementStrategyManager.add(new HeadingStrategy("h1"));
            elementStrategyManager.add(new HeadingStrategy("p"));
            elementStrategyManager.add(new HeadingStrategy("a"));
            elementStrategyManager.add(new HeadingStrategy("input"));
            elementStrategyManager.add(new HeadingStrategy("img"));
        }

        public event Action<RmlWysiwygComponent> RmlEdited;

        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;
        private int imageWidth;
        private String documentStart = "<body>";
        private String documentEnd = "</body>";
        private bool disposed = false;
        private MedicalUICallback uiCallback;
        private RmlWysiwygBrowserProvider browserProvider;
        RmlElementEditor currentEditor = null;
        private bool allowEdit = true;
        private SelectedElementManager selectedElementManager;
        private PreviewElement previewElement = new PreviewElement();
        private DraggingElementManager draggingElementManager;
        private bool lastInsertBefore = false;
        private UndoRedoBuffer undoBuffer = new UndoRedoBuffer(50);

        private AnomalousMvcContext context;

        public RmlWysiwygComponent(RmlWysiwygView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.RmlWysiwyg.RmlWysiwygComponent.layout", viewHost)
        {
            this.context = context;
            this.uiCallback = view.UICallback;
            this.browserProvider = view.BrowserProvider;

            rmlImage = (ImageBox)widget;
            rocketWidget = new RocketWidget(rmlImage);
            rmlImage.MouseButtonClick += new MyGUIEvent(rmlImage_MouseButtonClick);
            rmlImage.MouseButtonPressed += rmlImage_MouseButtonPressed;
            rmlImage.MouseButtonReleased += rmlImage_MouseButtonReleased;
            rmlImage.MouseDrag += new MyGUIEvent(rmlImage_MouseDrag);
            rmlImage.MouseWheel += new MyGUIEvent(rmlImage_MouseWheel);
            rmlImage.EventScrollGesture += new MyGUIEvent(rmlImage_EventScrollGesture);
            imageHeight = rmlImage.Height;

            selectedElementManager = new SelectedElementManager(rmlImage.findWidget("SelectionWidget"));
            draggingElementManager = new DraggingElementManager(this);

            if (view.RmlFile != null)
            {
                using (ElementDocument document = rocketWidget.Context.LoadDocument(view.RmlFile))
                {
                    if (document != null)
                    {
                        saveDocumentStartAndEndFile(view.RmlFile);
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
            }

            view._fireComponentCreated(this);
        }

        public override void Dispose()
        {
            draggingElementManager.Dispose();
            previewElement.Dispose();
            disposed = true;
            rocketWidget.Dispose();
            base.Dispose();
        }

        public override void topLevelResized()
        {
            if (widget.Height != imageHeight || widget.Width != imageWidth)
            {
                rocketWidget.resized();
                imageHeight = widget.Height;
                imageWidth = widget.Width;
            }
            base.topLevelResized();
        }

        public void aboutToSaveRml()
        {
            if (currentEditor != null)
            {
                currentEditor.hide();
            }
        }

        public void reloadDocument(String documentName)
        {
            RocketGuiManager.clearAllCaches();
            rocketWidget.Context.UnloadAllDocuments();
            selectedElementManager.clearSelectedAndHighlightedElement();

            if (documentName != null)
            {
                using (ElementDocument document = rocketWidget.Context.LoadDocument(documentName))
                {
                    if (document != null)
                    {
                        saveDocumentStartAndEndFile(documentName);
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
            }
        }

        public void insertRml(String rml, IntVector2 position)
        {
            if (!widget.contains(position.x, position.y))
            {
                selectedElementManager.clearSelectedAndHighlightedElement();
                previewElement.hidePreviewElement();
                rmlModified();
            }
            else if(allowEdit)
            {
                String undoRml = UnformattedRml;
                insertRmlIntoDocument(rml);
                updateUndoStatus(undoRml);

                //Clear selection for drag and drop
                selectedElementManager.SelectedElement = null;
                selectedElementManager.HighlightElement = null;
            }
        }

        public void insertRml(String rml)
        {
            if (allowEdit)
            {
                String undoRml = UnformattedRml;
                insertRmlIntoDocument(rml);
                updateUndoStatus(undoRml);
            }
        }

        public bool contains(IntVector2 position)
        {
            return widget.contains(position.x, position.y);
        }

        internal void insertRml(String rml, String undoRml)
        {
            if (allowEdit)
            {
                insertRmlIntoDocument(rml);
                updateUndoStatus(undoRml);
            }
        }

        private void insertRmlIntoDocument(String rml)
        {
            if (allowEdit)
            {
                previewElement.hidePreviewElement();

                ElementDocument document = rocketWidget.Context.GetDocument(0);
                using (Element div = document.CreateElement("temp"))
                {
                    Element topContentElement = TopContentElement;
                    if (selectedElementManager.HasSelection && selectedElementManager.SelectedElement != topContentElement)
                    {
                        Element insertInto = selectedElementManager.SelectedElement.ParentNode;
                        if (insertInto != null)
                        {
                            insertInto.Insert(div, selectedElementManager.SelectedElement, lastInsertBefore);
                        }
                    }
                    else
                    {
                        topContentElement.AppendChild(div);
                    }

                    div.InnerRml = rml;

                    Element parent = div.ParentNode;
                    Element child;
                    Element next = div.FirstChild;

                    while (next != null)
                    {
                        child = next;
                        next = child.NextSibling;
                        parent.InsertBefore(child, div);
                    }
                    parent.RemoveChild(div);

                    rmlModified();
                }
            }
        }

        public void setPreviewElement(IntVector2 position, String innerRmlHint, String previewElementTagType)
        {
            if (widget.contains(position.x, position.y))
            {
                position.x -= widget.AbsoluteLeft;
                position.y -= widget.AbsoluteTop;

                Element toSelect = rocketWidget.Context.FindElementAtPoint(position);
                Element selectedElement = selectedElementManager.SelectedElement;

                bool insertBefore = lastInsertBefore;
                bool toSelectIsNotPreview = true;
                if (toSelect != null)
                {
                    toSelectIsNotPreview = !previewElement.isPreviewOrAncestor(toSelect);
                    if (toSelectIsNotPreview)
                    {
                        insertBefore = insertBeforeOrAfter(toSelect, position);
                    }
                }
                if (toSelectIsNotPreview && (toSelect != selectedElement || insertBefore != lastInsertBefore))
                {
                    if (toSelect != null)
                    {
                        Element topContentElement = TopContentElement;
                        if (toSelect != topContentElement)
                        {
                            selectedElementManager.SelectedElement = toSelect;
                            previewElement.hidePreviewElement();
                            ElementDocument document = rocketWidget.Context.GetDocument(0);
                            previewElement.showPreviewElement(document, innerRmlHint, toSelect.ParentNode, toSelect, previewElementTagType, insertBefore);
                            selectedElementManager.HighlightElement = previewElement.HighlightPreviewElement;
                        }
                        else
                        {
                            selectedElementManager.SelectedElement = null;
                            previewElement.hidePreviewElement();
                            ElementDocument document = rocketWidget.Context.GetDocument(0);
                            previewElement.showPreviewElement(document, innerRmlHint, toSelect, null, previewElementTagType, insertBefore);
                            selectedElementManager.HighlightElement = previewElement.HighlightPreviewElement;
                        }
                    }
                    else
                    {
                        selectedElementManager.clearSelectedAndHighlightedElement();
                        previewElement.hidePreviewElement();
                    }

                    lastInsertBefore = insertBefore;

                    rmlModified();
                }
            }
            else
            {
                clearPreviewElement();
            }
        }

        public void clearPreviewElement()
        {
            selectedElementManager.clearSelectedAndHighlightedElement();
            previewElement.hidePreviewElement();
            rmlModified();
        }

        public void undo()
        {
            if (currentEditor != null)
            {
                currentEditor.ApplyChanges = false;
                currentEditor.hide();
            }
            undoBuffer.undo();
        }

        public void redo()
        {
            if (currentEditor != null)
            {
                currentEditor.ApplyChanges = false;
                currentEditor.hide();
            }
            undoBuffer.execute();
        }

        public String CurrentRml
        {
            get
            {
                String rml = UnformattedRml;
                if (rml != null)
                {
                    rml = formatRml(rml);
                }
                return rml;
            }
        }

        public String UnformattedRml
        {
            get
            {
                Element topContentElemnt = TopContentElement;
                if (topContentElemnt != null)
                {
                    String contentRml = topContentElemnt.InnerRml;
                    StringBuilder sb = new StringBuilder(documentStart.Length + contentRml.Length + documentEnd.Length);
                    sb.Append(documentStart);
                    sb.Append(contentRml);
                    sb.Append(documentEnd);
                    return sb.ToString();
                }
                return null;
            }
        }

        public Element TopContentElement
        {
            get
            {
                Element document = rocketWidget.Context.GetDocument(0);
                if (document != null)
                {
                    Variant templateName = document.GetAttribute("template");
                    if (templateName == null)
                    {
                        return document;
                    }
                    else
                    {
                        Template template = TemplateCache.GetTemplate(templateName.StringValue);
                        if (template != null)
                        {
                            Element contentDocument = document.GetElementById(template.Content);
                            if (contentDocument != null)
                            {
                                return contentDocument;
                            }
                            else
                            {
                                return document;
                            }
                        }
                        else
                        {
                            return document;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        internal IntVector2 localCoord(IntVector2 position)
        {
            position.x -= widget.AbsoluteLeft;
            position.y -= widget.AbsoluteTop;
            return position;
        }

        private String formatRml(String inputRml)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(inputRml);
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineChars = "\n";
                settings.NewLineHandling = NewLineHandling.Replace;
                using (XmlWriter xmlWriter = XmlWriter.Create(sb, settings))
                {
                    xmlDoc.Save(xmlWriter);
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.show("There was an error parsing your RML back into a nice format.\nYou will want to correct it as it means your XML is malformed.\nThe error was:\n" + ex.Message, "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                return inputRml;
            }
        }

        private void saveDocumentStartAndEndFile(String file)
        {
            using (StreamReader sr = new StreamReader(context.ResourceProvider.openFile(file)))
            {
                saveDocumentStartAndEnd(sr.ReadToEnd());
            }
        }

        private void saveDocumentStartAndEnd(String inputRml)
        {
            int bodyStart = inputRml.IndexOf("<body", StringComparison.InvariantCultureIgnoreCase);
            if (bodyStart > -1)
            {
                bodyStart = inputRml.IndexOf(">", bodyStart, StringComparison.InvariantCultureIgnoreCase) + 1;
                if (bodyStart > -1)
                {
                    documentStart = inputRml.Substring(0, bodyStart);

                    int closeBodyStart = inputRml.IndexOf("</body", bodyStart, StringComparison.InvariantCultureIgnoreCase);
                    if (closeBodyStart > -1)
                    {
                        documentEnd = inputRml.Substring(closeBodyStart);
                        allowEdit = true;
                    }
                    else
                    {
                        allowEdit = false;
                        MessageBox.show("Cannot find an closing body tag.\nPlease ensure that your source has a closing </body> element.\nYou will not be able to edit elements in the document until this is fixed.", "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                        bodyStart = 0;
                    }
                }
                else
                {
                    allowEdit = false;
                    MessageBox.show("Cannot find an opening body tag.\nPlease ensure that your source has an opening <body> element.\nYou will not be able to edit elements in the document until this is fixed.", "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                    bodyStart = 0;
                }
            }
            else
            {
                allowEdit = false;
                MessageBox.show("Cannot find an opening body tag.\nPlease ensure that your source has an opening <body> element.\nYou will not be able to edit elements in the document until this is fixed.", "RML Format Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                bodyStart = 0;
            }
        }

        void rmlImage_MouseButtonClick(Widget source, EventArgs e)
        {
            if (!allowEdit)
            {
                //Break if they cannot edit
                return;
            }

            Element element = rocketWidget.Context.GetFocusElement();
            if (element != null)
            {
                showRmlElementEditor(element, elementStrategyManager[element]);
            }
        }

        void rmlImage_EventScrollGesture(Widget source, EventArgs e)
        {
            selectedElementManager.updateHighlightPosition();
        }

        void rmlImage_MouseWheel(Widget source, EventArgs e)
        {
            selectedElementManager.updateHighlightPosition();
        }

        void rmlImage_MouseDrag(Widget source, EventArgs e)
        {
            selectedElementManager.updateHighlightPosition();
            draggingElementManager.dragging(((MouseEventArgs)e).Position);
        }

        void rmlImage_MouseButtonPressed(Widget source, EventArgs e)
        {
            IntVector2 mousePosition = ((MouseEventArgs)e).Position;
            IntVector2 localPosition = localCoord(mousePosition);
            Element element = rocketWidget.Context.FindElementAtPoint(localPosition);
            if (elementStrategyManager[element].AllowDragAndDrop)
            {
                draggingElementManager.dragStart(mousePosition, element);
            }
        }

        void rmlImage_MouseButtonReleased(Widget source, EventArgs e)
        {
            draggingElementManager.dragEnded(((MouseEventArgs)e).Position);
        }

        private void showRmlElementEditor(Element element, ElementStrategy strategy)
        {
            RmlElementEditor editor = strategy.openEditor(element, uiCallback, browserProvider, rocketWidget.AbsoluteLeft, rocketWidget.AbsoluteTop);
            if (editor == null)
            {
                //The editor was null, which means editing is not supported so just clear the selection.
                selectedElementManager.clearSelectedAndHighlightedElement();
                return; //Return here to prevent more execution
            }

            //Everything is good so setup.
            editor.Hiding += (src, evt) =>
            {
                if (editor.ApplyChanges && !disposed)
                {
                    String undoRml = UnformattedRml;
                    String text = editor.Text;
                    if (isTextElement(element) && String.IsNullOrEmpty(text)) //THIS IS WHERE WE CAN EDIT AUTO DELETEING (or make it configurable somehow)
                    {
                        Element parent = element.ParentNode;
                        if (parent != null)
                        {
                            parent.RemoveChild(element);
                            if (element == selectedElementManager.SelectedElement)
                            {
                                selectedElementManager.clearSelectedAndHighlightedElement();
                            }
                        }
                    }
                    else
                    {
                        element.InnerRml = editor.Text;
                    }
                    rmlModified();
                    updateUndoStatus(undoRml, true);
                }
                if (currentEditor == editor)
                {
                    currentEditor = null;
                }
            };
            editor.MoveElementUp += upElement =>
            {
                Element previousSibling = upElement.PreviousSibling;
                if (previousSibling != null)
                {
                    Element parent = upElement.ParentNode;
                    if (parent != null)
                    {
                        String undoRml = UnformattedRml;
                        upElement.addReference();
                        parent.RemoveChild(upElement);
                        parent.InsertBefore(upElement, previousSibling);
                        upElement.removeReference();
                        rmlModified();
                        updateUndoStatus(undoRml);
                    }
                }
            };
            editor.MoveElementDown += downElement =>
            {
                Element parent = downElement.ParentNode;
                if (parent != null)
                {
                    Element nextSibling = downElement.NextSibling;
                    if (nextSibling != null)
                    {
                        String undoRml = UnformattedRml;

                        downElement.addReference();
                        parent.RemoveChild(downElement);
                        nextSibling = nextSibling.NextSibling;
                        if (nextSibling != null)
                        {
                            parent.InsertBefore(downElement, nextSibling);
                        }
                        else
                        {
                            parent.AppendChild(downElement);
                        }
                        downElement.removeReference();

                        rmlModified();
                        updateUndoStatus(undoRml);
                    }
                }
            };
            editor.DeleteElement += deleteElement =>
            {
                Element parent = deleteElement.ParentNode;
                if (parent != null)
                {
                    String undoRml = UnformattedRml;

                    Element nextSelectionElement = deleteElement.NextSibling;
                    if (nextSelectionElement == null)
                    {
                        nextSelectionElement = deleteElement.PreviousSibling;
                    }

                    parent.RemoveChild(deleteElement);
                    rmlModified();
                    updateUndoStatus(undoRml);

                    if (nextSelectionElement != null)
                    {
                        showRmlElementEditor(nextSelectionElement, elementStrategyManager[nextSelectionElement]);
                    }
                    else
                    {
                        selectedElementManager.clearSelectedAndHighlightedElement();
                    }
                }
            };
            currentEditor = editor;
            selectedElementManager.SelectedElement = element;
            selectedElementManager.HighlightElement = element;
        }

        private bool isTextElement(Element element)
        {
            if (element.TagName == "img")
            {
                return false;
            }
            return true;
        }

        private void rmlModified()
        {
            if (RmlEdited != null)
            {
                RmlEdited.Invoke(this);
            }
            selectedElementManager.updateHighlightPosition();
            rocketWidget.renderOnNextFrame();
        }

        private bool insertBeforeOrAfter(Element element, IntVector2 position)
        {
            return position.y - element.AbsoluteTop < element.OffsetHeight / 2;
        }

        private void setDocumentRml(String rml)
        {
            RocketGuiManager.clearAllCaches();
            rocketWidget.Context.UnloadAllDocuments();
            selectedElementManager.clearSelectedAndHighlightedElement();

            if (rml != null)
            {
                using (ElementDocument document = rocketWidget.Context.LoadDocumentFromMemory(rml))
                {
                    if (document != null)
                    {
                        saveDocumentStartAndEnd(rml);
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                        rmlModified();
                    }
                }
            }
        }

        internal void updateUndoStatus(String oldMarkup, bool check = false)
        {
            //This is a hacky way to check for changes (optionally) it should not be needed when the popup editor is overhauled.
            //You can remove check and keep only the line in the if statement when you no longer need the check.
            String currentMarkup = UnformattedRml;
            if (!check || currentMarkup != oldMarkup)
            {
                undoBuffer.pushAndSkip(new TwoWayDelegateCommand<String, String>(setDocumentRml, currentMarkup, setDocumentRml, oldMarkup));
            }
        }
    }
}
