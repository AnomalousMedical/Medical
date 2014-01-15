using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Medical.Irony;
using Engine;

namespace Medical.GUI
{
    class CssTextHighlighter : TextHighlighter
    {
        private static CssTextHighlighter instance;
        public static CssTextHighlighter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CssTextHighlighter();
                }
                return instance;
            }
        }

        LanguageData language;
        Parser parser;

        private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        private String commentColor = "#B675C6";
        private String propertyColor = "#99B2FF";
        private String valueColor = "#14EF35";
        private String selectorColor = "#FFC66D";
        private String punctuationColor = "#FFFFFF";
        private String otherTextColor = "#FFFFFF";

        private CssTextHighlighter()
        {
            language = new LanguageData(new CssGrammar());
            parser = new Parser(language);

            ConfigSection cssHighlightSection = EditorConfig.getConfigSection("CssColors");

            Color.TryFromRGBAString(cssHighlightSection.getValue("BackgroundColor", "#191919"), out backgroundColor, backgroundColor);
            commentColor = EditorConfig.readConfigHexColor(cssHighlightSection, "CommentColor", commentColor);
            propertyColor = EditorConfig.readConfigHexColor(cssHighlightSection, "PropertyColor", propertyColor);
            valueColor = EditorConfig.readConfigHexColor(cssHighlightSection, "ValueColor", valueColor);
            selectorColor = EditorConfig.readConfigHexColor(cssHighlightSection, "SelectorColor", selectorColor);
            punctuationColor = EditorConfig.readConfigHexColor(cssHighlightSection, "PunctuationColor", punctuationColor);
            otherTextColor = EditorConfig.readConfigHexColor(cssHighlightSection, "OtherTextColor", otherTextColor);
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
                        switch (token.Terminal.Name)
                        {
                            case CssGrammar.Property:
                                return propertyColor;
                            case CssGrammar.Value:
                                return valueColor;
                            default:
                                return selectorColor;
                        }
                    case TokenColor.Text:
                        return punctuationColor;
                    default:
                        return otherTextColor;
                }
            }
            else
            {
                return otherTextColor;
            }
        }
    }
}
