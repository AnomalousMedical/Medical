using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Medical.Irony
{
    [Language("XML", "1", "XML Grammar")]
    public class XmlGrammar : Grammar
    {
        public const String AttributeIdentifier = "AttributeIdentifier";
        public const String ElementIdentifier = "ElementIdentifier";
        public const String XmlDeclarationIdentifier = "XmlDeclarationIdentifier";

        public XmlGrammar()
            : base(false)
        {
            //Terminals
            Terminal comment = new CommentTerminal("comment", "<!--", "-->");
            NonGrammarTerminals.Add(comment);
            StringLiteral stringLiteral = new StringLiteral("string", "\"", StringOptions.None);
            Terminal stringContent = new ToTerminatorTerminal("StringContent", '<');
            KeyTerm elementOpener = ToTerm("<");
            KeyTerm closeElementOpener = ToTerm("</");
            KeyTerm elementCloser = ToTerm(">");
            KeyTerm openCloseElementCloser = ToTerm("/>");
            KeyTerm equals = ToTerm("=");
            KeyTerm xmlDeclOpen = ToTerm("<?");
            KeyTerm xmlDeclClose = ToTerm("?>");

            IdentifierTerminal attributeIdentifier = new IdentifierTerminal(AttributeIdentifier, ".", ".");
            IdentifierTerminal elementIdentifier = new IdentifierTerminal(ElementIdentifier, ".-", ".-");
            KeyTerm xmlDeclarationIdentifier = ToTerm("xml");

            //Non Terminals
            NonTerminal document = new NonTerminal("document");
            NonTerminal elementStart = new NonTerminal("ElementStart");
            NonTerminal elementEnd = new NonTerminal("ElementEnd");
            NonTerminal openCloseElement = new NonTerminal("OpenCloseElement");
            NonTerminal element = new NonTerminal("Element");
            NonTerminal requiredElements = new NonTerminal("RequiredElements");
            NonTerminal innerContent = new NonTerminal("InnerContent");
            NonTerminal content = new NonTerminal("Content");
            NonTerminal attribute = new NonTerminal("Attribute");
            NonTerminal optionalAttribute = new NonTerminal("OptionalAttribute");
            NonTerminal xmlDeclaration = new NonTerminal("XmlDeclaration");
            NonTerminal optionalXmlDeclaration = new NonTerminal("OptionalXmlDeclaration");

            //Rules
            this.Root = document;

            innerContent.Rule = element | stringContent;
            content.Rule = MakeStarRule(content, innerContent);

            attribute.Rule = attributeIdentifier + equals + stringLiteral;
            optionalAttribute.Rule = MakeStarRule(optionalAttribute, attribute);

            elementStart.Rule = elementOpener + elementIdentifier + optionalAttribute + elementCloser;
            elementEnd.Rule = closeElementOpener + elementIdentifier + elementCloser;
            openCloseElement.Rule = elementOpener + elementIdentifier + optionalAttribute + openCloseElementCloser;

            element.Rule = (elementStart + content + elementEnd) | openCloseElement;
            requiredElements.Rule = MakePlusRule(requiredElements, element);

            xmlDeclaration.Rule = xmlDeclOpen + xmlDeclarationIdentifier + optionalAttribute + xmlDeclClose;
            optionalXmlDeclaration.Rule = MakeStarRule(optionalXmlDeclaration, xmlDeclaration);

            document.Rule = optionalXmlDeclaration + requiredElements;

            MarkPunctuation(elementOpener, elementCloser, closeElementOpener, openCloseElementCloser, equals, xmlDeclOpen, xmlDeclClose);
            MarkTransient(innerContent);
        }
    }
}
