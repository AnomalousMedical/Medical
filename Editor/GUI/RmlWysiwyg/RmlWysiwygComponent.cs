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

namespace Medical.GUI
{
    public class RmlWysiwygComponent : LayoutComponent
    {
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
            rmlImage.MouseDrag += new MyGUIEvent(rmlImage_MouseDrag);
            rmlImage.MouseWheel += new MyGUIEvent(rmlImage_MouseWheel);
            rmlImage.EventScrollGesture += new MyGUIEvent(rmlImage_EventScrollGesture);
            imageHeight = rmlImage.Height;

            selectedElementManager = new SelectedElementManager(rmlImage.findWidget("SelectionWidget"));

            if (view.RmlFile != null)
            {
                //RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(view.RmlFile))
                {
                    if (document != null)
                    {
                        saveDocumentStartAndEnd(view.RmlFile);
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                //RocketEventListenerInstancer.resetEventController();
            }

            view._fireComponentCreated(this);
        }

        public override void Dispose()
        {
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
            selectedElementManager.clearSelectedElement();

            if (documentName != null)
            {
                //RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(documentName))
                {
                    if (document != null)
                    {
                        saveDocumentStartAndEnd(documentName);
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                //RocketEventListenerInstancer.resetEventController();
            }
        }

        public void insertRawRml(String rml)
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
                        insertElementIntoParent(div, selectedElementManager.SelectedElement);
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

                    selectedElementManager.SelectedElement = null;
                }
            }
        }

        public void changeSelectedElement(IntVector2 position, String innerRmlHint = null)
        {
            if (widget.contains(position.x, position.y))
            {
                position.x -= widget.AbsoluteLeft;
                position.y -= widget.AbsoluteTop;

                Element toSelect = rocketWidget.Context.FindElementAtPoint(position);
                Element selectedElement = selectedElementManager.SelectedElement;

                if (toSelect != selectedElement)
                {
                    if (toSelect != null)
                    {
                        Element topContentElement = TopContentElement;
                        if (toSelect != topContentElement)
                        {
                            if (!previewElement.isPreviewOrAncestor(toSelect))
                            {
                                selectedElementManager.SelectedElement = toSelect;
                                previewElement.hidePreviewElement();
                                ElementDocument document = rocketWidget.Context.GetDocument(0);
                                previewElement.showPreviewElement(document, innerRmlHint, toSelect.ParentNode, toSelect);
                            }
                        }
                        else
                        {
                            selectedElementManager.SelectedElement = null;
                            previewElement.hidePreviewElement();
                            ElementDocument document = rocketWidget.Context.GetDocument(0);
                            previewElement.showPreviewElement(document, innerRmlHint, toSelect, null);
                        }
                    }
                    else
                    {
                        selectedElementManager.SelectedElement = null;
                        previewElement.hidePreviewElement();
                    }
                }

                rmlModified();
            }
        }

        public String CurrentRml
        {
            get
            {
                Element topContentElemnt = TopContentElement;
                if (topContentElemnt != null)
                {
                    return formatRml(topContentElemnt.InnerRml);
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

        private String formatRml(String inputRml)
        {
            inputRml = inputRml.Insert(0, documentStart);
            inputRml += documentEnd;
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

        private void saveDocumentStartAndEnd(String file)
        {
            String inputRml;
            using (StreamReader sr = new StreamReader(context.ResourceProvider.openFile(file)))
            {
                inputRml = sr.ReadToEnd();
            }
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
                switch (element.TagName)
                {
                    case "h1":
                        showRmlElementEditor(element);
                        break;
                    case "p":
                        showRmlElementEditor(element);
                        break;
                    case "a":
                        showRmlElementEditor(element);
                        break;
                    case "input":
                        showRmlElementEditor(element);
                        break;
                    case "img":
                        showRmlElementEditor(element);
                        break;
                    default:
                        selectedElementManager.clearSelectedElement();
                        break;
                }
            }
        }

        void rmlImage_EventScrollGesture(Widget source, EventArgs e)
        {
            selectedElementManager.updateSelectionPosition();
        }

        void rmlImage_MouseWheel(Widget source, EventArgs e)
        {
            selectedElementManager.updateSelectionPosition();
        }

        void rmlImage_MouseDrag(Widget source, EventArgs e)
        {
            selectedElementManager.updateSelectionPosition();
        }

        private void showRmlElementEditor(Element element)
        {
            RmlElementEditor editor = RmlElementEditor.openTextEditor(uiCallback, browserProvider, element, (int)(element.AbsoluteLeft + element.ClientWidth) + rocketWidget.AbsoluteLeft, (int)element.AbsoluteTop + rocketWidget.AbsoluteTop);
            editor.Hiding += (src, evt) =>
            {
                if (editor.ApplyChanges && !disposed)
                {
                    String text = editor.Text;
                    if (isTextElement(element) && String.IsNullOrEmpty(text))
                    {
                        Element parent = element.ParentNode;
                        if (parent != null)
                        {
                            parent.RemoveChild(element);
                            if (element == selectedElementManager.SelectedElement)
                            {
                                selectedElementManager.clearSelectedElement();
                            }
                        }
                    }
                    else
                    {
                        element.InnerRml = editor.Text;
                    }
                    rmlModified();
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
                        upElement.addReference();
                        parent.RemoveChild(upElement);
                        parent.InsertBefore(upElement, previousSibling);
                        upElement.removeReference();
                        rmlModified();
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
                    }
                    rmlModified();
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

                    if (nextSelectionElement != null)
                    {
                        showRmlElementEditor(nextSelectionElement);
                    }
                    else
                    {
                        selectedElementManager.clearSelectedElement();
                    }
                }
            };
            currentEditor = editor;
            selectedElementManager.SelectedElement = element;
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
            selectedElementManager.updateSelectionPosition();
            rocketWidget.renderOnNextFrame();
        }

        private static void insertElementIntoParent(Element newElement, Element sibling)
        {
            Element parent = sibling.ParentNode;
            if (parent != null)
            {
                if (sibling == null)
                {
                    parent.AppendChild(newElement);
                }
                else
                {
                    parent.InsertBefore(newElement, sibling);
                }
            }
        }
    }
}
