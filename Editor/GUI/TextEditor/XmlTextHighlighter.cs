using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Medical.Irony;
using Engine;

namespace Medical.GUI
{
    class XmlTextHighlighter : TextHighlighter
    {
        private static XmlTextHighlighter instance;
        public static XmlTextHighlighter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XmlTextHighlighter();
                }
                return instance;
            }
        }

        LanguageData language;
        Parser parser;
        private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        private String commentColor = "#B675C6";
        private String attributeColor = "#99B2FF";
        private String elementColor = "#FFC66D";
        private String stringColor = "#14EF35";
        private String textColor = "#FFFFFF";
        private String punctuationColor = "#CC7832";

        private XmlTextHighlighter()
        {
            language = new LanguageData(new XmlGrammar());
            parser = new Parser(language);

            ConfigSection rmlHighlightSection = EditorConfig.getConfigSection("XmlColors");

            Color.TryFromHexString(rmlHighlightSection.getValue("BackgroundColor", "#191919"), out backgroundColor, backgroundColor);
            commentColor = EditorConfig.readConfigHexColor(rmlHighlightSection, "CommentColor", commentColor);
            attributeColor = EditorConfig.readConfigHexColor(rmlHighlightSection, "AttributeColor", attributeColor);
            elementColor = EditorConfig.readConfigHexColor(rmlHighlightSection, "ElementColor", elementColor);
            stringColor = EditorConfig.readConfigHexColor(rmlHighlightSection, "StringColor", stringColor);
            textColor = EditorConfig.readConfigHexColor(rmlHighlightSection, "TextColor", textColor);
            punctuationColor = EditorConfig.readConfigHexColor(rmlHighlightSection, "PunctuationColor", punctuationColor);
        }

        public void colorString(StringBuilder input)
        {
            ParseTree parseTree = parser.Parse(input.ToString());

            int additionalOffset = 0;
            foreach (Token token in parseTree.Tokens)
            {
                int tokenStart = token.Location.Position;
                int tokenEnd = tokenStart + token.Length;

                input.Insert(tokenStart + additionalOffset, getColor(token));
                additionalOffset += 7;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
        }

        private String getColor(Token token)
        {
            if (token.EditorInfo != null)
            {
                switch (token.EditorInfo.Color)
                {
                    case TokenColor.Comment:
                        return commentColor;
                    case TokenColor.Identifier:
                        if (token.Terminal.Name == XmlGrammar.AttributeIdentifier)
                        {
                            return attributeColor;
                        }
                        return elementColor;
                    case TokenColor.Keyword:
                        return elementColor;
                    case TokenColor.String:
                        return stringColor;
                    case TokenColor.Text:
                        return punctuationColor;
                    default:
                        return textColor;
                }
            }
            else
            {
                return textColor;
            }
        }
    }
}
