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
        public XmlGrammar()
            : base(false)
        {
            //Terminals
            Terminal comment = new CommentTerminal("comment", "<!--", "-->");
            NonGrammarTerminals.Add(comment);
            var number = new NumberLiteral("number");
            var stringLiteral = new StringLiteral("string", "\"", StringOptions.None);
            var stringContent = new XmlContentText("StringContent");
            IdentifierTerminal identifier = new IdentifierTerminal("Identifier");

            //Non Terminals
            NonTerminal document = new NonTerminal("document");
            NonTerminal elementStart = new NonTerminal("ElementStart");
            NonTerminal elementEnd = new NonTerminal("ElementEnd");
            NonTerminal openCloseElement = new NonTerminal("OpenCloseElement");
            NonTerminal element = new NonTerminal("Element");
            NonTerminal optionalElements = new NonTerminal("Elements");
            NonTerminal requiredElements = new NonTerminal("Elements");
            NonTerminal content = new NonTerminal("ElementContent");
            NonTerminal attribute = new NonTerminal("Attribute");
            NonTerminal optionalAttribute = new NonTerminal("Attribute");
            NonTerminal xmlDeclaration = new NonTerminal("XmlDeclaration");
            NonTerminal optionalXmlDeclaration = new NonTerminal("XmlDeclaration");

            //Rules
            this.Root = document;

            content.Rule = optionalElements | stringContent;

            attribute.Rule = identifier + ToTerm("=") + stringLiteral;
            optionalAttribute.Rule = MakeStarRule(optionalAttribute, attribute);

            elementStart.Rule = ToTerm("<") + identifier + optionalAttribute + ToTerm(">");
            elementEnd.Rule = ToTerm("</") + identifier + ToTerm(">");
            openCloseElement.Rule = ToTerm("<") + identifier + optionalAttribute + ToTerm("/>");
            element.Rule = (elementStart + content + elementEnd) | openCloseElement;
            optionalElements.Rule = MakeStarRule(optionalElements, element);
            requiredElements.Rule = MakePlusRule(requiredElements, element);

            xmlDeclaration.Rule = ToTerm("<?") + identifier + optionalAttribute + ToTerm("?>");
            optionalXmlDeclaration.Rule = MakeStarRule(optionalXmlDeclaration, xmlDeclaration);

            document.Rule = optionalXmlDeclaration + requiredElements;
        }

    }

    public class XmlContentText : Terminal
    {
        public XmlContentText(String name)
            : base(name)
        {

        }

        public override IList<string> GetFirsts()
        {
            return null;
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            int stopIndex = source.Text.IndexOf('<', source.Location.Position);
            if (stopIndex == source.Location.Position)
            {
                return null;
            }
            if (stopIndex < 0)
            {
                stopIndex = source.Text.Length;
            }
            source.PreviewPosition = stopIndex;
            return source.CreateToken(this.OutputTerminal);
        }
    }
}
