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
        public const String DefaultImage = "/Medical.Resources.ImagePlaceholder.png";

        private static ElementStrategyManager elementStrategyManager = new ElementStrategyManager();

        static RmlWysiwygComponent()
        {
            //Headers
            elementStrategyManager.add(new TextElementStrategy("h1"));
            elementStrategyManager.add(new TextElementStrategy("h2"));
            elementStrategyManager.add(new TextElementStrategy("h3"));
            elementStrategyManager.add(new TextElementStrategy("h4"));
            elementStrategyManager.add(new TextElementStrategy("h5"));
            elementStrategyManager.add(new TextElementStrategy("h6"));
            
            //Text
            elementStrategyManager.add(new TextElementStrategy("p"));
            elementStrategyManager.add(new TextElementStrategy("a"));
            elementStrategyManager.add(new ImageStrategy("img"));
            elementStrategyManager.add(new DivStrategy("x-separator"));
            
            //Controls
            elementStrategyManager.add(new InputStrategy("input"));
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
        private RmlElementEditor currentEditor = null;
        private bool allowEdit = true;
        private SelectedElementManager selectedElementManager;
        private PreviewElement previewElement = new PreviewElement();
        private DraggingElementManager draggingElementManager;
        private bool lastInsertBefore = false;
        private UndoRedoBuffer undoBuffer;
        private String documentName;

        private AnomalousMvcContext context;

        public RmlWysiwygComponent(RmlWysiwygView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.RmlWysiwyg.RmlWysiwygComponent.layout", viewHost)
        {
            this.context = context;
            this.uiCallback = view.UICallback;
            this.browserProvider = view.BrowserProvider;
            this.undoBuffer = view.UndoBuffer;

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

            documentName = view.RmlFile;
            loadDocumentFile(documentName, false);

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

        public void reloadDocument()
        {
            loadDocumentFile(documentName, true);
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
                previewElement.hidePreviewElement();
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
                previewElement.hidePreviewElement();
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

        public void cancelAndHideEditor()
        {
            if (currentEditor != null)
            {
                currentEditor.cancelAndHide();
            }
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

        internal void deleteElement(Element element)
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
                draggingElementManager.dragStart(mousePosition, element, elementStrategyManager[element].PreviewIconName);
            }
        }

        void rmlImage_MouseButtonReleased(Widget source, EventArgs e)
        {
            draggingElementManager.dragEnded(((MouseEventArgs)e).Position);
        }

        private void showRmlElementEditor(Element element, ElementStrategy strategy)
        {
            cancelAndHideEditor();
            RmlElementEditor editor = strategy.openEditor(element, uiCallback, browserProvider, widget.AbsoluteLeft + widget.Width, (int)element.AbsoluteTop + rocketWidget.AbsoluteTop);
            if (editor == null)
            {
                //The editor was null, which means editing is not supported so just clear the selection.
                selectedElementManager.clearSelectedAndHighlightedElement();
                return; //Return here to prevent more execution
            }

            editor.UndoRml = UnformattedRml;
            //Everything is good so setup.
            editor.Hiding += (src, arg) =>
            {
                if (!disposed && editor.deleteIfNeeded(this))
                {
                    rmlModified();
                    updateUndoStatus(editor.UndoRml, true);
                    editor.UndoRml = UnformattedRml;
                }
            };
            editor.Hidden += (src, arg) =>
            {
                if (currentEditor == editor)
                {
                    currentEditor = null;
                }
            };
            editor.ChangesMade += (applyElement) =>
            {
                if (!disposed && editor.applyChanges(this))
                {
                    rmlModified();
                    updateUndoStatus(editor.UndoRml, true);
                    editor.UndoRml = UnformattedRml;
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
                        upElement.addReference();
                        parent.RemoveChild(upElement);
                        parent.InsertBefore(upElement, previousSibling);
                        upElement.removeReference();
                        rmlModified();
                        updateUndoStatus(editor.UndoRml);
                        editor.UndoRml = UnformattedRml;
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
                        updateUndoStatus(editor.UndoRml);
                        editor.UndoRml = UnformattedRml;
                    }
                }
            };
            editor.DeleteElement += deleteElement =>
            {
                Element parent = deleteElement.ParentNode;
                if (parent != null)
                {
                    Element nextSelectionElement = deleteElement.NextSibling;
                    if (nextSelectionElement == null)
                    {
                        nextSelectionElement = deleteElement.PreviousSibling;
                    }

                    parent.RemoveChild(deleteElement);
                    rmlModified();
                    updateUndoStatus(editor.UndoRml);
                    editor.UndoRml = UnformattedRml;

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

        internal void updateUndoStatus(String oldMarkup, bool check = false)
        {
            //This is a hacky way to check for changes (optionally) it should not be needed when the popup editor is overhauled.
            //You can remove check and keep only the line in the if statement when you no longer need the check.
            String currentMarkup = UnformattedRml;
            if (!check || currentMarkup != oldMarkup)
            {
                undoBuffer.pushAndSkip(new TwoWayDelegateCommand<String, String>(undoRedoCallback, currentMarkup, undoRedoCallback, oldMarkup));
            }
        }

        private void undoRedoCallback(String rml)
        {
            cancelAndHideEditor();
            if (setDocumentRml(rml, true))
            {
                rmlModified();
            }
        }

        private void loadDocumentFile(String file, bool maintainScrollPosition)
        {
            if (file != null)
            {
                using (StreamReader sr = new StreamReader(context.ResourceProvider.openFile(file)))
                {
                    setDocumentRml(sr.ReadToEnd(), maintainScrollPosition);
                }
            }
        }

        private bool setDocumentRml(String rml, bool maintainScrollPosition)
        {
            float scrollLeft = 0.0f;
            float scrollTop = 0.0f;
            Element topContentElement;

            if (maintainScrollPosition)
            {
                topContentElement = TopContentElement;
                if (topContentElement != null)
                {
                    scrollLeft = topContentElement.ScrollLeft;
                    scrollTop = topContentElement.ScrollTop;
                }
            }

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

                        if (maintainScrollPosition)
                        {
                            topContentElement = TopContentElement;
                            if (topContentElement != null)
                            {
                                topContentElement.ScrollLeft = scrollLeft;
                                topContentElement.ScrollTop = scrollTop;
                            }
                        }

                        return true;
                    }
                }
            }
            return false;
        }
    }
}
