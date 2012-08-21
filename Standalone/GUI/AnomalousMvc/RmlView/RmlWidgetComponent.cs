using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;
using System.Text.RegularExpressions;

namespace Medical.GUI.AnomalousMvc
{
    public class RmlWidgetComponent : LayoutComponent
    {
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;
        private int imageWidth;

        private AnomalousMvcContext context;

        public RmlWidgetComponent(RmlView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            :base("Medical.GUI.AnomalousMvc.RmlView.RmlWidgetComponent.layout", viewHost)
        {
            this.context = context;

            rmlImage = (ImageBox)widget;
            rocketWidget = new RocketWidget(rmlImage);
            imageHeight = rmlImage.Height;

            if (view.RmlFile != null)
            {
                RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(view.RmlFile))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                RocketEventListenerInstancer.resetEventController();
            }

            view._fireComponentCreated(this);
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        public override void closing()
        {
            rocketWidget.RenderingEnabled = false;
            rocketWidget.InputEnabled = false;
            base.closing();
        }

        public override void populateViewData(IDataProvider dataProvider)
        {
            base.populateViewData(dataProvider);
            ElementDocument document = rocketWidget.Context.GetDocument(0);
            foreach (Element form in document.GetElementsByTagName("form"))
            {
                foreach (Element input in form.GetElementsByTagName("input"))
                {
                    String name = input.GetAttributeString("name");
                    String value = dataProvider.getValue(name);
                    String type = input.GetAttributeString("type").ToLowerInvariant();
                    if (value != null)
                    {
                        switch(type)
                        {
                            case "text":
                                input.SetAttribute("value", value);
                                break;
                            case "password":
                                input.SetAttribute("value", value);
                                break;
                            case "radio":
                                if (input.GetAttributeString("value") == value)
                                {
                                    input.SetAttribute("checked", "true");
                                }
                                break;
                            case "checkbox":
                                bool check;
                                if (bool.TryParse(value, out check) && check)
                                {
                                    input.SetAttribute("checked", "true");
                                }
                                break;
                            case "range":
                                input.SetAttribute("value", value);
                                break;
                        }
                    }
                }
                foreach (Element textArea in form.GetElementsByTagName("textarea"))
                {
                    String name = textArea.GetAttributeString("name");
                    String value = dataProvider.getValue(name);
                    if (value != null)
                    {
                        textArea.SetAttribute("value", value);
                    }
                }
                foreach (ElementFormControl select in form.GetElementsByTagName("select"))
                {
                    String name = select.GetAttributeString("name");
                    String value = dataProvider.getValue(name);
                    if (value != null)
                    {
                        select.Value = value;
                    }
                }
            }
        }

        public override void analyzeViewData(IDataProvider dataProvider)
        {
            base.analyzeViewData(dataProvider);
            ElementDocument document = rocketWidget.Context.GetDocument(0);

            runIfAnalysis(dataProvider, document);

            //Print statements
            foreach (Element print in document.GetElementsByTagName("print"))
            {
                String name = print.GetAttributeString("name");
                String value = dataProvider.getValue(name);
                if (value != null)
                {
                    print.InnerRml = value;
                }
            }
        }

        const String ifRegex = "(==)|(!=)|(>=)|(<=)|>|<";

        private void runIfAnalysis(IDataProvider dataProvider, ElementDocument document)
        {
            List<Element> removeElements = new List<Element>();
            foreach (Element element in document.GetElementsWithAttribute("condition"))
            {
                bool success = false;
                String statement = element.GetAttributeString("condition").Replace("&lt;", "<").Replace("&gt;", ">");
                Match match = Regex.Match(statement, ifRegex);
                if (match.Success)
                {
                    String left = statement.Substring(0, match.Index).Trim();
                    String right = statement.Substring(match.Index + match.Length).Trim();
                    String op = match.Value;

                    if (!String.IsNullOrEmpty(op) && !String.IsNullOrEmpty(left) && !String.IsNullOrEmpty(right))
                    {
                        if (left[0] == '%')
                        {
                            left = dataProvider.getValue(left.Substring(1, left.Length - 1));
                        }
                        if (right[0] == '%')
                        {
                            right = dataProvider.getValue(right.Substring(1, right.Length - 1));
                        }
                        if (!String.IsNullOrEmpty(left) && !String.IsNullOrEmpty(right))
                        {
                            switch (op)
                            {
                                case "==":
                                    success = left == right;
                                    break;
                                case "!=":
                                    success = left != right;
                                    break;
                                case ">=":
                                    decimal leftNum, rightNum;
                                    if (decimal.TryParse(left, out leftNum) && decimal.TryParse(right, out rightNum))
                                    {
                                        success = leftNum >= rightNum;
                                    }
                                    break;
                                case "<=":
                                    if (decimal.TryParse(left, out leftNum) && decimal.TryParse(right, out rightNum))
                                    {
                                        success = leftNum <= rightNum;
                                    }
                                    break;
                                case ">":
                                    if (decimal.TryParse(left, out leftNum) && decimal.TryParse(right, out rightNum))
                                    {
                                        success = leftNum > rightNum;
                                    }
                                    break;
                                case "<":
                                    if (decimal.TryParse(left, out leftNum) && decimal.TryParse(right, out rightNum))
                                    {
                                        success = leftNum < rightNum;
                                    }
                                    break;
                            }
                        }
                    }
                }

                if (!success)
                {
                    removeElements.Add(element);
                }
            }

            foreach (Element remove in removeElements)
            {
                Element parent = remove.ParentNode;
                if (parent != null)
                {
                    parent.RemoveChild(remove);
                }
            }
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
                RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(documentName))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                RocketEventListenerInstancer.resetEventController();
            }
        }
    }
}
