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

        private AnomalousMvcContext context;

        public RmlWysiwygComponent(RmlWysiwygView view, AnomalousMvcContext context, MyGUIViewHost viewHost, MedicalUICallback uiCallback)
            : base("Medical.GUI.RmlWysiwyg.RmlWysiwygComponent.layout", viewHost)
        {
            this.context = context;
            this.uiCallback = uiCallback;

            rmlImage = (ImageBox)widget;
            rocketWidget = new RocketWidget(rmlImage);
            rocketWidget.MouseButtonClick += new MyGUIEvent(rocketWidget_MouseButtonClick);
            imageHeight = rmlImage.Height;

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

        public void reloadDocument(String documentName)
        {
            RocketGuiManager.clearAllCaches();
            rocketWidget.Context.UnloadAllDocuments();

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

        public String CurrentRml
        {
            get
            {
                Element document = rocketWidget.Context.GetDocument(0);
                if (document != null)
                {
                    Variant templateName = document.GetAttribute("template");
                    if (templateName == null)
                    {
                        return formatRml(document.InnerRml);
                    }
                    else
                    {
                        Template template = TemplateCache.GetTemplate(templateName.StringValue);
                        if (template != null)
                        {
                            Element contentDocument = document.GetElementById(template.Content);
                            if (contentDocument != null)
                            {
                                return formatRml(contentDocument.InnerRml);
                            }
                            else
                            {
                                return formatRml(document.InnerRml);
                            }
                        }
                        else
                        {
                            return formatRml(document.InnerRml);
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
            bodyStart = inputRml.IndexOf(">", bodyStart, StringComparison.InvariantCultureIgnoreCase) + 1;
            documentStart = inputRml.Substring(0, bodyStart);

            int closeBodyStart = inputRml.IndexOf("</body", bodyStart, StringComparison.InvariantCultureIgnoreCase);
            documentEnd = inputRml.Substring(closeBodyStart);
        }

        void rocketWidget_MouseButtonClick(Widget source, EventArgs e)
        {
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
                }

                //Logging.Log.Debug(element.TagName);
                //Logging.Log.Debug(element.InnerRml);

                //int index = 0;
                //String name = null;
                //String value = null;
                //while (element.IterateAttributes(ref index, ref name, ref value))
                //{
                //    Logging.Log.Debug("Attr: {0} - {1}", name, value);
                //}
            }
        }

        private void showRmlElementEditor(Element element)
        {
            RmlElementEditor editor = RmlElementEditor.openTextEditor(uiCallback, element, (int)element.AbsoluteLeft + rocketWidget.AbsoluteLeft, (int)element.AbsoluteTop + rocketWidget.AbsoluteTop);
            editor.Hiding += (src, evt) =>
            {
                if (!disposed)
                {
                    element.InnerRml = editor.Text;
                    rocketWidget.renderOnNextFrame();
                    if (RmlEdited != null)
                    {
                        RmlEdited.Invoke(this);
                    }
                }
            };
        }
    }
}
